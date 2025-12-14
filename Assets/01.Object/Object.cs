using DG.Tweening;
using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
public class Object : MonoBehaviour
{
    protected int level = 1;
    protected int point = 10;
    Rigidbody rb;
    Outline outline;
    Renderer renderer;
    MeshFilter filter;
    public MaterialArrayData Mats;
    protected Vector3 scale;
    Collider collider;
    private float lidTime = 0;
    public float LidTime 
    {
        get { return lidTime; }
        set
        {
            lidTime = value;
            if(lidTime > 1f)
            {
                Decompose();
            }
        }
    }


    public virtual int Level
    {
        get { return level; } 
        set 
        { 
            level = value;
            point = 1;
            if (level <= 2) objectType = ObjectType.particle;
            else if (level <= 4) objectType = ObjectType.particle2;
            else if (level <= 13) objectType = ObjectType.Gem;
            else if (level <= 20) objectType = ObjectType.Planet;
            else objectType = ObjectType.Star;
            scale = Vector3.one * (0.5f + (level - 1) * 0.5f) * 0.03f * Mathf.Pow((int)objectType + 1, 3);
            transform.localScale = scale * 0.2f;
            transform.DOScale(scale, 0.25f)
            .SetEase(Ease.OutQuart);
            if (Mats.pairs.Count >= level)
            {
                int pairIndex = level - 1;
                MeshMaterialPair currentPair = Mats.pairs[pairIndex];

                if (currentPair.mesh != null)
                {
                    renderer.materials = currentPair.materials;
                    filter.mesh = currentPair.mesh;
                }
                else
                {
                    MeshMaterialPair defaultPair = Mats.pairs[0];

                    renderer.materials = defaultPair.materials;
                    filter.mesh = defaultPair.mesh;
                }
            }
            rb.mass = Mathf.Pow(scale.x + 1, 3);
            point = (int)(point * Mathf.Pow(1.1f, level - 1) * Mathf.Pow(2, level - 1));
        }
    }
    public ObjectType objectType;
    public enum ObjectType
    {
        particle,
        particle2,
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
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        outline = GetComponent<Outline>();
        renderer = GetComponent<Renderer>();
        filter = GetComponent<MeshFilter>();
        collider = GetComponent<Collider>();
    }
    public bool PreventingChange = true;
    public bool Merged;
    private void OnEnable()
    {
        LidTime = 0;
        IsFocused = false;
        PreventingChange = true;
        StartCoroutine(PreventMerge());


    }

    IEnumerator PreventMerge()
    {
        Physics.IgnoreCollision(collider, Lid.Instance.GetComponent<Collider>(), true);

        yield return new WaitForEndOfFrame();

        if (Merged)
        {
            PreventingChange = false;
            Physics.IgnoreCollision(collider, Lid.Instance.GetComponent<Collider>(), false);
            yield break;
        }
        yield return new WaitForSeconds(1);
        PreventingChange = false;
        yield return new WaitForSeconds(2);
        Physics.IgnoreCollision(collider, Lid.Instance.GetComponent<Collider>(), false);
    }

    private void OnDisable()
    {
        Lid.Instance.UpdateLidObjects();

        LidTime = 0;
        IsFocused = false;
        PreventingChange = true;

        Physics.IgnoreCollision(GetComponent<Collider>(), Lid.Instance.GetComponent<Collider>(), true);
    }
    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < 0.1f)
            rb.AddForce(new Vector3(-transform.position.x, 0, -transform.position.z).normalized * 3);

        if(transform.position.y < Plask.Instance.Under.position.y)
        {
            transform.position = Spawner.Instance.spawnPoint.position;
            StartCoroutine(PreventMerge());
        }

        if(rb.linearVelocity.y > 1)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 1, rb.linearVelocity.z);
        }
    }
    public virtual void OnCollisionEnter(Collision collision)
    {
        if (PreventingChange)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Object"))
        {
            Object obj = collision.gameObject.GetComponent<Object>();

            if (obj == null) return;

            if (obj.Level == Level && !obj.PreventingChange && obj.objectType != ObjectType.Cosmic)
            {
                if (transform.position.y < collision.transform.position.y ||
                (transform.position.y == collision.transform.position.y && gameObject.GetInstanceID() < collision.gameObject.GetInstanceID()))
                {
                    PreventingChange = true;

                    Spawner.Instance.ReturnObject(obj);
                    Spawner.Instance.StartCoroutine(Spawner.Instance.SpawnMergedObject(Level + 1, transform.position, false));
                    Spawner.Instance.ReturnObject(this);
                }
            }
        }
    }

    public virtual void Decompose()
    {
        if(gameObject.activeSelf && !PreventingChange)
        {
            PreventingChange = true;
            DecomposePS ps = Spawner.Instance.PoolManager.GetFromPool<DecomposePS>();
            ps.SetTarget(0, transform.position, scale.x, 1);
            ps.StartEmit(point);

            if(Random.value < Spawner.Instance.CosmicProbability)
            {
                DecomposePS ps2 = Spawner.Instance.PoolManager.GetFromPool<DecomposePS>();
                ps2.SetTarget(1, transform.position, scale.x, 1);
                ps2.StartEmit((int)Random.Range(point * 0.9f, point * 1.1f));
            }
            

            Spawner.Instance.ReturnObject(this);
        }
    }

    public void SetKinematic(bool value)
    {
        rb.isKinematic = value;
    }
}
