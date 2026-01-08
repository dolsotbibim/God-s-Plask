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
    public TextMeshProUGUI pointtext;

    private int point;
    public int Point
    {
        get { return point; }
        set
        {
            point = value;
            DataManager.SetIntData("CosmicOre", point);
            pointtext.fontMaterial.DOColor(Color.white * 2, "_FaceColor", 0.2f).OnComplete(() =>
            {
                pointtext.text = point.ToString();
                pointtext.fontMaterial.DOColor(Color.white, "_FaceColor", 0.2f);
            });
        }
    }

    private float UpgradeDuration = 0.4f;
    private Vector2 psStartPos;

    private bool IsGettingObjet = false;
    private float temp = 0;
    ParticleSystem psComp;

    private void Awake()
    {
        psStartPos = ParticleSystem.anchoredPosition;
        psComp = ParticleSystem.GetComponent<ParticleSystem>();

    }
    public int CollectedCosmicOre = 0;
    public float CosmicPoint
    {
        get
        {
            return cosmicPoint;
        }
        set
        {
            ObjetManager.Instance.RequiredCosmicPoint = 10 * Mathf.Pow(1.12f, CollectedCosmicOre);

            cosmicPoint = value;
            DataManager.SetFloatData("CosmicPoint", cosmicPoint);

            Percentage = value / ObjetManager.Instance.RequiredCosmicPoint * 100f;

            if (!IsGettingObjet)
            {
                UpdateUIDotween(Percentage / 100f, 0.1f);
            }

            if (Percentage >= 100f)
            {
                CollectedCosmicOre += 1;
                DataManager.SetIntData("CosmicOreCount", CollectedCosmicOre);
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
        Target.anchoredPosition = new Vector3(ratio * GetComponent<RectTransform>().sizeDelta.x, 0, 0);

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
    public float DoubleGetChance = 0f;
    IEnumerator GetObjetCoroutine()
    {
        if (IsGettingObjet) yield break;
        IsGettingObjet = true;

        float required = ObjetManager.Instance.RequiredCosmicPoint;
        float overageRatio = (Percentage - 100f) / 100f;
        float overagePoint = overageRatio * required;

        float duration = UpgradeDuration;
        float startPoint = ObjetManager.Instance.RequiredCosmicPoint;
        UpgradeEffectPS ps = Spawner.Instance.PoolManager.GetFromPool<UpgradeEffectPS>();
        yield return StartCoroutine(ParticleRoutine());
        Spawner.Instance.PoolManager.TakeToPool<UpgradeEffectPS>(ps);
        float finalRemainingPoint = overagePoint + temp;

        temp = 0;
        IsGettingObjet = false;

        CosmicPoint = finalRemainingPoint;

        IEnumerator ParticleRoutine()
        {
            int x = 1;

            while (x < 21)
            {
                cosmicPoint = required / 100 * (100 - x * 5);
                UpdateUIDotween(cosmicPoint / required, 0f);
                Percentage = 100 - x * 5;
                DataManager.SetFloatData("CosmicPoint", cosmicPoint);
                ps.SetTarget(1, Target.position, 0);
                ps.Emit(1);
                x++;
                yield return null;
            }
            yield return new WaitForSeconds(0.2f);
            if(Random.value < DoubleGetChance)
            {
                Point += 2;
            }
            else
            {
                Point += 1;
            }
            SoundManager.Instance.PlaySFX(6, 0.75f, false);

        }
    }
}