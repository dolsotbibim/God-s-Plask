 using UnityEngine;
using UnityEngine.UI;

public class FeverGage : MonoBehaviour
{
    public int MaxFeverStack = 10;
    public int FeverStack
    {
        get { return feverstack; }
        set
        {
            feverstack = value;
            FeverFill.fillAmount = (float)feverstack / MaxFeverStack;
            Spawner.Instance.SpawnPanel.isFever = feverstack >= MaxFeverStack;
        }
    }

    public static FeverGage Instance;

    private void Awake()
    {
        Instance = this;
    }

    private int feverstack;

    public int FeverBonus = 2;
    public int FeverSpeed = 2;
    public int FeverRambda = 2;

    public Image FeverFill;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
