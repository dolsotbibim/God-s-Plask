using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class PlaskGage : MonoBehaviour
{
    public Image Fill;
    public TextMeshProUGUI PercentageText;
    public float Percentage = 0;
    public RectTransform ParticleSystem;
    public RectTransform Target;
    bool IsUpgrading = false;
    float temp = 0;
    private float plaskPoint;
    public float UpgradeDuration = 1f;
    Vector2 psStartPos;
    public Transform Floor;
    float previous;
    ParticleSystem ParticleComp;

    private void Awake()
    {
        psStartPos = ParticleSystem.anchoredPosition;
        ParticleComp = ParticleSystem.GetComponent<ParticleSystem>();
    }

    public float PlaskPoint
    {
        get { return plaskPoint; }
        set
        {
            plaskPoint = value;
            DataManager.SetFloatData("PlaskPoint", plaskPoint);
            Percentage = plaskPoint / Plask.Instance.RequiredPlaskPoint * 100f;

            if (!IsUpgrading)
            {
                float ratio = Mathf.Clamp01(plaskPoint / Plask.Instance.RequiredPlaskPoint);
                UpdateUIDotween(ratio, 0.1f);
            }

            if (Percentage >= 100f)
                StartCoroutine(UpgradePlask());
        }
    }

    public void AddPoint(float point)
    {
        if (!IsUpgrading)
            PlaskPoint += point;
        else
            temp += point;
    }

    IEnumerator UpgradePlask()
    {
        if (IsUpgrading) yield break;
        IsUpgrading = true;

        float required = Plask.Instance.RequiredPlaskPoint;
        float overage = (Percentage - 100f) / 100f * required;

        UpgradeEffectPS ps = Spawner.Instance.PoolManager.GetFromPool<UpgradeEffectPS>();

        float duration = UpgradeDuration;
        float startPoint = required;

        yield return StartCoroutine(ParticleRoutine());

        Percentage = 0f;
        yield return new WaitForSeconds(1.5f);
        UpgradeManager.Instance.UpgradePlask();
        Spawner.Instance.PoolManager.TakeToPool<UpgradeEffectPS>(ps);

        IsUpgrading = false;
        float finalRemainingPoint = overage + temp;
        PlaskPoint = finalRemainingPoint;
        temp = 0;

        IEnumerator ParticleRoutine()
        {
            int x = 0;

            while (x < 100)
            {
                plaskPoint = required / 100 * (100 - x);
                UpdateUIDotween((100 - x) / 100f, 0f);
                Percentage = 100 - x;
                DataManager.SetFloatData("PlaskPoint", plaskPoint);
                ps.SetTarget(0, Target.position, (Plask.Instance.NextScale - Plask.Instance.CurrentScale) / 100);
                ps.Emit(1);
                x++;
                yield return new WaitForSeconds(duration / 100);
            }
        }
    }

    void UpdateUIDotween(float ratio, float duration)
    {
        ratio = Mathf.Clamp01(ratio);

        Fill.materialForRendering.DOFloat(ratio, "_Top", duration);
        PercentageText.text = (int)(ratio * 100) + "%";

        float targetX = (ratio * GetComponent<RectTransform>().sizeDelta.x - 5) / 2;
        Target.anchoredPosition = new Vector3(ratio * GetComponent<RectTransform>().sizeDelta.x, 0, 0);
        ParticleSystem.DOAnchorPosX(targetX, duration);

        float targetScaleX = 1000f * ratio;

        DOTween.To(() => ParticleComp.shape.scale.x, x =>
        {
            ShapeModule shapeModule = ParticleComp.shape;
            shapeModule.scale = new Vector3(x, 20, 1);
        }, targetScaleX, duration);

        float targetRate = ratio * 5;

        DOTween.To(() => ParticleComp.emission.rateOverTime.constant, x =>
        {
            EmissionModule emissionModule = ParticleComp.emission;
            emissionModule.rateOverTime = x;
        }, targetRate, duration);
    }
}