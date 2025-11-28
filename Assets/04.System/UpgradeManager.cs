using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField]
    private InputActionReference spawnActionReference;

    
    public static UpgradeManager Instance;
    public PlaskGage PlaskGage;
    private void Awake()
    {
        Instance = this;
    }

    
    public void UpgradePlask()
    {
        Plask.Instance.Level += 1;
    }
}
