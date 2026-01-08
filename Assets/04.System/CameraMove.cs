using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraLookController : MonoBehaviour
{
    [Header("카메라 설정")]
    [Tooltip("카메라 회전 속도 (민감도)")]
    public float sensitivity = 1.0f;
    public static CameraLookController instance;
    private Vector3 rotationCenter = Vector3.zero;

    private Vector2 lookDelta; // 마우스 이동(델타) 값
    public bool isRightClicking = false; // 우클릭 상태 플래그
    private Camera cam;

    private float initialScale;
    private float initialDistance;
    private Vector3 initialDirection;

    private void Awake()
    {
        instance = this;

    }
    void Start()
    {
        cam = GetComponent<Camera>();
        initialWorldRotation = transform.rotation;
        // 1. 초기 상태 저장
        initialScale = Flask.Instance.transform.localScale.x;

        // 카메라와 오브젝트 사이의 벡터와 거리 계산
        Vector3 diff = transform.position - Flask.Instance.transform.position;
        initialDistance = diff.magnitude;
        initialDirection = diff.normalized; // 방향 유지용
    }

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
    public Material SkyboxMaterial;
    private void ApplyLookRotation()
    {
        float yawDelta = lookDelta.x * sensitivity * 10f;
        yawDelta *= Time.deltaTime;

        Spawner.Instance.transform.Rotate(Vector3.up, yawDelta);
        SkyboxMaterial.SetFloat("_Rotation", Spawner.Instance.transform.eulerAngles.y);
    }
    private Quaternion initialWorldRotation;
    public bool isShaking = false;
    public void ShakeCamera()
    {
        isShaking = true;
    }
    private void LateUpdate()
    {
        transform.rotation = initialWorldRotation;

        if (Flask.Instance == null) return;
        if (isShaking) return;
        // 2. 현재 스케일 비율 계산
        float currentScale = Flask.Instance.transform.localScale.x;
        float scaleRatio = currentScale / initialScale;

        // 3. 비율에 맞춰 새로운 거리 계산
        float newDistance = initialDistance * scaleRatio;

        // 4. 카메라 위치 갱신 (오브젝트의 현재 위치 + 초기 방향 * 새로운 거리)
        transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0.5f, 0) + (initialDirection * newDistance), 0.1f);

    }
}