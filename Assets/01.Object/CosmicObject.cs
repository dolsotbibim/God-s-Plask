using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CosmicObject : Object
{
    Renderer InnerRenderer;
    Color[] colors =
    {
        new Color(51 / 255f, 255 / 255f, 153 / 255f) * 7,
        new Color(255 / 255f, 221 / 255f, 102 / 255f) * 7,
        new Color(255 / 255f, 51 / 255f, 187 / 255f) * 7
    };
    public override int Level 
    { 
        get => base.Level;
        set
        {
            level = Mathf.Clamp(value, 1, 3);
            
            objectType = ObjectType.Cosmic;
            scale = Vector3.one * (1 + (level - 1) * 0.2f) * 0.1f;
            transform.localScale = scale * 0.2f;
            transform.DOScale(scale, 0.25f)
            .SetEase(Ease.OutQuart);
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

            InnerRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor("_Color", colors[level - 1]);

            InnerRenderer.SetPropertyBlock(propertyBlock);
            point = (int)(level * 10 * Mathf.Pow(1.1f, level));

            if (level == 3)
            {
                StartCoroutine(DecomposeRoutine());
                return;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        InnerRenderer = GetComponentsInChildren<Renderer>()[1];
    }
    public override void OnCollisionEnter(Collision collision)
    {
        if (PreventingChange)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Object"))
        {
            CosmicObject obj = collision.gameObject.GetComponent<CosmicObject>();

            if (obj == null) return;

            if (obj.Level == Level && !obj.PreventingChange)
            {
                if (transform.position.y < collision.transform.position.y ||
                    (transform.position.y == collision.transform.position.y && gameObject.GetInstanceID() < collision.gameObject.GetInstanceID()))
                {
                    PreventingChange = true;

                    Spawner.Instance.ReturnObject(obj);
                    Spawner.Instance.StartCoroutine(Spawner.Instance.SpawnMergedObject(Level + 1, transform.position, true));
                    Spawner.Instance.ReturnObject(this);
                }
            }
        }
    }

    IEnumerator DecomposeRoutine()
    {
        yield return new WaitForSeconds(3);
        if(gameObject.activeSelf)
            Decompose();
    }

    public override void Decompose()
    {
        if (gameObject.activeSelf && !PreventingChange)
        {
            PreventingChange = true;
            DecomposePS ps = Spawner.Instance.PoolManager.GetFromPool<DecomposePS>();
            ps.SetTarget(1, transform.position, scale.x, 1);
            ps.StartEmit(point);
            Spawner.Instance.ReturnObject(this);
        }
    }
}
