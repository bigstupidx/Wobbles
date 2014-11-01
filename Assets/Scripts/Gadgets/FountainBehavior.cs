using UnityEngine;
using System.Collections;

public class FountainBehavior : BasePlatformBehavior
{
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Wobble") && other.attachedRigidbody)
        {
            // Reduce y velocity by 15% each frame
            other.attachedRigidbody.AddForce(Vector3.up * other.attachedRigidbody.velocity.y * -0.15f, ForceMode.VelocityChange);
        }
    }
}
