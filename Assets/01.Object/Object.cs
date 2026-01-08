using DG.Tweening;
using System.Collections;
using UnityEngine;
public class Object : MonoBehaviour
{
    protected int level = 1;
    protected int point = 10;
    Rigidbody rb;
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
            if(lidTime >= 1f)
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
            point = 2;
            if (level <= 8) objectType = ObjectType.Gem;
            else if (level <= 15) objectType = ObjectType.Ore;
            else if (level <= 23) objectType = ObjectType.Planet;
            else if (level <= 27) objectType = ObjectType.Star;
            else
            {
                BlackHole.instance.gameObject.SetActive(true);
                return;
            }
            float volume = 1;
            if (Merged) volume = Mathf.Pow(1.75f, level) / Mathf.Pow(1.75f, Spawner.Instance.CurrentMaxObjectLevel + 1);
            if (level <= 3) SoundManager.Instance.PlaySFX(9, volume, true);
            else if (level <= 7) SoundManager.Instance.PlaySFX(11, volume * 0.8f, true);
            else if (level <= 10) SoundManager.Instance.PlaySFX(9, volume, true);
            else if (level <= 14) SoundManager.Instance.PlaySFX(11, volume * 0.8f, true);
            else if (level <= 22) SoundManager.Instance.PlaySFX(13, volume * 1.5f, true);
            else SoundManager.Instance.PlaySFX(15, volume * 1.3f, true);
            scale = Vector3.one * (Mathf.Pow(1.75f, level) * 0.15f);
            transform.localScale = scale * 0.2f;
            transform.DOScale(scale, 0.25f)
            .SetEase(Ease.OutQuart);
            if (Mats.pairs.Count > level)
            {
                int pairIndex = level;
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
            point = (int)(point * Mathf.Pow(2.1f, level));
        }
    }
    public ObjectType objectType;
    public enum ObjectType
    {
        Gem,
        Ore,
        Planet,
        Star
    }
    bool isFocused = false;
    public bool IsFocused
    {
        get
            { return isFocused; }
        set
        {
            isFocused = value;
        }
    }
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        float elapsed = 0;
        float Duration = Random.Range(0.75f, 1.25f);

        if (Merged)
        {
            Duration = Random.Range(0.25f, 0.5f);
            while (elapsed < Duration)
            {
                rb.AddForce(new Vector3(0, -9.8f, 0) * Flask.Instance.CurrentScale * rb.mass);
                bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 100f);

                if (!isGrounded)
                {
                    transform.position = Vector3.MoveTowards(transform.position, Vector3.zero + Vector3.up * 2, 0.1f);
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(Random.Range(0.25f, 0.5f));
            PreventingChange = false;
            Physics.IgnoreCollision(collider, Lid.Instance.GetComponent<Collider>(), false);
            yield break;
        }
        while (elapsed < Duration)
        {
            rb.AddForce(new Vector3(0, -9.8f, 0) * Flask.Instance.CurrentScale * rb.mass);
            elapsed += Time.deltaTime;
            yield return null;
        }
        PreventingChange = false;
        yield return new WaitForSeconds(1f);
        Physics.IgnoreCollision(collider, Lid.Instance.GetComponent<Collider>(), false);
    }

    private void OnDisable()
    {
        if(Lid.Instance != null)
            Lid.Instance.UpdateLidObjects();

        LidTime = 0;
        IsFocused = false;
        PreventingChange = true;
        if(Lid.Instance != null)
            Physics.IgnoreCollision(GetComponent<Collider>(), Lid.Instance.GetComponent<Collider>(), true);
    }
    private void FixedUpdate()
    {
        if (rb.linearVelocity.magnitude < 0.1f * scale.x)
            rb.AddForce(new Vector3(-transform.position.x, 0, -transform.position.z).normalized * scale.x);

        if(transform.position.y < Flask.Instance.Under.position.y)
        {
            transform.position = Spawner.Instance.spawnPoint.position;
            StartCoroutine(PreventMerge());
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

            if (obj.Level == Level && !obj.PreventingChange)
            {
                if (transform.position.y < collision.transform.position.y ||
                (transform.position.y == collision.transform.position.y && gameObject.GetInstanceID() < collision.gameObject.GetInstanceID()))
                {
                    PreventingChange = true;
                    obj.PreventingChange = true;
                    Spawner.Instance.ReturnObject(obj);
                    Spawner.Instance.StartCoroutine(Spawner.Instance.SpawnMergedObject(Level + 1, transform.position, false));
                    Spawner.Instance.ReturnObject(this);
                }
            }
        }
    }

    public virtual void OnCollisionStay(Collision collision)
    {
        if (PreventingChange)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Object"))
        {
            Object obj = collision.gameObject.GetComponent<Object>();

            if (obj == null) return;

            if (obj.Level == Level && !obj.PreventingChange)
            {
                if (transform.position.y < collision.transform.position.y ||
                (transform.position.y == collision.transform.position.y && gameObject.GetInstanceID() < collision.gameObject.GetInstanceID()))
                {
                    PreventingChange = true;
                    obj.PreventingChange = true;
                    Spawner.Instance.ReturnObject(obj);
                    Spawner.Instance.StartCoroutine(Spawner.Instance.SpawnMergedObject(Level + 1, transform.position, false));
                    Spawner.Instance.ReturnObject(this);
                }
            }
        }
    }

    public static float StarPointMultiplier = 1f;
    public static float CosmicPointMultiplier = 1f;
    public virtual void Decompose()
    {
        if(gameObject.activeSelf && !PreventingChange)
        {
            if (level <= 3) SoundManager.Instance.PlaySFX(10, ((level + 1) / (float)Spawner.Instance.CurrentMaxObjectLevel), true);
            else if (level <= 7) SoundManager.Instance.PlaySFX(12, ((level + 1) / (float)Spawner.Instance.CurrentMaxObjectLevel) * 0.8f, true);
            else if (level <= 10) SoundManager.Instance.PlaySFX(10, ((level + 1) / (float)Spawner.Instance.CurrentMaxObjectLevel), true);
            else if (level <= 14) SoundManager.Instance.PlaySFX(12, ((level + 1) / (float)Spawner.Instance.CurrentMaxObjectLevel) * 0.8f, true);
            else if (level <= 22) SoundManager.Instance.PlaySFX(14, ((level + 1) / (float)Spawner.Instance.CurrentMaxObjectLevel) * 1.5f, true);
            else SoundManager.Instance.PlaySFX(16, ((level + 1) / (float)Spawner.Instance.CurrentMaxObjectLevel) * 1.3f, true);


            PreventingChange = true;
            DecomposePS ps = Spawner.Instance.PoolManager.GetFromPool<DecomposePS>();
            ps.SetTarget(0, transform.position, scale.x, StarPointMultiplier * point / (level + 1));
            ps.StartEmit(level + 1);

            if(Random.value < Spawner.Instance.CosmicProbability)
            {
                DecomposePS ps2 = Spawner.Instance.PoolManager.GetFromPool<DecomposePS>();
                ps2.SetTarget(1, transform.position, scale.x, CosmicPointMultiplier * point / (level + 1));
                ps2.StartEmit(level + 1);
            }
            

            Spawner.Instance.ReturnObject(this);
        }
    }

    public void SetKinematic(bool value)
    {
        rb.isKinematic = value;
    }
}
