using System.Collections;
using UnityEngine;

public class SuccessEffect : MonoBehaviour
{
    public ParticleSystem RingPS;
    public ParticleSystem FlashPS;
    public ParticleSystem ParticlePS;

    ParticleSystemRenderer RingPSRenderer;
    ParticleSystemRenderer FlashPSRenderer;
    ParticleSystemRenderer ParticlePSRenderer;

    RectTransform RingRect;
    RectTransform FlashRect;
    RectTransform ParticleRect;

    ParticleSystem.MainModule RingMainModule;
    ParticleSystem.MainModule FlashMainModule;
    ParticleSystem.MainModule ParticleMainModule;

    public static Color[] colors =
    {
        Color.red * 2.16f,      
        Color.yellow * 1.50f,   
        Color.green * 1.5f,
        new Color(0f, 0.3f, 1f) * 3.40f,     
        Color.magenta * 2.5f
    };
    private void Awake()
    {
        RingRect = RingPS.GetComponent<RectTransform>();
        FlashRect = FlashPS.GetComponent<RectTransform>();
        ParticleRect = ParticlePS.GetComponent<RectTransform>();

        RingPSRenderer = RingPS.GetComponent<ParticleSystemRenderer>();
        FlashPSRenderer = FlashPS.GetComponent<ParticleSystemRenderer>();
        ParticlePSRenderer = ParticlePS.GetComponent<ParticleSystemRenderer>();

        RingMainModule = RingPS.main;
        FlashMainModule = FlashPS.main;
        ParticleMainModule = ParticlePS.main;
    }
    public float SizeFactor;
    public float SizeFactor1;
    public float SizeFactor2;
    private void Update()
    {
        RingMainModule.startSize = SizeFactor;
        FlashMainModule.startSize = SizeFactor1;
        ParticleMainModule.startSize = SizeFactor2;
    }
    public void StartEffect(int SuccessLevel, float Pos)
    {
        if (SuccessLevel == 0) return;
        if (SuccessLevel >= 11)
        {
            RingPSRenderer.material.color = Color.white * 2;
            FlashPSRenderer.material.color = Color.white * 2;
            ParticlePSRenderer.material.color = Color.white * 2;
        }
        else
        {
            RingPSRenderer.material.color = colors[(SuccessLevel - 1) % 5];
            FlashPSRenderer.material.color = colors[(SuccessLevel - 1) % 5] * 2;
            ParticlePSRenderer.material.color = colors[(SuccessLevel - 1) % 5] * 2;
        }
        RingRect.anchoredPosition = new Vector3(Pos, 0, 0);
        FlashRect.anchoredPosition = new Vector3(Pos, 0, 0);
        ParticleRect.anchoredPosition = new Vector3(Pos, 0, 0);

        if (SuccessLevel == 1) StartCoroutine(Success1());
        else if(SuccessLevel == 2) StartCoroutine(Success2());
        else if (SuccessLevel == 3) StartCoroutine(Success3());
        else if (SuccessLevel == 4) StartCoroutine(Success4());
        else if (SuccessLevel >= 5) StartCoroutine(Success5());
    }

    IEnumerator Success1()
    {
        RingPS.Emit(1);
        yield break;
    }

    IEnumerator Success2()
    {
        RingPS.Emit(1);
        yield return new WaitForSeconds(0.05f);
        RingPS.Emit(1);
    }

    IEnumerator Success3()
    {
        RingPS.Emit(1);
        FlashPS.Emit(1);
        yield return new WaitForSeconds(0.05f);
        RingPS.Emit(1);
    }

    IEnumerator Success4()
    {
        RingPS.Emit(1);
        ParticlePS.Emit(5);

        FlashPS.Emit(1);
        yield return new WaitForSeconds(0.05f);
        RingPS.Emit(1);
    }

    IEnumerator Success5()
    {
        RingPS.Emit(1);
        ParticlePS.Emit(15);
        FlashPS.Emit(1);
        yield return new WaitForSeconds(0.05f);
        RingPS.Emit(1);
    }
}
