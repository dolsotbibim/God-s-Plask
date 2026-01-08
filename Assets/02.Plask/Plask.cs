using UnityEngine;

public class Flask : MonoBehaviour
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
            if (BlackHole.instance.gameObject.activeSelf) return;

            Spawner.Instance.SpawnLevel = level;
            DataManager.SetIntData("FlaskLevel", level);
            RequiredFlaskPoint = RequiredFlaskPointBase * Mathf.Pow(1.20575f, level + 1);
            Spawner.Instance.SetSpawnLevel();

            if (level < 11)
            {
                CurrentScale = 0.175f + Mathf.Pow(1.125f, level - 1);
                NextScale = 0.175f + Mathf.Pow(1.125f, level);
                transform.localScale = new Vector3(1.125f, 1f, 1.125f) * CurrentScale;
                Camera.main.GetComponent<CameraMove2>().size = CurrentScale;

            }
            else
            {
                CurrentScale = 0.175f + Mathf.Pow(1.125f, 9);
                NextScale = 0.175f + Mathf.Pow(1.125f, 10);
                transform.localScale = new Vector3(1.125f, 1f, 1.125f) * CurrentScale;
                Camera.main.GetComponent<CameraMove2>().size = CurrentScale;

                Spawner.Instance.transform.localScale = Spawner.Instance.NextScale * Vector3.one;
            }

        }
    }
    public static Flask Instance;
    public Transform Under;
    public float RequiredFlaskPointBase = 50;
    public float RequiredFlaskPoint = 50;

    private void Awake()
    {
        Instance = this;
        Color baseColor = Color.HSVToRGB(Mathf.Repeat(colorValue, 1f), 150 / 255f, 150 / 255f);
        UIManager.Instance.SkyboxMaterial.SetColor("_Tint", baseColor);
    }

    private void Update()
    {
    }
    public float colorValue = 0;
    int count = 0;
    public void UpdateScale(float value)
    {
        colorValue += 0.0001f;
        if (BlackHole.instance.gameObject.activeSelf) return;
        count++;
        if (count % 33 == 0)
            SoundManager.Instance.PlaySFX(5, 0.15f, false);

        Color baseColor = Color.HSVToRGB(Mathf.Repeat(colorValue, 1f), 150 / 255f, 150 / 255f);
        UIManager.Instance.SkyboxMaterial.SetColor("_Tint", baseColor);
        if(Level < 10)
        {
            transform.localScale += new Vector3(1.125f, 1f, 1.125f) * value;
            Camera.main.GetComponent<CameraMove2>().size = transform.localScale.x;
        }
        else
        {
            Spawner.Instance.SetScale((NextScale - CurrentScale) / 100f);
        }
        
    }
}
