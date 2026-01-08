using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class UpgradeEffectPS : MonoBehaviour
{
    public ParticleSystem ps;
    Particle[] particles;
    public Transform target;
    public float pointPerParticle;
    public float targetZDepth = 0.25f;
    Color[] colors =
    {
        new Color(131 / 255f, 191 / 255f, 0) * 3.4f,
        new Color(44 / 255f, 0 / 255f, 191 / 255f) * 10.41f,
        new Color(44 / 255f, 0 / 255f, 191 / 255f) * 10.41f,
        new Color(44 / 255f, 0 / 255f, 191 / 255f) * 10.41f
    };
    Transform[] Targets = new Transform[4];

    int setTarget = -1;
    public void SetTarget(int i, Vector3 pos, float point)
    {
        ps = GetComponent<ParticleSystem>();
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        transform.position = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, targetZDepth));

        pointPerParticle = point;
        if (setTarget == i) return;

        Targets[0] = UpgradeManager.Instance.FlaskGage.Floor;
        Targets[1] = ObjetManager.Instance.CosmicOre;
        Targets[2] = ObjetManager.Instance.NewObjet1.GetComponent<RectTransform>();
        Targets[3] = ObjetManager.Instance.NewObjet2.GetComponent<RectTransform>();
        setTarget = i;

        ps.GetComponent<ParticleSystemRenderer>().material.color = colors[i];
        target = Targets[i];
        ps.collision.SetPlane(0, target);
    }

    public void Emit(int emit)
    {
        ps.Emit(emit);
    }
    public float SizeFactor;
    void LateUpdate()
    {
        if (ps == null || target == null || Camera.main == null)
            return;

        ParticleSystem.MainModule psMain = ps.main;
        psMain.startSize = SizeFactor * Camera.main.orthographicSize;

        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new Particle[ps.main.maxParticles];

        int numParticles = ps.GetParticles(particles);
        for (int i = 0; i < numParticles; i++)
        {
            Particle p = particles[i];
            float elapsedTime = p.startLifetime - p.remainingLifetime;
            if(setTarget == 0 && elapsedTime > 1f)
            {
                float t = (elapsedTime - 1f) / 2f;
                t = Mathf.Clamp01(t);
                p.position = Vector3.Lerp(p.position, target.position, t);
            }
            else if (setTarget == 1 && elapsedTime > 0.25f)
            {
                float t = (elapsedTime - 0.25f) / 0.4f;
                t = Mathf.Clamp01(t);
                p.position = Vector3.Lerp(p.position, target.position, t);
            }
            else if ((setTarget == 2 || setTarget == 3) && elapsedTime > 0.25f)
            {
                float t = (elapsedTime - 0.25f) / 1f;
                t = Mathf.Clamp01(t);
                p.position = Vector3.Lerp(p.position, target.position, t);
            }

            float dist = Vector3.Distance(p.position, target.position);

            if (dist < 0.1f && p.remainingLifetime > 0)
            {
                if (setTarget == 0)
                    Flask.Instance.UpdateScale(pointPerParticle);

                p.remainingLifetime = -1f;
            }
            particles[i] = p;
        }

        ps.SetParticles(particles, numParticles);
    }

}
