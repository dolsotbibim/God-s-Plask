using UnityEngine;
using UnityEngine.InputSystem;

public class DecompositionManager : MonoBehaviour
{
    public string targetLayerName = "Object";
    private LayerMask raycastMask;
    void Start()
    {
        raycastMask = LayerMask.GetMask(targetLayerName);
    }
    public void OnClick(InputAction.CallbackContext context)
    {
        if (ObjetManager.Instance.IsObjetWindowEnabled) return;
        if (context.started)
        {
            Vector3 mouseScreenPosition = Vector3.zero;
            mouseScreenPosition = Mouse.current.position.ReadValue();

            Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);

            RaycastHit hit;
            float maxDistance = 1000f;

            if (Physics.Raycast(ray, out hit, maxDistance, raycastMask))
            {
                Object obj = hit.collider.GetComponent<Object>();

                if (obj)
                {
                    foreach (Object ob in Spawner.Instance.objs) if (ob != obj) ob.IsFocused = false;

                    if (obj.IsFocused)
                    {
                        obj.Decompose();
                    }
                    else
                    {
                        obj.IsFocused = true;
                    }
                }
            }
        }
    }
}
