using UnityEngine;

public class SnaradController : MonoBehaviour
{
    public float damping = 0.3f;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        Vector3 dampedVelocity = _rigidbody.linearVelocity * damping;
        _rigidbody.linearVelocity = dampedVelocity;
    }
}
