using UnityEngine;
using Redcode.Pools;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;
public class Spawner : MonoBehaviour
{
    public GameObject objectPrefab;
    public Transform spawnPoint;

    public static Spawner Instance;
    public PoolManager PoolManager;

    public int SpawnLevel = 1;
    public float RoundTripTime = 10;
    public float CosmicProbability = 0.1f;
    public SpawnPanel SpawnPanel;
    public int FeverStackAmount = 1;
    public List<Object> objs = new List<Object>(); 
    private void OnEnable()
    {
        Instance = this;
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
            SpawnObject();
    }

    public void OnSpawn(InputAction.CallbackContext context)
    {
        if(context.started && !SpawnPanel.IsStuck)
        {
            int value = SpawnPanel.CheckHand();
            SpawnObjects(value);
            if(value == 0)
                SpawnPanel.IsStuck = true;
            else
            {
                FeverGage.Instance.FeverStack += FeverStackAmount;
                SpawnPanel.Restart();
            }
        }
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
        if (Random.value < CosmicProbability)
        {
            CosmicObject cosobj = SpawnCosmic();
            cosobj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-0.25f, 0.25f), 0, Random.Range(-0.25f, 0.25f)), ForceMode.VelocityChange);
            objs.Add(cosobj);
        }
        Object obj = Spawn();
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-0.25f, 0.25f), 0, Random.Range(-0.25f, 0.25f)), ForceMode.VelocityChange);
        objs.Add(obj);
    }

    public IEnumerator SpawnMergedObject(int Level, Transform spawnPoint, bool isCosmic)
    {
        yield return new WaitForSeconds(0.25f);
        if (isCosmic) 
            SpawnCosmic(Level, spawnPoint);
        else
            Spawn(Level, spawnPoint);
    }

    Object Spawn(int Level = -1, Transform spawnpoint = null)
    {
        int level = Level > 0 ? Level : SpawnLevel;
        Transform Pos = spawnpoint ? spawnpoint : this.spawnPoint;

        Object obj = PoolManager.GetFromPool<Object>();
        obj.transform.position = Pos.position;
        obj.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        obj.Level = level;
        objs.Add(obj);

        return obj;
    }

    CosmicObject SpawnCosmic(int Level = -1, Transform spawnpoint = null)
    {
        int level = Level > 0 ? Level : SpawnLevel;
        Transform Pos = spawnpoint ? spawnpoint : spawnPoint;

        CosmicObject obj = PoolManager.GetFromPool<CosmicObject>();
        obj.transform.position = Pos.position;
        obj.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
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
