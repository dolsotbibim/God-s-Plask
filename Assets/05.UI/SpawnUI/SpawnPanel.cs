using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class SpawnPanel : MonoBehaviour
{
    public GameObject Hand;
    public int SpawnUI_Length = 1000;
    RectTransform HandTransform;
    public Material[] SuccessMats;
    public GameObject SuccessRangePrefab;
    public float SuccessRange = 100;
    public int ValidDistance = 10;
    public GameObject[] SuccessRangeIndices;
    public float Rambda = 1;
    private bool isStuck = false;
    public bool isFever = false;
    public bool IsWaiting = false;
    public ParticleSystem Flash;
    public bool IsStuck
    {
        get { return isStuck; }
        set
        {
            isStuck = value;
            IsWaiting = false;
            if (isStuck)
            {
                StuckTime = 1f;
            }
        }
    }
    int[] Map;
    PoolManager PoolManager;
    Coroutine SpawnPanelRoutine = null;
    List<SuccessRange> SuccessRangePool = new List<SuccessRange>();
    private float _lastXPosition = -1000;
    private bool _isMovingRight = true;

    private Coroutine SuccessRangeRoutine = null;
    private List<Coroutine> EventCoroutines = new List<Coroutine>();
    public Material LastMat;
    public float ExpandDuration = 0.25f;
    public float EffectTime = 0.25f;

    private void Awake()
    {
        SuccessRange = 100;
    }
    private void Start()
    {
        PoolManager = GetComponent<PoolManager>();
        HandTransform = Hand.GetComponent<RectTransform>();

        SpawnPanelRoutine = StartCoroutine(MoveHand());
    }
    float totalTime;

    public float StuckTime = 0f;

    List<List<(int, int)>> TutorialPulseList = new List<List<(int, int)>>()
    {
        new List<(int, int)>() { (500,1) },
        new List<(int, int)>() { (450,1), (550,1) },
        new List<(int, int)>() { (200,1), (250,1), (275,1) },
        new List<(int, int)>() { (750,1), (730,1), (775,1), (790,1), (222, 1) },
        new List<(int, int)>() { (400,1), (410,1), (425,1), (380,1), (390,1), (857, 1), (674, 1) },
        new List<(int, int)>() { (450,1), (470,1), (495,1), (520,1), (530,1), (100, 1), (150, 1), (888, 1) },
        new List<(int, int)>() { (500,1), (888, 1) },
        new List<(int, int)>() { (900,1), (850, 1), (100, 1), (300, 1) },
        new List<(int, int)>() { (300,1), (325, 1), (250, 1), (777, 1), (666, 1) },
        new List<(int, int)>() { (300,1), (380, 1), (366, 1), (333, 1), (700, 1) },
        new List<(int, int)>() { (300,1), (352, 1), (287, 1), (256, 1), (333, 1), (666, 1), (500, 1) },
        new List<(int, int)>()
        {
            (0, 1), (20, 1), (40, 1), (60, 1), (80, 1), (100, 1), (120, 1), (140, 1), (160, 1), (180, 1),
            (200, 1), (220, 1), (240, 1), (260, 1), (280, 1), (300, 1), (320, 1), (340, 1), (360, 1), (380, 1),
            (400, 1), (420, 1), (440, 1), (460, 1), (480, 1), (500, 1), (520, 1), (540, 1), (560, 1), (580, 1),
            (600, 1), (620, 1), (640, 1), (660, 1), (680, 1), (700, 1), (720, 1), (740, 1), (760, 1), (780, 1),
            (800, 1), (820, 1), (840, 1), (860, 1), (880, 1), (900, 1), (920, 1), (940, 1), (960, 1), (980, 1), (1000, 1)
        }
    };

    List<float> TutoHand = new List<float>()
    {
        0,
        0,
        -260,
        (750 + 730 + 775 + 790) / 4 - 500,
        (400 + 410 + 425 + 380 + 390) / 5 - 500,
        (450 + 470 + 495 + 520 + 530) / 5 - 500,
        0,
        375,
        -210,
        -160,
        -195,
        0,
    };
    public IEnumerator MoveHand()
    {
        
        while (true)
        {
            if (BlackHole.instance.gameObject.activeSelf) yield break;
            HandTransform.anchoredPosition = new Vector3(-SpawnUI_Length / 2, HandTransform.anchoredPosition.y, 0);
            if (isFever && !UIManager.Instance.IsTutorial)
            {
                isFever = false;
                
                IsWaiting = true;
                UpdateSuccessRange(Rambda * FeverGage.Instance.FeverRambda);
                yield return new WaitForSeconds(0.5f);
                IsWaiting = false;
                
                yield return FeverHandRoutine();
                FeverGage.Instance.FeverStack = FeverGage.Instance.FeverStack;
                
                Spawner.Instance.ResetCombo();
            }
            else if(!UIManager.Instance.IsTutorial)
            {
                IsWaiting = true;
                UpdateSuccessRange(Rambda);
                yield return new WaitForSeconds(0.5f);
                IsWaiting = false;
                yield return MoveHandRoutine();
                Spawner.Instance.ResetCombo();

            }
            else
            {
                if (isFever)
                {
                    isFever = false;

                    StartCoroutine(TutorialPulse(TutorialPulseList[UIManager.Instance.TutorialStep]));

                    yield return new WaitForSeconds(0.5f);
                    IsWaiting = false;

                    yield return MoveTutoHandRoutine(TutoHand[UIManager.Instance.TutorialStep], 2);
                    FeverGage.Instance.FeverStack = FeverGage.Instance.FeverStack;

                    Spawner.Instance.ResetCombo();
                }
                else
                {
                    StartCoroutine(TutorialPulse(TutorialPulseList[UIManager.Instance.TutorialStep]));
                    yield return new WaitForSeconds(0.5f);
                    IsWaiting = false;
                    yield return MoveTutoHandRoutine(TutoHand[UIManager.Instance.TutorialStep], 1);
                    Spawner.Instance.ResetCombo();

                }
            }
        }
    }


    IEnumerator MoveHandRoutine()
    {
        float time = 0;
        float currentX = 0;
        float oldTotalTime = Spawner.Instance.RoundTripTime; // 초기 totalTime 저장

        while (true)
        {
            float newTotalTime = Spawner.Instance.RoundTripTime;

            if (time >= oldTotalTime * 2) break;

            if (newTotalTime != oldTotalTime)
            {
                float normalizedTime = time / oldTotalTime;
                time = normalizedTime * newTotalTime;
                oldTotalTime = newTotalTime;
            }

            while (StuckTime > 0)
            {
                StuckTime -= Time.deltaTime;
                yield return null;
            }

            _lastXPosition = currentX;
            IsStuck = false;
            time += Time.deltaTime;
            float pingPongValue = Mathf.PingPong(time, newTotalTime);
            float normalizedPingPong = pingPongValue / newTotalTime;
            currentX = Mathf.Lerp(-SpawnUI_Length / 2 + 1, SpawnUI_Length / 2 - 1, normalizedPingPong);
            HandTransform.anchoredPosition = new Vector2(currentX, HandTransform.anchoredPosition.y);

            yield return null;
        }

    }

    IEnumerator MoveTutoHandRoutine(float targetX, int isFever)
    {
        float currentX = HandTransform.anchoredPosition.x;
        IsWaiting = true;

        while (true)
        {
            while (StuckTime > 0)
            {
                StuckTime -= Time.deltaTime;
                yield return null;
            }

            _lastXPosition = currentX;

            currentX = Mathf.MoveTowards(currentX, targetX, 200 * Time.deltaTime * isFever);
            HandTransform.anchoredPosition = new Vector2(currentX, HandTransform.anchoredPosition.y);
            if(Mathf.Abs(currentX - targetX) < 0.1f && !UIManager.Instance.Tutorials[UIManager.Instance.TutorialStep].activeSelf)
            {
                IsWaiting = false;
                UIManager.Instance.Tutorials[UIManager.Instance.TutorialStep].SetActive(true);
            }
            yield return null;
        }
        HandTransform.anchoredPosition = new Vector2(targetX, HandTransform.anchoredPosition.y);
    }

    IEnumerator FeverHandRoutine()
    {
        float currentX = 0;
        float time = 0;

        float initialTotalTime = Spawner.Instance.RoundTripTime / FeverGage.Instance.FeverSpeed;
        float oldTotalTime = initialTotalTime;

        while (true)
        {
            float newTotalTime = Spawner.Instance.RoundTripTime / FeverGage.Instance.FeverSpeed;

            if (time >= oldTotalTime * 2) break;

            if (newTotalTime != oldTotalTime)
            {
                float normalizedTime = time / oldTotalTime;
                time = normalizedTime * newTotalTime;
                oldTotalTime = newTotalTime;
            }

            while (StuckTime > 0)
            {
                StuckTime -= Time.deltaTime;
                yield return null;
            }

            _lastXPosition = currentX;
            IsStuck = false;
            time += Time.deltaTime;
            float pingPongValue = Mathf.PingPong(time, newTotalTime);
            float normalizedPingPong = pingPongValue / newTotalTime;
            currentX = Mathf.Lerp(-SpawnUI_Length / 2 + 1, SpawnUI_Length / 2 - 1, normalizedPingPong);
            HandTransform.anchoredPosition = new Vector2(currentX, HandTransform.anchoredPosition.y);

            yield return null;
        }

    }

    public int CheckHand()
    {
        IsWaiting = true;
        float HandPos = HandTransform.anchoredPosition.x;
        return Map[Mathf.RoundToInt(HandPos) + Map.Length / 2];
    }

    public IEnumerator Restart()
    {
        StopCoroutine(SpawnPanelRoutine);

        yield return new WaitForSeconds(0.5f);
        _lastXPosition = -1000;
        _isMovingRight = true;
        SpawnPanelRoutine = StartCoroutine(MoveHand());
    }

    // UpdateSuccessRange는 이제 코루틴을 시작/중단으로 변경
    void UpdateSuccessRange(float Rambda)
    {
        // 이전 코루틴이 동작 중이면 종료
        if (SuccessRangeRoutine != null)
        {
            StopCoroutine(SuccessRangeRoutine);
            SuccessRangeRoutine = null;
        }

        // 이벤트별 코루틴도 중단
        if (EventCoroutines != null)
        {
            foreach (var c in EventCoroutines)
            {
                if (c != null) StopCoroutine(c);
            }
            EventCoroutines.Clear();
        }

        SuccessRangeRoutine = StartCoroutine(AnimateSuccessRanges(Rambda));
    }

    public IEnumerator AnimateSuccessRanges(float Rambda)
    {
        List<(int, int)> centers = GenerateEventPositions(Rambda);
        List<Image> currentFrameImages = new List<Image>();

        float targetRadius = Mathf.Clamp(SuccessRange, 0f, MaxRange);
        int eventsCount = centers.Count;
        float[] radii = new float[eventsCount];
        bool[] finished = new bool[eventsCount];
        for (int i = 0; i < eventsCount; i++) { radii[i] = 0f; finished[i] = false; }

        float duration = Mathf.Max(0.01f, ExpandDuration);
        EventCoroutines.Clear();

        int RandomN = Random.Range(0, 3);

        void StartExpandCoroutine(int i, float delay)
        {
            Coroutine c = StartCoroutine(ExpandEventCoroutine(i, delay, targetRadius, duration, radii, finished, centers[i].Item1, centers[i].Item2));
            EventCoroutines.Add(c);
        }

        if (RandomN == 0)
        {
            for (int i = 0; i < eventsCount; i++) StartExpandCoroutine(i, EffectTime / eventsCount * i);
        }
        else if (RandomN == 1)
        {
            StartExpandCoroutine(0, 0f);
            for (int i = 1; i < eventsCount; i++) StartExpandCoroutine(i, Random.Range(0, EffectTime));
        }
        else if (RandomN == 2)
        {
            for (int i = 0; i < eventsCount; i++) StartExpandCoroutine(i, EffectTime - EffectTime / eventsCount * i);
        }

        while (true)
        {
            int[] coverage = new int[ArraySize];
            for (int e = 0; e < eventsCount; e++)
            {
                int center = centers[e].Item1;
                int r = Mathf.RoundToInt(radii[e]);
                int start = Mathf.Max(0, center - r);
                int end = Mathf.Min(ArraySize - 1, center + r);

                for (int i = start; i <= end; i++)
                {
                    coverage[i] += centers[e].Item2;
                    coverage[i] = Mathf.Clamp(coverage[i], 0, MaxNesting);
                }
            }
            Map = coverage;

            List<(int Value, int Length)> compressed = CompressMapWithTolerance(coverage, ValidDistance);
            currentFrameImages.Clear();

            foreach (SuccessRange obj in SuccessRangePool)
            {
                if (obj != null && obj.gameObject.activeSelf)
                {
                    Image imgToDestroy = obj.GetComponent<Image>();
                    if (imgToDestroy != null && imgToDestroy.material != null)
                    {
                        if (imgToDestroy.material != imgToDestroy.defaultMaterial &&
                            imgToDestroy.material.name.EndsWith("(Instance)"))
                        {
                            Destroy(imgToDestroy.material);
                        }
                        imgToDestroy.material = null;
                    }

                    PoolManager.TakeToPool<SuccessRange>(obj);
                }
            }
            SuccessRangePool.Clear();

            int Pos = -SpawnUI_Length / 2;
            for (int i = 0; i < compressed.Count; i++)
            {
                int val = Mathf.Min(compressed[i].Value, MaxNesting);
                int length = compressed[i].Length;

                if (val > 0)
                {
                    if (length < ValidDistance)
                    {
                        if (val > 1)
                        {
                            val -= 1;
                            compressed[i] = (val, length);
                        }
                        else 
                        {
                            compressed[i] = (val - 1, length);
                            Pos += length; 
                            continue; 
                        }
                    }

                    SuccessRange sr = PoolManager.GetFromPool<SuccessRange>();
                    if (sr == null) { Pos += length; continue; }
                    if (!SuccessRangePool.Contains(sr)) SuccessRangePool.Add(sr);

                    int parentIndex = Mathf.Clamp(val - 1, 0, SuccessRangeIndices.Length - 1);
                    sr.transform.SetParent(SuccessRangeIndices[parentIndex].transform, false);

                    RectTransform Rect = sr.GetComponent<RectTransform>();
                    float centerPos = Pos + length / 2f;
                    Rect.anchoredPosition = new Vector3(centerPos, 0, 0);
                    Rect.localScale = Vector3.one;
                    Rect.sizeDelta = new Vector2(length, Rect.sizeDelta.y);

                    Image img = Rect.GetComponent<Image>();
                    if (img != null && SuccessMats.Length > 0)
                    {
                        int matIndex = (val - 1) % 5;
                        Material baseMat = SuccessMats[matIndex];
                        if (val >= 11) baseMat = LastMat;
                        if (img.material != null && img.material != baseMat)
                        {
                            if (img.material.name.EndsWith("(Instance)"))
                            {
                                Destroy(img.material);
                            }
                        }
                        img.material = GameObject.Instantiate(baseMat);

                        currentFrameImages.Add(img);

                        if (img.material != null)
                        {
                            if (compressed[i].Value > SuccessMats.Length)
                                img.material.SetFloat("_Size", 1);
                            else
                                img.material.SetFloat("_Size", img.GetComponent<RectTransform>().sizeDelta.x);
                        }
                    }
                }
                Pos += length;
            }

            bool allFinished = true;
            for (int i = 0; i < eventsCount; i++)
            {
                if (!finished[i]) { allFinished = false; break; }
            }

            if (allFinished)
            {
                void ApplyBoost(System.Func<IEnumerable<int>, int> selector, System.Func<(int Value, int Length), int> keySelector, string debugName, bool isLengthBased)
                {
                    int maxOrMinVal = selector(compressed.Where(t => t.Value != 0).Select(keySelector));

                    int targetIndexInCompressed = compressed.FindIndex(t => keySelector(t) == maxOrMinVal);
                    if (targetIndexInCompressed == -1) return;

                    int uiIndex = 0;
                    int accumulatedLength = -SpawnUI_Length / 2;

                    for (int i = 0; i < targetIndexInCompressed; i++)
                    {
                        int val = Mathf.Min(compressed[i].Value, MaxNesting);
                        int length = compressed[i].Length;

                        if (val > 0)
                        {
                            if (length < ValidDistance)
                            {
                                if (val > 1) uiIndex++;
                            }
                            else
                            {
                                uiIndex++;
                            }
                        }
                        accumulatedLength += length;
                    }

                    if (uiIndex >= currentFrameImages.Count) return;
                    
                    Image targetImage = currentFrameImages[uiIndex];
                    Material baseMat = SuccessMats[(compressed[targetIndexInCompressed].Value + boostAmount - 1) % 5];
                    if ((compressed[targetIndexInCompressed].Value + boostAmount - 1) >= 10) baseMat = LastMat;
                    (int Value, int Length) itemToModify = compressed[targetIndexInCompressed];
                    itemToModify.Value += boostAmount;
                    compressed[targetIndexInCompressed] = itemToModify;

                    if (targetImage.material != null && targetImage.material != baseMat)
                    {
                        if (targetImage.material.name.EndsWith("(Instance)"))
                        {
                            Destroy(targetImage.material);
                        }
                    }
                    targetImage.material = GameObject.Instantiate(baseMat);
                    if (compressed[targetIndexInCompressed].Value > SuccessMats.Length)
                        targetImage.material.SetFloat("_Size", 1);
                    else
                        targetImage.material.SetFloat("_Size", targetImage.GetComponent<RectTransform>().sizeDelta.x);

                    int mapIndexStart = accumulatedLength + SpawnUI_Length / 2;
                    int clusterLength = compressed[targetIndexInCompressed].Length;

                    for (int i = 0; i < clusterLength; i++)
                    {
                        int mapIndex = mapIndexStart + i;

                        if (mapIndex >= 0 && mapIndex < ArraySize)
                        {
                            Map[mapIndex] += boostAmount;
                        }
                    }

                    Flash.GetComponent<RectTransform>().anchoredPosition = new Vector3(accumulatedLength + compressed[targetIndexInCompressed].Length / 2, -25, 0);
                    Flash.Emit(1);
                }

                if (ObjetManager.Instance.CheckObjetEquiped<Supernova>())
                {
                    ApplyBoost(Enumerable.Max, t => t.Value, "TheBrightest", false);
                    yield return null;

                }
                if (ObjetManager.Instance.CheckObjetEquiped<Arrow>())
                {
                    ApplyBoost(Enumerable.Max, t => t.Length, "TheLongest", true);
                    yield return null;

                }

                if (ObjetManager.Instance.CheckObjetEquiped<Spear>())
                {
                    ApplyBoost(Enumerable.Min, t => t.Length, "TheShortest", true);
                    yield return null;

                }

                break;
            }
            yield return null;
        }

        EventCoroutines.Clear();
        SuccessRangeRoutine = null;

    }
    public int boostAmount = 1;
    public int MaxNesting = 5;
    
    // 각 이벤트별 반경 확장 코루틴 (독립 실행, radii와 finished는 외부에서 전달)
    private IEnumerator ExpandEventCoroutine(int index, float startDelay, float targetRadius, float duration, float[] radii, bool[] finished, int pos, int multiplier)
    {
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);
        SpawnPS ps = PoolManager.GetFromPool<SpawnPS>();
        if (multiplier > 1)
        {
            ps.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", new Color(191 / 255f, 15 / 255f, 52 / 255f)  * 4f);
        }
        else
            ps.GetComponent<ParticleSystemRenderer>().material.SetColor("_Color", new Color(97 / 255f, 52 / 255f, 191 / 255f) * 3.41f);

        ps.GetComponent<RectTransform>().anchoredPosition = new Vector3(pos -500, -25, -12);
        ps.GetComponent<ParticleSystem>().Emit(1);
        SoundManager.Instance.PlaySFX(0, 1f, true);
        float t = 0f;
        float from = 0f;
        yield return new WaitForSeconds(0.05f);
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            radii[index] = Mathf.Lerp(from, targetRadius, Mathf.SmoothStep(0f, 1f, k));
            yield return null;
        }
        PoolManager.TakeToPool<SpawnPS>(ps);
        radii[index] = targetRadius;
        finished[index] = true;
    }

    private const int ArraySize = 1001;
    private const int MaxRange = 1000;

    public static List<(int Value, int Length)> CompressMapWithTolerance(int[] map, int nThreshold)
    {
        if (map == null || map.Length == 0 || nThreshold < 1)
        {
            return new List<(int Value, int Length)>();
        }

        List<(int Value, int Length)> compressedList = new List<(int Value, int Length)>();

        int currentValue = map[0];
        int currentLength = 1;

        int diffCount = 0;

        int lastDiffIndex = -1;

        for (int i = 1; i < map.Length; i++)
        {
            if (map[i] == currentValue)
            {
                currentLength++;
                diffCount = 0;
                lastDiffIndex = -1;
            }
            else
            {
                if (diffCount == 0)
                {
                    lastDiffIndex = i;
                }

                diffCount++;

                if (diffCount >= nThreshold)
                {
                    int lengthToCompress = lastDiffIndex;
                    if (lengthToCompress > 0)
                    {
                        compressedList.Add((currentValue, currentLength - (diffCount - 1)));
                    }

                    currentValue = map[lastDiffIndex];

                    currentLength = diffCount;

                    diffCount = 0;
                    lastDiffIndex = -1;
                }
                else
                {
                    currentLength++;
                }
            }
        }

        if (currentLength > 0)
        {
            compressedList.Add((currentValue, currentLength));
        }

        return compressedList;
    }

    public static int[] GetEventMap(List<int> eventPositions, int successRange)
    {
        int[] successMap = new int[ArraySize];

        foreach (int center in eventPositions)
        {
            int start = center - successRange;
            int end = center + successRange;

            int startIndex = Mathf.Max(0, start);
            int endIndex = Mathf.Min(ArraySize - 1, end);

            for (int i = startIndex; i <= endIndex; i++)
            {
                successMap[i]++;
            }
        }

        return successMap;
    }

    public int EventMultiplier = 2;
    public float EventMultiplyChance = 0.5f;
    static int minDistance = 20;
    public List<(int, int)> GenerateEventPositions(float lambda)
    {
        int eventMultiplier = 1;
        List<(int, int)> eventPositions = new List<(int, int)>();

        const int MAX_RETRIES = 100;
        if (Random.value < EventMultiplyChance)
            eventMultiplier = EventMultiplier;
        else
            eventMultiplier = 1;
        int initialEventPos = Random.Range(0, MaxRange + 1);
        eventPositions.Add((initialEventPos, eventMultiplier));

        int additionalEventsNeeded = GetPoissonRandom(lambda);

        int eventsGenerated = 0;
        while (eventsGenerated < additionalEventsNeeded)
        {
            int retries = 0;
            bool positionFound = false;

            while (retries < MAX_RETRIES)
            {
                int newPos = Random.Range(0, MaxRange + 1);
                bool isTooClose = false;

                for(int i = 0; i < eventPositions.Count; i++)
                {
                    int existingPos = eventPositions[i].Item1;
                    if (Mathf.Abs(newPos - existingPos) < minDistance)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                if (!isTooClose)
                {
                    if (Random.value < EventMultiplyChance)
                        eventMultiplier = EventMultiplier;
                    else 
                        eventMultiplier = 1;
                    eventPositions.Add((newPos, eventMultiplier));
                    positionFound = true;
                    break;
                }

                retries++;
            }

            if (positionFound)
            {
                eventsGenerated++;
            }
            else
            {
                break;
            }
        }

        eventPositions.Sort();
        return eventPositions;
    }

    private static int GetPoissonRandom(float lambda)
    {
        if (lambda <= 0) return 0;

        float L = Mathf.Exp(-lambda);
        int k = 0;
        float p = 1.0f;
        do
        {
            k++;
            p *= Random.value;
        } while (p > L);

        return k - 1;
    }

    public IEnumerator TutorialPulse(List<(int, int)> centers)
    {
        List<Image> currentFrameImages = new List<Image>();

        float targetRadius = Mathf.Clamp(SuccessRange, 0f, MaxRange);
        int eventsCount = centers.Count;
        float[] radii = new float[eventsCount];
        bool[] finished = new bool[eventsCount];
        for (int i = 0; i < eventsCount; i++) { radii[i] = 0f; finished[i] = false; }

        float duration = Mathf.Max(0.01f, ExpandDuration);
        EventCoroutines.Clear();

        int RandomN = Random.Range(0, 3);

        void StartExpandCoroutine(int i, float delay)
        {
            Coroutine c = StartCoroutine(ExpandEventCoroutine(i, delay, targetRadius, duration, radii, finished, centers[i].Item1, centers[i].Item2));
            EventCoroutines.Add(c);
        }

        if (RandomN == 0)
        {
            for (int i = 0; i < eventsCount; i++) StartExpandCoroutine(i, EffectTime / eventsCount * i);
        }
        else if (RandomN == 1)
        {
            StartExpandCoroutine(0, 0f);
            for (int i = 1; i < eventsCount; i++) StartExpandCoroutine(i, Random.Range(0, EffectTime));
        }
        else if (RandomN == 2)
        {
            for (int i = 0; i < eventsCount; i++) StartExpandCoroutine(i, EffectTime - EffectTime / eventsCount * i);
        }

        while (true)
        {
            int[] coverage = new int[ArraySize];
            for (int e = 0; e < eventsCount; e++)
            {
                int center = centers[e].Item1;
                int r = Mathf.RoundToInt(radii[e]);
                int start = Mathf.Max(0, center - r);
                int end = Mathf.Min(ArraySize - 1, center + r);

                for (int i = start; i <= end; i++)
                {
                    coverage[i] += centers[e].Item2;
                    coverage[i] = Mathf.Clamp(coverage[i], 0, MaxNesting);
                }
            }
            Map = coverage;

            List<(int Value, int Length)> compressed = CompressMapWithTolerance(coverage, ValidDistance);
            currentFrameImages.Clear();

            foreach (SuccessRange obj in SuccessRangePool)
            {
                if (obj != null && obj.gameObject.activeSelf)
                {
                    Image imgToDestroy = obj.GetComponent<Image>();
                    if (imgToDestroy != null && imgToDestroy.material != null)
                    {
                        if (imgToDestroy.material != imgToDestroy.defaultMaterial &&
                            imgToDestroy.material.name.EndsWith("(Instance)"))
                        {
                            Destroy(imgToDestroy.material);
                        }
                        imgToDestroy.material = null;
                    }

                    PoolManager.TakeToPool<SuccessRange>(obj);
                }
            }
            SuccessRangePool.Clear();

            int Pos = -SpawnUI_Length / 2;
            for (int i = 0; i < compressed.Count; i++)
            {
                int val = Mathf.Min(compressed[i].Value, MaxNesting);
                int length = compressed[i].Length;

                if (val > 0)
                {
                    if (length < ValidDistance)
                    {
                        if (val > 1)
                        {
                            val -= 1;
                            compressed[i] = (val, length);
                        }
                        else
                        {
                            compressed[i] = (val - 1, length);
                            Pos += length;
                            continue;
                        }
                    }

                    SuccessRange sr = PoolManager.GetFromPool<SuccessRange>();
                    if (sr == null) { Pos += length; continue; }
                    if (!SuccessRangePool.Contains(sr)) SuccessRangePool.Add(sr);

                    int parentIndex = Mathf.Clamp(val - 1, 0, SuccessRangeIndices.Length - 1);
                    sr.transform.SetParent(SuccessRangeIndices[parentIndex].transform, false);

                    RectTransform Rect = sr.GetComponent<RectTransform>();
                    float centerPos = Pos + length / 2f;
                    Rect.anchoredPosition = new Vector3(centerPos, 0, 0);
                    Rect.localScale = Vector3.one;
                    Rect.sizeDelta = new Vector2(length, Rect.sizeDelta.y);

                    Image img = Rect.GetComponent<Image>();
                    if (img != null && SuccessMats.Length > 0)
                    {
                        int matIndex = (val - 1) % 5;
                        Material baseMat = SuccessMats[matIndex];

                        if (img.material != null && img.material != baseMat)
                        {
                            if (img.material.name.EndsWith("(Instance)"))
                            {
                                Destroy(img.material);
                            }
                        }
                        img.material = GameObject.Instantiate(baseMat);

                        currentFrameImages.Add(img);

                        if (img.material != null)
                        {
                            if (compressed[i].Value > SuccessMats.Length)
                                img.material.SetFloat("_Size", 1);
                            else
                                img.material.SetFloat("_Size", img.GetComponent<RectTransform>().sizeDelta.x);
                        }
                    }
                }
                Pos += length;
            }

            bool allFinished = true;
            for (int i = 0; i < eventsCount; i++)
            {
                if (!finished[i]) { allFinished = false; break; }
            }

            if (allFinished)
            {
                break;
            }
            yield return null;
        }

        EventCoroutines.Clear();
        SuccessRangeRoutine = null;

    }
}
