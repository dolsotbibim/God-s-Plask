using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lid : MonoBehaviour
{
    public static Lid Instance;
    public GameObject WarningSign;
    public List<GameObject> Objects = new List<GameObject>();
    private void OnEnable()
    {
        Instance = this;
    }
    float time = 0;
    private void Update()
    {
        if (Objects.Count > 0)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0;
        }

        if(time > 0.5f)
        {
            WarningSign.SetActive(true);
            Spawner.Instance.PauseObjects();
        }
        if (time > 1.5f)
        {
            foreach (GameObject obj in Objects)
            {
                obj.GetComponent<Object>().Decompose();
            }
            Spawner.Instance.UnPauseObjects();
            WarningSign.SetActive(false);
        }
        if(time > 0.5f && time <= 1.5f && Objects.Count == 0)
        {
            Spawner.Instance.UnPauseObjects();
            WarningSign.SetActive(false);
        }
    }

    public void UpdateLidObjects()
    {
        foreach (GameObject obj in Objects) {
            if (!obj.activeSelf) Objects.Remove(obj);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            if(!Objects.Contains(other.gameObject))
            {
                Objects.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.CompareTag("Object"))
        {
            if(Objects.Contains(other.gameObject))
            {
                Objects.Remove(other.gameObject);
            }
        }
    }
}
