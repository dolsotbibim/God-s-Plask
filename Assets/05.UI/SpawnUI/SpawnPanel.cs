using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using Redcode.Pools;
using UnityEngine.Rendering;
public class SpawnPanel : MonoBehaviour
{
    public GameObject Hand;
    public int SpawnUI_Length = 1000;
    RectTransform HandTransform;
    public Material[] SuccessMats;
    public GameObject SuccessRangePrefab;
    public float SuccessRange;
    public int ValidDistance = 10;
    public GameObject[] SuccessRangeIndices;
    public float Rambda = 1;
    private bool isStuck = false;
    public bool isFever = false;
    public bool IsWaiting = false;
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

    public float ExpandDuration = 0.25f;
    public float EffectTime = 0.25f;


    private void Start()
    {
        PoolManager = GetComponent<PoolManager>();
        HandTransform = Hand.GetComponent<RectTransform>();
        SuccessRange = 100;

        SpawnPanelRoutine = StartCoroutine(MoveHand());
    }
    float totalTime;

    public float StuckTime = 0f;
    public IEnumerator MoveHand()
    {
        while (true)
        {
            HandTransform.anchoredPosition = new Vector3(-SpawnUI_Length / 2, HandTransform.anchoredPosition.y, 0);

            if (isFever)
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
            else
            {
                IsWaiting = true;
                UpdateSuccessRange(Rambda);
                yield return new WaitForSeconds(0.5f);
                IsWaiting = false;
                yield return MoveHandRoutine();
                Spawner.Instance.ResetCombo();

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

        // 시작: 각 이벤트에서 반경이 0 -> SuccessRange로 확장하도록 애니메이션 (각 이벤트마다 코루틴 분리)
        SuccessRangeRoutine = StartCoroutine(AnimateSuccessRanges(Rambda));
    }

    // 애니메이션 코루틴: 각 이벤트 중심에서부터 반경을 키우며 맵을 재생성하고 겹침에 따라 색을 분리해서 렌더링
    IEnumerator AnimateSuccessRanges(float Rambda)
    {
        // 생성된 이벤트 위치
        List<int> centers = GenerateEventPositions(Rambda);
        List<Image> images = new List<Image>();
        // 각 이벤트의 현재 반경(단위: 인덱스)
        float targetRadius = Mathf.Clamp(SuccessRange, 0f, MaxRange);
        int eventsCount = centers.Count;
        float[] radii = new float[eventsCount];
        bool[] finished = new bool[eventsCount];
        for (int i = 0; i < eventsCount; i++) { radii[i] = 0f; finished[i] = false; }

        // 확장 속도 관련: duration은 인스펙터 값 사용
        float duration = Mathf.Max(0.01f, ExpandDuration);

        EventCoroutines.Clear();

        int RandomN = Random.Range(0, 4);
        if(RandomN == 0)
            for (int i = 0; i < eventsCount; i++)
            {
                float startDelay = EffectTime / eventsCount * i;
                Coroutine c = StartCoroutine(ExpandEventCoroutine(i, startDelay, targetRadius, duration, radii, finished, centers[i]));
                EventCoroutines.Add(c);
            }
        else if(RandomN == 1)
            for (int i = 0; i < eventsCount; i++)
            {
                Coroutine c = StartCoroutine(ExpandEventCoroutine(i, 0, targetRadius, duration, radii, finished, centers[i]));
                EventCoroutines.Add(c);
            }
        else if(RandomN == 2)
        {
            Coroutine c = StartCoroutine(ExpandEventCoroutine(0, 0, targetRadius, duration, radii, finished, centers[0]));
            EventCoroutines.Add(c);
            for (int i = 1; i < eventsCount; i++)
            {
                float startDelay = Random.Range(0, EffectTime);
                c = StartCoroutine(ExpandEventCoroutine(i, startDelay, targetRadius, duration, radii, finished, centers[i]));
                EventCoroutines.Add(c);
            }
        }
        else if (RandomN == 3)
            for (int i = 0; i < eventsCount; i++)
            {
                float startDelay = EffectTime - EffectTime / eventsCount * i;
                Coroutine c = StartCoroutine(ExpandEventCoroutine(i, startDelay, targetRadius, duration, radii, finished, centers[i]));
                EventCoroutines.Add(c);
            }


        // 중앙 루프: 각 프레임마다 coverage 계산하고 렌더
        while (true)
        {
            // coverage map 계산
            int[] coverage = new int[ArraySize];
            for (int e = 0; e < eventsCount; e++)
            {
                int center = centers[e];
                int r = Mathf.RoundToInt(radii[e]);
                int start = Mathf.Max(0, center - r);
                int end = Mathf.Min(ArraySize - 1, center + r);
                for (int i = start; i <= end; i++)
                {
                    coverage[i]++;
                    coverage[i] = Mathf.Clamp(coverage[i], 0, 5);
                }
            }
            Map = coverage;

            // 압축 및 렌더링
            List<(int Value, int Length)> compressed = CompressMapWithTolerance(coverage, ValidDistance);

            // 기존에 활성화된 오브젝트들은 전부 풀로 반환
            foreach (SuccessRange obj in SuccessRangePool)
            {
                if (obj != null && obj.gameObject.activeSelf)
                    PoolManager.TakeToPool<SuccessRange>(obj);
            }
            SuccessRangePool.Clear();

            int Pos = -SpawnUI_Length / 2;
            for (int i = 0; i < compressed.Count; i++)
            {
                int val = Mathf.Min(compressed[i].Value, SuccessMats.Length);
                int length = compressed[i].Length;

                if (val > 0)
                {
                    if (length < ValidDistance && val > 1) val -= 1;
                    else if (length < ValidDistance) { Pos += length; continue; }

                    SuccessRange sr = PoolManager.GetFromPool<SuccessRange>();
                    if (sr == null) continue;
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
                        int matIndex = Mathf.Clamp(val - 1, 0, SuccessMats.Length - 1);
                        Material baseMat = SuccessMats[matIndex];

                        img.material = GameObject.Instantiate(baseMat);
                        images.Add(img);
                    }
                }
                Pos += length;
            }

            foreach (Image img in images)
            {
                img.material.SetFloat("_Size", img.GetComponent<RectTransform>().sizeDelta.x);
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

    // 각 이벤트별 반경 확장 코루틴 (독립 실행, radii와 finished는 외부에서 전달)
    private IEnumerator ExpandEventCoroutine(int index, float startDelay, float targetRadius, float duration, float[] radii, bool[] finished, int pos)
    {
        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);
        
        SpawnPS ps = PoolManager.GetFromPool<SpawnPS>();
        ps.GetComponent<RectTransform>().anchoredPosition = new Vector3(pos -500, -25, -12);
        ps.GetComponent<ParticleSystem>().Emit(1);
        float t = 0f;
        float from = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            // 부드러운 확장 (원하면 Mathf.Lerp 사용)
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


    static int minDistance = 20;
    public static List<int> GenerateEventPositions(float lambda)
    {
        List<int> eventPositions = new List<int>();

        const int MAX_RETRIES = 100;

        int initialEventPos = Random.Range(0, MaxRange + 1);
        eventPositions.Add(initialEventPos);

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

                foreach (int existingPos in eventPositions)
                {
                    if (Mathf.Abs(newPos - existingPos) < minDistance)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                if (!isTooClose)
                {
                    eventPositions.Add(newPos);
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
}
