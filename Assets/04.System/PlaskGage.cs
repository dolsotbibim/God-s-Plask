using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class FlaskGage : MonoBehaviour
{
    public Image Fill;
    public TextMeshProUGUI PercentageText;
    float Percentage = 0;
    public RectTransform ParticleSystem;
    public RectTransform Target;
    bool IsUpgrading = false;
    float temp = 0;
    private float flaskPoint;
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

    public float FlaskPoint
    {
        get { return flaskPoint; }
        set
        {
            flaskPoint = value;
            SoundManager.Instance.PlayPitchSFX(4, Percentage / 100 - 0.5f, 1f);

            DataManager.SetFloatData("FlaskPoint", flaskPoint);
            Percentage = flaskPoint / Flask.Instance.RequiredFlaskPoint * 100f;
            if (!IsUpgrading)
            {
                float ratio = Mathf.Clamp01(flaskPoint / Flask.Instance.RequiredFlaskPoint);
                UpdateUIDotween(ratio, 0.1f);
            }

            if (Percentage >= 100f)
                StartCoroutine(UpgradeFlask());
        }
    }

    public void AddPoint(float point)
    {
        if (!IsUpgrading)
            FlaskPoint += point;
        else
            temp += point;
    }

    IEnumerator UpgradeFlask()
    {
        if (IsUpgrading) yield break;
        IsUpgrading = true;

        float required = Flask.Instance.RequiredFlaskPoint;
        float overage = (Percentage - 100f) / 100f * required;
        UpgradeEffectPS ps = Spawner.Instance.PoolManager.GetFromPool<UpgradeEffectPS>();

        float duration = UpgradeDuration;
        float startPoint = required;

        yield return StartCoroutine(ParticleRoutine());

        Percentage = 0f;
        yield return new WaitForSeconds(2f);
        UpgradeManager.Instance.UpgradeFlask();
        Spawner.Instance.PoolManager.TakeToPool<UpgradeEffectPS>(ps);

        IsUpgrading = false;
        float finalRemainingPoint = overage + temp;
        FlaskPoint = finalRemainingPoint;
        temp = 0;

        IEnumerator ParticleRoutine()
        {
            int x = 0;
            SoundManager.Instance.PlaySFX(5, 0.25f, false);
            while (x < 50)
            {
                flaskPoint = required / 100 * (100 - x * 2);
                UpdateUIDotween((100 - x * 2) / 100f, 0f);
                Percentage = 100 - x * 2;
                DataManager.SetFloatData("FlaskPoint", flaskPoint);
                ps.SetTarget(0, Target.position, (Flask.Instance.NextScale - Flask.Instance.CurrentScale) / 100f * 2);
                ps.Emit(2);
                x++;
                yield return null;
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