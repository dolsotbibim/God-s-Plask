using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class CosmicGage : MonoBehaviour
{
    public Image Fill;
    public TextMeshProUGUI PercentageText;
    public float Percentage = 0;
    public RectTransform ParticleSystem;
    public RectTransform Target;
    private float cosmicPoint;

    public float UpgradeDuration = 0.5f;
    private Vector2 psStartPos;

    private bool IsGettingObjet = false;
    private float temp = 0;
    ParticleSystem psComp;

    private void Awake()
    {
        psStartPos = ParticleSystem.anchoredPosition;
        psComp = ParticleSystem.GetComponent<ParticleSystem>();
    }

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

            Percentage = value / ObjetManager.Instance.RequiredCosmicPoint * 100f;

            if (!IsGettingObjet)
            {
                UpdateUIDotween(Percentage / 100f, 0.1f);
            }

            if (Percentage >= 100f)
            {
                StartCoroutine(GetObjetCoroutine());
            }
        }
    }

    public void AddPoint(float point)
    {
        if (!IsGettingObjet)
            CosmicPoint += point;
        else
            temp += point;
    }

    private void UpdateUIDotween(float ratio, float duration)
    {
        ratio = Mathf.Clamp01(ratio);

        PercentageText.text = (int)(ratio * 100) + "%";
        Fill.materialForRendering.DOFloat(ratio, "_Top", duration);

        float targetX = (ratio * GetComponent<RectTransform>().sizeDelta.x - 5) / 2f;

        ParticleSystem.DOAnchorPosX(targetX, duration);

        float targetScaleX = 300f * ratio;

        DOTween.To(() => psComp.shape.scale.x, x =>
        {
            ShapeModule shapeModule = psComp.shape;
            shapeModule.scale = new Vector3(x, 40, 1);
        }, targetScaleX, duration);

        float targetRate = ratio * 5;

        DOTween.To(() => psComp.emission.rateOverTime.constant, x =>
        {
            EmissionModule emissionModule = psComp.emission;
            emissionModule.rateOverTime = x;
        }, targetRate, duration);
    }

    IEnumerator GetObjetCoroutine()
    {
        if (IsGettingObjet) yield break;
        IsGettingObjet = true;

        DoTweenStartEffect();

        float required = ObjetManager.Instance.RequiredCosmicPoint;
        float overageRatio = (Percentage - 100f) / 100f;
        float overagePoint = overageRatio * required;

        float finalRemainingPoint = overagePoint + temp;

        float duration = UpgradeDuration;
        float startPoint = cosmicPoint;

        yield return DOTween.To(() => startPoint, x =>
        {
            cosmicPoint = x;
            UpdateUIDotween(cosmicPoint / required, 0f);
            Percentage = cosmicPoint / required * 100f;
        }, finalRemainingPoint, duration)
        .SetEase(Ease.Linear)
        .WaitForCompletion();

        ObjetManager.Instance.GetObjet();
        DoTweenEndEffect();

        temp = 0;
        CosmicPoint = finalRemainingPoint;

        IsGettingObjet = false;
    }

    void DoTweenStartEffect()
    {
        ParticleSystem.DOAnchorPos(psStartPos, 0.4f).SetEase(Ease.OutBack);
    }

    void DoTweenEndEffect()
    {
        ParticleSystem.DOAnchorPos(psStartPos, 0.4f).SetEase(Ease.InOutQuad);
    }
}