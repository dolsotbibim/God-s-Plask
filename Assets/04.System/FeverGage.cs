using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class FeverGage : MonoBehaviour
{
    public int MaxFeverStack = 10;
    public bool IsFever => feverstack >= MaxFeverStack;
    float targetFillAmount;
    public float FeverStack
    {
        get { return feverstack; }
        set
        {
            if (targetFillAmount >= 1) feverstack = value - MaxFeverStack;
            else feverstack = value;
            targetFillAmount = feverstack / MaxFeverStack;

            FeverFill.material.SetFloat("_Fill", targetFillAmount);
            FeverFill.DOFillAmount(targetFillAmount, 0.25f)
                    .SetEase(Ease.OutQuad);

            if(targetFillAmount >= 1)
            {
                FeverFill.material.SetFloat("_Alpha", 2);
                Spawner.Instance.SpawnPanel.isFever = true;

            }
            else
            {
                FeverFill.material.SetFloat("_Alpha", 1);
            }

        }
    }

    public static FeverGage Instance;

    private void Awake()
    {
        Instance = this;
        FeverStack = 0;
    }

    private float feverstack;

    public int FeverBonus = 2;
    public int FeverSpeed = 2;
    public int FeverRambda = 2;

    public Image FeverFill;
}
