using UnityEngine;

[CreateAssetMenu(fileName = "MaterialArrayData", menuName = "Custom Data/Material Array")]
public class MaterialArrayData : ScriptableObject
{
    public Material[] materials;
    public int MaterialCount
    {
        get { return materials.Length; }
    }
}