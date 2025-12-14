using DG.Tweening;
using Redcode.Pools;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public List<Object> objs = new List<Object>();

    public SuccessEffect successEffect;

    public int ComboStack;
    private void OnEnable()
    {
        Instance = this;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
            SpawnObjects(10);
    }

    public void OnSpawn(InputAction.CallbackContext context)
    {
        if(context.started && !SpawnPanel.IsStuck && !SpawnPanel.IsWaiting)
        {
            int value = SpawnPanel.CheckHand();
            float HandPos = SpawnPanel.Hand.GetComponent<RectTransform>().anchoredPosition.x;
            if (FeverGage.Instance.IsFever)
                SpawnObjects(value * FeverGage.Instance.FeverBonus);
            else
                SpawnObjects(value);
            if (value == 0)
            {
                ComboStack = 0;
                RoundTripTime = DefaultRoundTripTime;
                SpawnPanel.IsStuck = true;
            }
            else
            {
                successEffect.StartEffect(value, HandPos);

                ComboStack += 1;
                if (ComboStack <= 4)
                    RoundTripTime = DefaultRoundTripTime * Mathf.Pow(0.9f, ComboStack);
                FeverGage.Instance.FeverStack += FeverStackAmount * value / 2.5f;
                StartCoroutine(SpawnPanel.Restart());
            }
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
        int level = Level > 0 ? Level : SpawnLevel;
        Vector3 Pos;
        if(spawnpoint.HasValue)
        {
            Pos = (Vector3)spawnpoint;
        }
        else
        {
            Pos = spawnPoint.position;
        }
        Object obj = PoolManager.GetFromPool<Object>();
        obj.transform.position = Pos + new Vector3(
            Random.Range(-0.05f, 0.05f),
            0,
            Random.Range(-0.05f, 0.05f)
        );
        obj.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        obj.Level = level;
        objs.Add(obj);
        obj.Merged = Merged;

        return obj;
    }

    CosmicObject SpawnCosmic(bool Merged, int Level = -1, Vector3? spawnpoint = null)
    {
        int level = Level > 0 ? Level : SpawnLevel;
        Vector3 Pos;
        if (spawnpoint.HasValue)
        {
            Pos = (Vector3)spawnpoint;
        }
        else
        {
            Pos = spawnPoint.position;
        }
        CosmicObject obj = PoolManager.GetFromPool<CosmicObject>();
        obj.transform.position = Pos + new Vector3(
            Random.Range(-0.05f, 0.05f),
            0,
            Random.Range(-0.05f, 0.05f)
        );
        obj.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        obj.Level = level;
        objs.Add(obj);
        obj.Merged = Merged;
        return obj;
    }

    public void ReturnObject(Object obj)
    {
        Vector3 Scale = obj.transform.localScale;
        obj.transform.DOScale(Scale * 0.2f, 0.05f)
            .SetEase(Ease.OutQuart)

            .OnComplete(() => OnScaleComplete(obj));
    }

    public void ReturnObject(CosmicObject obj)
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

    void OnScaleComplete(CosmicObject obj)
    {
        if (obj.gameObject.activeSelf)
        {
            objs.Remove(obj);
            PoolManager.TakeToPool<CosmicObject>(obj);
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
