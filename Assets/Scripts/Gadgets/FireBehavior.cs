using UnityEngine;
using System.Collections;

public class FireBehavior : BasePlatformBehavior 
{
    public float Velocity;
    public GameObject SmokeTrail;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wobble"))
        {
            // Subtract the current velocity (for uniform bounce) then add in extra velocity
            if (other.attachedRigidbody.useGravity)
            {
                other.attachedRigidbody.AddForce(Vector3.up * Velocity - Vector3.up * other.attachedRigidbody.velocity.y, ForceMode.VelocityChange);
            }
            else
            {
                other.attachedRigidbody.AddForce(Vector3.down * Velocity - Vector3.up * other.attachedRigidbody.velocity.y, ForceMode.VelocityChange);
            }
            AudioManager.Play("Scream");
            other.gameObject.SendMessage("PlayAnimation", "Jump");
        }
    }
}
  