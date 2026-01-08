using DG.Tweening;
using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Random = UnityEngine.Random;
public class Spawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform spawnPoint;

    public static Spawner Instance;
    public PoolManager PoolManager;

    public int SpawnLevel = 1;
    public float RoundTripTime = 5;
    public float DefaultRoundTripTime = 5;
    public float CosmicProbability = 0.1f;
    public SpawnPanel SpawnPanel;
    public int FeverStackAmount = 1;
    public ObservableCollection<Object> objs = new ObservableCollection<Object>();
    public int CurrentMaxObjectLevel;

    List<(int, float)> SpawnTable = new List<(int, float)>()
    {
        (1, 1f),
        (2, 0f),
    };
    public SuccessEffect successEffect;

    public int ComboStack;
    private void OnEnable()
    {
        Instance = this;
    }
    public void OnInteract()
    {
        //SpawnObjects(30);
    }
    private void Awake()
    {
        objs.CollectionChanged += OnObjectsChanged;
    }

    private void OnObjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (objs.Count == 0)
        {
            CurrentMaxObjectLevel = 0;
            return;
        }

        CurrentMaxObjectLevel = objs.Max(item => item.Level);
    }
    public float CurrentScale = 1f;
    public float NextScale = 1f;
    public void SetScale(float value)
    {
        transform.localScale += Vector3.one * (NextScale - CurrentScale) / 100f;
    }

    public void SetSpawnLevel()
    {
        int SpawnValue = (int)((SpawnLevel - 1) * 20 + EnhancedSpawnChance * 100) / 100;
        int Remainder = (int)((SpawnLevel - 1) * 20 + EnhancedSpawnChance * 100) % 100;
        SpawnTable[0] = (SpawnValue, 100 - Remainder);
        SpawnTable[1] = (SpawnValue + 1, Remainder);

        UIManager.Instance.SetInfoObject(SpawnTable[0], SpawnTable[1]);

        CurrentScale = 3 * Mathf.Pow(0.875f, Flask.Instance.Level - 10);
        NextScale = CurrentScale * 0.875f;
    }
    public float BonusSpawnChance = 0f;
    public int BonusSpawnAmount = 1;
    public float EnhancedSpawnChance = 0f;
    public int SpawnedObjectCount = 0;
    public int MergedObjectCount = 0;
    public int FailCount = 0;
    public int SuccessCount = 0;
    public void OnSpawn(InputAction.CallbackContext context)
    {
        if(context.started && !SpawnPanel.IsStuck && !SpawnPanel.IsWaiting && !BlackHole.instance.gameObject.activeSelf)
        {
            
            int value = SpawnPanel.CheckHand();
            
            float HandPos = SpawnPanel.Hand.GetComponent<RectTransform>().anchoredPosition.x;
            
            if (value == 0)
            {
                
                ComboStack = 0;
                RoundTripTime = DefaultRoundTripTime;
                SpawnPanel.IsStuck = true;
                FailCount += 1;
                DataManager.SetIntData("FailCount", FailCount);
                return;
            }
            else
            {
                if (Random.value < BonusSpawnChance)
                {
                    value += BonusSpawnAmount;
                    successEffect.StartEffect(value - BonusSpawnAmount, HandPos);
                }
                else
                {
                    successEffect.StartEffect(value, HandPos);
                }
                    SuccessCount += 1;
                DataManager.SetIntData("SuccessCount", SuccessCount);
                SpawnedObjectCount += value;
                DataManager.SetIntData("SpawnedObjectCount", SpawnedObjectCount);
                if(value < 3)
                    SoundManager.Instance.SuccessSFX(1, 1);
                else if(value < 5)
                    SoundManager.Instance.SuccessSFX(2, 1);
                else
                    SoundManager.Instance.SuccessSFX(3, 1);
                if (FeverGage.Instance.IsFever)
                    SpawnObjects((int)(value * FeverGage.Instance.FeverBonus));
                else
                    SpawnObjects(value);
                ComboStack += 1;
                if (ComboStack <= 4)
                    RoundTripTime = DefaultRoundTripTime * Mathf.Pow(0.9f, ComboStack);
                if (FeverGage.Instance.IsFever && Random.value < FeverGage.Instance.FiverRetriggerChance)
                {
                    FeverGage.Instance.FeverStack += FeverGage.Instance.MaxFeverStack;
                }
                else if(!UIManager.Instance.IsTutorial)
                    FeverGage.Instance.FeverStack += FeverStackAmount * value / 2.5f;
            }
            if (UIManager.Instance.TutorialStep < UIManager.Instance.Tutorials.Length - 1)
            {
                FeverGage.Instance.FeverStack += UIManager.Instance.TutorialStep % 5 + 1;
                UIManager.Instance.TutorialStep++;
            }
            else
            {
                for (int i = 0; i < UIManager.Instance.Tutorials.Length; i++)
                {
                    UIManager.Instance.Tutorials[i].SetActive(false);
                }
                UIManager.Instance.IsTutorial = false;
                DataManager.SetIntData("TutorialCleared", 1);
            }

            StartCoroutine(SpawnPanel.Restart());
        }
    }

    public void ResetCombo()
    {
        ComboStack = 0;
        RoundTripTime = DefaultRoundTripTime;
    }
    public void SpawnObjects(int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnObject();
        }
    }

    public void SpawnObject()
    {
        Object obj = Spawn(false);
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-0.25f, 0.25f), 0, Random.Range(-0.25f, 0.25f)), ForceMode.VelocityChange);
    }

    public IEnumerator SpawnMergedObject(int Level, Vector3 spawnPoint, bool isCosmic)
    {
        yield return new WaitForSeconds(0.25f);
        Object obj = Spawn(true, Level, spawnPoint);
        Physics.IgnoreCollision(obj.GetComponent<Collider>(), Lid.Instance.GetComponent<Collider>(), false);
    }

    Object Spawn(bool Merged, int Level = -1, Vector3? spawnpoint = null)
    {
        int level = Level;
        Vector3 Pos;
        if(spawnpoint.HasValue)
        {
            Pos = (Vector3)spawnpoint;
        }
        else
        {
            Pos = spawnPoint.position;
        }

        if (!Merged)
        {
            if(Random.value < SpawnTable[0].Item2 / 100f)
            {
                level = SpawnTable[0].Item1;
            }
            else
            {
                level = SpawnTable[1].Item1;
            }
        }
        else
        {
            MergedObjectCount += 1;
            DataManager.SetIntData("MergedObjectCount", MergedObjectCount);
        }

            Object obj = PoolManager.GetFromPool<Object>();
        obj.transform.position = Pos + new Vector3(
            Random.Range(-0.05f, 0.05f),
            0,
            Random.Range(-0.05f, 0.05f)
        );
        obj.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        obj.Merged = Merged;

        obj.Level = level;
        objs.Add(obj);

        return obj;
    }

    

    public void ReturnObject(Object obj)
    {
        Vector3 Scale = obj.transform.localScale;
        obj.transform.DOScale(Scale * 0.2f, 0.05f)
            .SetEase(Ease.OutQuart)

            .OnComplete(() => OnScaleComplete(obj));
    }



    void OnScaleComplete(Object obj)
    {
        if (obj.gameObject.activeSelf)
        {
            objs.Remove(obj);
            PoolManager.TakeToPool<Object>(obj);
        }
    }
    public void UnPauseObjects()
    {
        foreach (Object obj in objs)
        {
            obj.SetKinematic(false);
        }
    }
    public void PauseObjects()
    {
        foreach (Object obj in objs)
        {
            obj.SetKinematic(true);
        }
    }
}
