using System.Linq;
using UnityEngine;

public class Plask : MonoBehaviour
{
    private int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            DataManager.SetIntData("PlaskLevel", level);
            RequiredPlaskPoint = RequiredPlaskPointBase * Mathf.Pow(3f, level - 1);
            UpdateScale();
        }
    }

    public static Plask Instance;

    public float RequiredPlaskPointBase = 1000;
    public float RequiredPlaskPoint = 1000;

    private void Awake()
    {
        Instance = this;
    }

    private void UpdateScale()
    {
        float scaleFactor = 0.405f + (level - 1) * 0.2f;
        transform.localScale = new Vector3(1, 1f, 1f) * scaleFactor;
    }
}
