using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField]
    private InputActionReference spawnActionReference;

    
    public static UpgradeManager Instance;
    public FlaskGage FlaskGage;
    private void Awake()
    {
        Instance = this;
    }

    
    public void UpgradeFlask()
    {
        Flask.Instance.Level += 1;
    }
}
