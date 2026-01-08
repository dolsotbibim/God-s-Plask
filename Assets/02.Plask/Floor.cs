using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Object"))
        {
            if(other != null)
                other.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.up;
        }
    }
}
