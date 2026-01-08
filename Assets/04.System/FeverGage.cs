using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class FeverGage : MonoBehaviour
{
    public TextMeshProUGUI FeverMultiplierText;
    public int MaxFeverStack = 10;
    public bool IsFever => feverstack >= MaxFeverStack;
    float targetFillAmount;
    public float FiverRetriggerChance = 0f;
    public float FeverStack
    {
        get { return feverstack; }
        set
        {
            if (targetFillAmount >= 1) feverstack = value - MaxFeverStack;
            else feverstack = value;
            DataManager.SetFloatData("FeverStack", feverstack);
            targetFillAmount = Mathf.Clamp01(feverstack / MaxFeverStack);
            FeverFill.material.SetFloat("_Fill", targetFillAmount);
            FeverFill.DOFillAmount(targetFillAmount, 0.25f)
                    .SetEase(Ease.OutQuad);
            if (targetFillAmount >= 1)
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
    public float feverBonus = 2;
    public float FeverBonus
    {
        get { return feverBonus; }
        set
        {
            feverBonus = value;
            //FeverMultiplierText.text = "X" + feverBonus.ToString();
        }
    }
    public int FeverSpeed = 2;
    public float FeverRambda = 2;

    public Image FeverFill;
}
