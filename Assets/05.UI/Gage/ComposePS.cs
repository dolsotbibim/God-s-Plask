using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class DecomposePS : MonoBehaviour
{
    public ParticleSystem ps;
    Particle[] particles;
    public RectTransform target;
    public float pointPerParticle;
    public float targetZDepth = 14f;
    Color[] colors =
    {
        new Color(131 / 255f, 191 / 255f, 0) * 3.4f,
        new Color(44 / 255f, 0 / 255f, 191 / 255f) * 10.41f
    };
    RectTransform[] Targets = new RectTransform[2];

    int setTarget = -1;
    public void SetTarget(int i, Vector3 pos, float scale, float point)
    {
        ps = GetComponent<ParticleSystem>();

        transform.position = pos;
        pointPerParticle = point;
        ShapeModule shapeModule = ps.shape;
        shapeModule.radius = scale;
        if (setTarget == i) return;
        setTarget = i;
        Targets[0] = UpgradeManager.Instance.PlaskGage.Target;
        Targets[1] = ObjetManager.Instance.CosmicGage.Target;
        ps.GetComponent<ParticleSystemRenderer>().material.color = colors[i];
        target = Targets[i];
        ps.collision.SetPlane(0, target);
    }

    void OnParticleCollision(GameObject other)
    {
        if(setTarget == 0)
            UpgradeManager.Instance.PlaskGage.AddPoint(pointPerParticle);
        if(setTarget == 1)
            ObjetManager.Instance.CosmicGage.AddPoint(pointPerParticle);
    }

    public void StartEmit(int emit)
    {
        ps.Emit(1);
        for (int i = 0; i < emit - 1; i++)
        {
            StartCoroutine(Emit());
        }
    }

    public IEnumerator Emit()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.2f));
        ps.Emit(1);
    }
    public float SizeFactor;
    void LateUpdate()
    {
        ParticleSystem.MainModule psMain = ps.main;
        psMain.startSize = SizeFactor * Camera.main.orthographicSize;

        if (particles == null || particles.Length < ps.main.maxParticles)
        {
            particles = new Particle[ps.main.maxParticles];
        }

        int numParticles = ps.GetParticles(particles);

        if(numParticles <= 0)
        {
            Spawner.Instance.PoolManager.TakeToPool<DecomposePS>(this);
        }

        Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);
        Vector3 targetWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(
            screenPoint.x,
            screenPoint.y,
            targetZDepth
        ));

        for (int i = 0; i < numParticles; i++)
        {
            Particle p = particles[i];
            float elapsedTime = p.startLifetime - p.remainingLifetime;

            if (elapsedTime > 0.5f)
            {
                float t = (elapsedTime - 0.5f) / 3f;
                t = Mathf.Clamp01(t);
                p.position = Vector3.Lerp(p.position, targetWorldPosition, t);
            }
            particles[i] = p;
        }

        ps.SetParticles(particles, numParticles);
    }
}