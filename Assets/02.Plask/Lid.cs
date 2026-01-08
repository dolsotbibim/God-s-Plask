using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lid : MonoBehaviour
{
    public static Lid Instance;
    public List<GameObject> Objects = new List<GameObject>();
    public Material mat;
    private void OnEnable()
    {
        Instance = this;
    }
    private void Update()
    {
        if (Objects.Count > 0)
        {
            foreach (GameObject obj in Objects)
            {
                if (obj.activeSelf && obj.GetComponent<Object>().PreventingChange == false)
                    obj.GetComponent<Object>().LidTime += 1;
            }
            if(ColorRoutine == null) ColorRoutine = StartCoroutine(SetLidColor());
        }
    }
    Coroutine ColorRoutine = null;
    IEnumerator SetLidColor()
    {
        mat.SetColor("_Color", Color.red);
        yield return new WaitForSeconds(0.2f);
        mat.SetColor("_Color", Color.white);
        ColorRoutine = null;
    }

    private void Start()
    {
        mat.SetColor("_Color", Color.white);
    }

    public void UpdateLidObjects()
    {
        for (int i = 0; i < Objects.Count; i++)
        {
            GameObject obj = Objects[i];
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
            if (Objects.Contains(other.gameObject))
            {
                other.GetComponent<Object>().LidTime = 0;
                Objects.Remove(other.gameObject);
            }
        }
    }
}
