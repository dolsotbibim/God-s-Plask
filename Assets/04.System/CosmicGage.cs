using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CosmicGage : MonoBehaviour
{
    public Image Fill;
    public TextMeshProUGUI PercentageText;
    public float Percentage = 0;
    private float cosmicPoint;
    public float CosmicPoint
    {
        get
        {
            return cosmicPoint;
        }
        set
        {
            cosmicPoint = value;
            DataManager.SetFloatData("CosmicPoint", cosmicPoint);
            Percentage = value / ObjetManager.Instance.RequiredCosmicPoint * 100;
            Fill.fillAmount = Percentage / 100;
            PercentageText.text = (int)(Percentage) + "%";
            if (Percentage >= 100)
            {
                GetObjet();
            }
        }
    }

    void GetObjet()
    {
        float Overage = Percentage - 100 * ObjetManager.Instance.RequiredCosmicPoint / 100;
        CosmicPoint = Overage;
        ObjetManager.Instance.GetObjet();
    }
}
