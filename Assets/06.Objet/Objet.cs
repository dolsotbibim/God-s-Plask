using UnityEngine;

public abstract class Objet : MonoBehaviour
{
    public int Tier;
    public Sprite icon;

    public string Name;
    public string Description;

    public abstract void OnEquip();
    public abstract void Init();

    private void OnEnable()
    {
        Init();
    }
}