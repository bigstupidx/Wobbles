using UnityEngine;
using System.Collections;

public class GravityBehavior : BasePlatformBehavior 
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wobble") && other.attachedRigidbody && other.constantForce)
        {
            // Turn gravity on or off, actual mechanics handled by wobbles movement script
            other.attachedRigidbody.useGravity = !other.attachedRigidbody.useGravity;
            AudioManager.Play("Wobble_Gravity");
        }
    }
}
  