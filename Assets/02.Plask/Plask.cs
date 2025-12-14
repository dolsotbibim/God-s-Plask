using System.Linq;
using UnityEngine;

public class Plask : MonoBehaviour
{
    public float CurrentScale;
    public float NextScale;
    private int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            CurrentScale = 0.175f + Mathf.Pow(1.25f, level - 1) * level * 1;
            NextScale = 0.175f + Mathf.Pow(1.25f, level) * (level + 1) * 1;
            DataManager.SetIntData("PlaskLevel", level);
            RequiredPlaskPoint = RequiredPlaskPointBase * Mathf.Pow(2f, level - 1);
            transform.localScale = new Vector3(1.125f, 1f, 1.125f) * CurrentScale;
            Camera.main.GetComponent<CameraMove2>().size = CurrentScale;
        }
    }

    public static Plask Instance;
    public Transform Under;
    public float RequiredPlaskPointBase = 50;
    public float RequiredPlaskPoint = 50;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
    }

    public void UpdateScale(float value)
    {
        transform.localScale += new Vector3(1.125f, 1f, 1.125f) * value;
        Camera.main.GetComponent<CameraMove2>().size = transform.localScale.x;
    }
}
