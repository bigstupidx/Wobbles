using UnityEngine;
using System.Collections;

public class OilSpillBehavior : BasePlatformBehavior 
{
    const float Velocity = 15f;
    const float VertBumpVel = 3f;

    void OnTriggerEnter(Collider other)
    {
        GameObject wobble = other.gameObject;
        if (wobble.CompareTag("Wobble") && wobble.rigidbody && wobble.constantForce)
        {
            AudioManager.Play("Wobble_Oil");
            // Horizontal force
            if (wobble.constantForce.force.x > 0f)
            {
                wobble.rigidbody.AddForce(Vector3.right * Velocity, ForceMode.VelocityChange);
            }
            else
            {
                wobble.rigidbody.AddForce(Vector3.left * Velocity, ForceMode.VelocityChange);
            }
            // Vertical force
            if(wobble.rigidbody.useGravity)
            {
                wobble.rigidbody.AddForce(Vector3.up * VertBumpVel - Vector3.up * wobble.rigidbody.velocity.y, ForceMode.VelocityChange);
            }
            else
            {
                wobble.rigidbody.AddForce(Vector3.down * VertBumpVel - Vector3.up * wobble.rigidbody.velocity.y, ForceMode.VelocityChange);
            }
        }
    }
}
