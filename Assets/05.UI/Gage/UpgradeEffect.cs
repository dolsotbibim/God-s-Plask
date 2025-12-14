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
        new Color(44 / 255f, 0 / 255f, 191 / 255f) * 10.41f
    };
    public Transform[] Targets = new Transform[2];

    int setTarget = -1;
    public void SetTarget(int i, Vector3 pos, float point)
    {
        ps = GetComponent<ParticleSystem>();
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, pos);
        transform.position = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPos.x, screenPos.y, targetZDepth));

        pointPerParticle = point;
        if (setTarget == i) return;

        Targets[0] = UpgradeManager.Instance.PlaskGage.Floor;
        //Targets[1] = Plask.Instance.Center;
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

            if (elapsedTime > 1f)
            {
                float t = (elapsedTime - 1f) / 1f;
                t = Mathf.Clamp01(t);

                p.position = Vector3.Lerp(p.position, target.position, t);
            }

            float dist = Vector2.Distance(p.position, target.position);

            if (dist < 1f)
            {
                Plask.Instance.UpdateScale(pointPerParticle);
                p.remainingLifetime = 0f;
            }
            particles[i] = p;
        }

        ps.SetParticles(particles, numParticles);
    }

}
