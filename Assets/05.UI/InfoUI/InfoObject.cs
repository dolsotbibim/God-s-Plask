using UnityEngine;

public class InfoObject : MonoBehaviour
{
    public MaterialArrayData Mats;
    private int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
            int pairIndex = level;
            MeshMaterialPair currentPair = Mats.pairs[pairIndex];
            Renderer renderer = GetComponent<Renderer>();
            MeshFilter filter = GetComponent<MeshFilter>();
            if (currentPair.mesh != null)
            {
                renderer.materials = currentPair.materials;
                filter.mesh = currentPair.mesh;
            }
        }
    }
}
