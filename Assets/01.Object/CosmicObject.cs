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
            level = value;
            
            objectType = ObjectType.Cosmic;
            scale = Vector3.one * (1 + (level - 1) * 0.2f) * 0.1f;
            transform.localScale = scale * 0.2f;
            transform.DOScale(scale, 0.25f)
            .SetEase(Ease.OutQuart);
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

            InnerRenderer.GetPropertyBlock(propertyBlock);

            propertyBlock.SetColor("_Color", colors[level - 1]);

            InnerRenderer.SetPropertyBlock(propertyBlock);
            point = level * 10;

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
        if (isIgnoringLid) return;

        if (collision.gameObject.CompareTag("Object"))
        {
            if (collision.gameObject.GetComponent<Object>().Level == Level && collision.transform.GetComponent<CosmicObject>())
            {
                if (transform.position.y < collision.transform.position.y ||
                (transform.position.y == collision.transform.position.y && gameObject.GetInstanceID() < collision.gameObject.GetInstanceID())
                )
                {
                    Spawner.Instance.ReturnObject(collision.gameObject.GetComponent<CosmicObject>());
                    Spawner.Instance.StartCoroutine(Spawner.Instance.SpawnMergedObject(Level + 1, transform, true));
                    Spawner.Instance.ReturnObject(this);
                }
            }
        }
    }

    IEnumerator DecomposeRoutine()
    {
        yield return new WaitForSeconds(3);
        Decompose();
    }

    public override void Decompose()
    {
        FindAnyObjectByType<CosmicGage>().CosmicPoint += point;
        Spawner.Instance.ReturnObject(this);
    }
}
