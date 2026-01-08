using UnityEngine;

public abstract class Objet : MonoBehaviour
{
    public int Cost;
    public Sprite icon;

    public string Name;
    public string Description;
    public string Type;

    public int level;
    public int maxLevel;

    public abstract void OnEquip();
    public abstract void Init();

    private void OnEnable()
    {
        Init();
    }
}