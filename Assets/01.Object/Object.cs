using DG.Tweening;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;
public class Object : MonoBehaviour
{
    protected int level = 1;
    protected float point = 1;
    Rigidbody rb;
    Outline outline;
    Renderer renderer;
    public MaterialArrayData Mats;
    protected Vector3 scale;
    public virtual int Level
    {
        get { return level; } 
        set 
        { 
            level = value;
            point = 1;
            if (level <= 4) objectType = ObjectType.particle;
            else if (level <= 13) objectType = ObjectType.Gem;
            else if (level <= 20) objectType = ObjectType.Planet;
            else objectType = ObjectType.Star;
            scale = Vector3.one * (1 + (level - 1) * 0.2f) * 0.1f * Mathf.Pow((int)objectType + 1, 2);
            transform.localScale = scale * 0.2f;
            transform.DOScale(scale, 0.25f)
            .SetEase(Ease.OutQuart);
            if(Mats.MaterialCount >= level)
                renderer.material = Mats.materials[level - 1];
            point = point * Mathf.Pow(1.05f, level - 1) * Mathf.Pow(2, level - 1);
        }
    }
    public ObjectType objectType;
    public enum ObjectType
    {
        particle,
        Gem,
        Planet,
        Star,
        Cosmic
    }
    bool isFocused = false;
    public bool IsFocused
    {
        get
            { return isFocused; }
        set
        {
            isFocused = value;
            outline.enabled = isFocused;
        }
    }
    protected bool isIgnoringLid = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        outline = GetComponent<Outline>();
        renderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        IsFocused = false;
    }

    private void OnDisable()
    {
        Lid.Instance.UpdateLidObjects();
    }
    private void FixedUpdate()
    {
        bool shouldIgnore = transform.position.y > Lid.Instance.transform.position.y;

        if (shouldIgnore != isIgnoringLid)
        {
            isIgnoringLid = shouldIgnore;
            Physics.IgnoreCollision(
                GetComponent<Collider>(),
                Lid.Instance.GetComponent<Collider>(),
                shouldIgnore
            );
        }
        if (rb.linearVelocity.magnitude < 0.05f)
            rb.AddForce(new Vector3(-transform.position.x, 0, -transform.position.z).normalized);

        
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (isIgnoringLid) return;

        if (collision.gameObject.CompareTag("Object"))
        {
            if(collision.gameObject.GetComponent<Object>().Level == Level && collision.gameObject.GetComponent<Object>().objectType != ObjectType.Cosmic)
            {
                if(transform.position.y < collision.transform.position.y ||
                (transform.position.y == collision.transform.position.y && gameObject.GetInstanceID() < collision.gameObject.GetInstanceID()))
                {
                    Spawner.Instance.ReturnObject(collision.gameObject.GetComponent<Object>());
                    Spawner.Instance.StartCoroutine(Spawner.Instance.SpawnMergedObject(Level + 1, transform, false));
                    Spawner.Instance.ReturnObject(this);
                }
            }
        }
    }

    public virtual void Decompose()
    {
        if(gameObject.activeSelf)
        {
            UpgradeManager.Instance.PlaskGage.PlaskPoint += point;
            Spawner.Instance.ReturnObject(this);
        }
    }

    public void SetKinematic(bool value)
    {
        rb.isKinematic = value;
    }
}
