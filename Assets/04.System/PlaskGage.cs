using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaskGage : MonoBehaviour
{
    public Image Fill;
    public TextMeshProUGUI PercentageText;
    public float Percentage = 0;
    private float plaskPoint;
    public float PlaskPoint
    {
        get
        {
            return plaskPoint;
        }
        set
        {
            plaskPoint = value;
            DataManager.SetFloatData("PlaskPoint", plaskPoint);
            Percentage = value / Plask.Instance.RequiredPlaskPoint * 100;
            Fill.fillAmount = Percentage / 100;
            PercentageText.text = (int)(Percentage) + "%";
            if (Percentage >= 100)
            {
                UpgradePlask();
            }
        }
    }

    void UpgradePlask()
    {
        float Overage = Percentage - 100 * Plask.Instance.RequiredPlaskPoint / 100;
        UpgradeManager.Instance.UpgradePlask();
        Percentage = 0;
        PlaskPoint = Overage;
    }
}
