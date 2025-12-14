using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLookController : MonoBehaviour
{
    [Header("카메라 설정")]
    [Tooltip("카메라 회전 속도 (민감도)")]
    public float sensitivity = 1.0f;

    private Vector3 rotationCenter = Vector3.zero;

    private Vector2 lookDelta; // 마우스 이동(델타) 값
    public bool isRightClicking = false; // 우클릭 상태 플래그

    void Update()
    {
        if (isRightClicking)
        {
            ApplyLookRotation();
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookDelta = context.ReadValue<Vector2>();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isRightClicking = true;
        }
        else if (context.canceled)
        {
            isRightClicking = false;
        }
    }
    private void ApplyLookRotation()
    {
        float yawDelta = lookDelta.x * sensitivity * 10f;
        yawDelta *= Time.deltaTime;
        Spawner.Instance.transform.Rotate(Vector3.up, yawDelta);
    }
    private Quaternion initialWorldRotation;

    private void Start()
    {
        initialWorldRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = initialWorldRotation;
    }
}