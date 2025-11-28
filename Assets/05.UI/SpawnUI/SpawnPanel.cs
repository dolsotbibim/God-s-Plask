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
    public int Rambda = 1;
    private bool isStuck = false;
    public bool isFever = false;
    public bool IsStuck
    {
        get { return isStuck; }
        set 
        { 
            isStuck = value;
            if(isStuck)
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
        HandTransform.anchoredPosition = new Vector3(0, HandTransform.anchoredPosition.y, 0);

        while (true)
        {
            if (isFever)
            {
                FeverGage.Instance.FeverStack = 0;
                UpdateSuccessRange(Rambda * FeverGage.Instance.FeverRambda);
                yield return FeverHandRoutine();
            }
            else
            {
                UpdateSuccessRange(Rambda);
                yield return MoveHandRoutine();
            }
        }
    }

    IEnumerator MoveHandRoutine()
    {
        float time = 0;
        float currentX = 0;
        totalTime = Spawner.Instance.RoundTripTime;

        while (time <= totalTime * 2)
        {
            while (StuckTime > 0)
            {
                StuckTime -= Time.deltaTime;
                yield return null;
            }
            _lastXPosition = currentX;

            IsStuck = false;
            time += Time.deltaTime;
            float pingPongValue = Mathf.PingPong(time, totalTime);
            currentX = Mathf.Lerp(-SpawnUI_Length / 2 + 1, SpawnUI_Length / 2 - 1, pingPongValue / totalTime);
            HandTransform.anchoredPosition = new Vector2(currentX, HandTransform.anchoredPosition.y);
            yield return null;
        }
    }

    IEnumerator FeverHandRoutine()
    {
        float currentX = 0;
        float time = 0;
        totalTime = Spawner.Instance.RoundTripTime / FeverGage.Instance.FeverSpeed;

        while (time <= totalTime * 2)
        {
            while (StuckTime > 0)
            {
                StuckTime -= Time.deltaTime;
                yield return null;
            }
            _lastXPosition = currentX;

            IsStuck = false;
            time += Time.deltaTime;
            float pingPongValue = Mathf.PingPong(time, totalTime);
            currentX = Mathf.Lerp(-SpawnUI_Length / 2 + 1, SpawnUI_Length / 2 - 1, pingPongValue / totalTime);
            HandTransform.anchoredPosition = new Vector2(currentX, HandTransform.anchoredPosition.y);
            yield return null;
        }
    }


    private bool CheckBoundaryHit(float currentX, float halfLength)
    {
        bool currentIsMovingRight = currentX > _lastXPosition;
        if (currentX - (-halfLength) <= SpawnUI_Length / totalTime * 60 + 5)
        {
            if (currentIsMovingRight != _isMovingRight)
            {
                _isMovingRight = currentIsMovingRight;
                if(_isMovingRight)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int CheckHand()
    {
        float HandPos = HandTransform.anchoredPosition.x;
        return Map[Mathf.RoundToInt(HandPos) + Map.Length / 2];
    }

    public void Restart()
    {
        _lastXPosition = -1000;
        _isMovingRight = true;
        StopCoroutine(SpawnPanelRoutine);
        SpawnPanelRoutine = StartCoroutine(MoveHand());
    }

    void UpdateSuccessRange(int Rambda)
    {
        Map = GetEventMap(GenerateEventPositions(Rambda), (int)SuccessRange);
        
        List<(int Value, int Length)> compressedData = CompressMap(Map);

        int Pos = -SpawnUI_Length / 2;
        int Center = SpawnUI_Length / 2;
        int Value = 0;

        foreach(SuccessRange obj in SuccessRangePool)
        {
            if(obj.gameObject.activeSelf)
                PoolManager.TakeToPool<SuccessRange>(obj);
        }

        for (int i = 0; i < compressedData.Count; i++)
        {
            Center = Pos + compressedData[i].Length / 2;
            Pos += compressedData[i].Length;
            Value = Mathf.Min(compressedData[i].Value, 5);

            if(Value > 0)
            {
                if (compressedData[i].Length < ValidDistance && Value > 1) Value -= 1;
                else if (compressedData[i].Length < ValidDistance) continue;

                SuccessRange SuccessRange = PoolManager.GetFromPool<SuccessRange>();
                if(!SuccessRangePool.Contains(SuccessRange)) SuccessRangePool.Add(SuccessRange);
                SuccessRange.transform.SetParent(SuccessRangeIndices[Value - 1].transform);
                RectTransform Rect = SuccessRange.GetComponent<RectTransform>();
                Rect.anchoredPosition = new Vector2(Center, Rect.anchoredPosition.y);
                Rect.sizeDelta = new Vector2(compressedData[i].Length + 1, Rect.sizeDelta.y);
                Rect.GetComponent<Image>().material = SuccessMats[Value - 1];
            }
        }
    }

    private const int ArraySize = 1001;
    private const int MaxRange = 1000;

    public static List<(int Value, int Length)> CompressMap(int[] map)
    {
        if (map == null || map.Length == 0)
        {
            return new List<(int Value, int Length)>();
        }

        List<(int Value, int Length)> compressedList = new List<(int Value, int Length)>();

        int currentValue = map[0];
        int currentLength = 1;

        for (int i = 1; i < map.Length; i++)
        {
            if (map[i] == currentValue)
            {
                currentLength++;
            }
            else
            {
                compressedList.Add((currentValue, currentLength));

                currentValue = map[i];
                currentLength = 1;
            }
        }

        compressedList.Add((currentValue, currentLength));

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
