using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DecompositionManager : MonoBehaviour
{
    public string targetLayerName = "Object";
    private LayerMask raycastMask;

    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f;

    public Transform flaskInstance;
    public float dragSpeed = 0.5f;

    public float moveLimit = 1.0f;
    private Vector3 initialPosition;
    private float currentOffset = 0f;

    private bool isPointerDown = false; // 클릭 상태 저장 변수

    void Start()
    {
        raycastMask = LayerMask.GetMask(targetLayerName);
        if (flaskInstance == null && Flask.Instance != null)
        {
            flaskInstance = Flask.Instance.transform;
        }

        if (flaskInstance != null)
        {
            initialPosition = flaskInstance.position;
        }
    }
    private Coroutine returnRoutine;
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isPointerDown = true; // 클릭 시작
            if (returnRoutine != null)
            {
                StopCoroutine(returnRoutine);
                returnRoutine = null;
            }
            HandleInteraction();
        }
        else if (context.canceled)
        {
            isPointerDown = false; // 클릭 해제
            if (flaskInstance != null)
            {
                if (returnRoutine != null) StopCoroutine(returnRoutine);
                returnRoutine = StartCoroutine(SmoothReturn());
            }
        }
    }

    private IEnumerator SmoothReturn()
    {
        Rigidbody rb = flaskInstance.GetComponent<Rigidbody>();
        Vector3 startPos = rb.position;
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = 1f - (1f - t) * (1f - t);
            rb.MovePosition(Vector3.Lerp(startPos, initialPosition, t));
            yield return null;
        }

        rb.MovePosition(initialPosition);
        currentOffset = 0f;
        returnRoutine = null;
    }

    private void HandleInteraction()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        float currentTime = Time.time;
        bool isDoubleClick = (currentTime - lastClickTime < doubleClickThreshold);
        lastClickTime = currentTime;

        if (isDoubleClick)
        {
            if (Physics.Raycast(ray, out hit, 1000f, raycastMask))
            {
                Object targetObj = hit.collider.GetComponent<Object>();
                if (targetObj != null)
                {
                    targetObj.Decompose();
                }
            }
        }
    }

    public void OnDrag(InputAction.CallbackContext context)
    {
        // isPointerDown이 true일 때만 (마우스를 누르고 있을 때만) 이동 수행
        if (isPointerDown && context.performed && flaskInstance != null)
        {
            moveLimit = Flask.Instance.CurrentScale * 0.25f;
            Vector2 delta = context.ReadValue<Vector2>();

            currentOffset += delta.x * dragSpeed / 1000 * flaskInstance.localScale.x;
            currentOffset = Mathf.Clamp(currentOffset, -moveLimit, moveLimit);

            Vector3 camRight = Camera.main.transform.right;
            camRight.y = 0;
            camRight.Normalize();

            flaskInstance.GetComponent<Rigidbody>().MovePosition(initialPosition + (camRight * currentOffset));
        }
    }
}