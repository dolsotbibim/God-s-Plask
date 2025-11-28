using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLookController : MonoBehaviour
{
    // === 인스펙터 설정 변수 ===
    [Header("카메라 설정")]
    [Tooltip("카메라 회전 속도 (민감도)")]
    public float sensitivity = 1.0f; 
    
    // === 내부 상태 변수 ===
    private Vector2 lookDelta; // 마우스 이동(델타) 값
    private bool isRightClicking = false; // 우클릭 상태 플래그

    // Update는 프레임마다 호출되며 부드러운 회전을 처리합니다.
    void Update()
    {
        // 우클릭 상태일 때만 회전 적용
        if (isRightClicking)
        {
            ApplyLookRotation();
        }
    }

    // === Input System 콜백 함수 ===

    // 1. Look 액션 처리 (Mouse/delta Vector2)
    public void OnLook(InputAction.CallbackContext context)
    {
        // 마우스 이동 벡터2 값을 가져옴
        lookDelta = context.ReadValue<Vector2>();
    }

    // 2. RightClick 액션 처리 (Mouse/rightButton Button)
    public void OnRightClick(InputAction.CallbackContext context)
    {
        // 우클릭 시작/종료 시 상태 플래그 업데이트
        if (context.started)
        {
            isRightClicking = true;
            // 필요하다면 여기서 Cursor.lockState = CursorLockMode.Locked; 등을 적용합니다.
        }
        else if (context.canceled)
        {
            isRightClicking = false;
            // 필요하다면 여기서 Cursor.lockState = CursorLockMode.None; 등을 적용합니다.
        }
    }

    // === 실제 Y축 회전 적용 로직 ===
    private void ApplyLookRotation()
    {
        // 마우스의 X축 움직임(lookDelta.x)을 회전 각도로 사용합니다.
        // Time.deltaTime을 곱하여 프레임 속도에 독립적으로 만듭니다. (10f는 조정 계수)
        float lookX = lookDelta.x * sensitivity * Time.deltaTime * 10f;
        
        // **transform.Rotate(Vector3.up, lookX, Space.World)**
        // 현재 오브젝트의 중심(transform)을 기준으로
        // 월드의 Y축(Vector3.up)을 중심으로 lookX만큼 회전합니다.
        transform.Rotate(Vector3.up, lookX, Space.World);
    }
}