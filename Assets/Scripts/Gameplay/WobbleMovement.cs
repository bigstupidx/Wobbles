using UnityEngine;
using System.Collections;

public class WobbleMovement : MonoBehaviour 
{
    const float DeadlyVelocity = 7.6f;
    const float MaxHorizVelocity = 2.5f;
    
    float prevVerticalVelocity;
    Vector3 prevPosition;

    // Unity functions
    void OnCollisionEnter(Collision collision)
    {
        // Kill wobble if they impact too hard
        if (!collision.gameObject.CompareTag("Platform"))
        {
            if (rigidbody.useGravity ? prevVerticalVelocity < -Mathf.Abs(DeadlyVelocity) : prevVerticalVelocity > Mathf.Abs(DeadlyVelocity))
            {
                foreach (ContactPoint point in collision.contacts)
                {
                    // Check the collision anlgle so they dont die when they hit a wall while falling
                    float angle = Mathf.Atan2(point.normal.y, point.normal.x);
                    if (angle < 0) angle += Mathf.PI * 2f;
                    angle *= Mathf.Rad2Deg;
                    if ((angle > 45f && angle < 135f) || (angle < 315f && angle > 225f))
                    {
                        SendMessage("DeathWithSmoke");
                        return;
                    }
                }
            }
        }
    }
    void FixedUpdate()
    {
        prevVerticalVelocity = rigidbody.velocity.y;

        // If not moving forward, give bump to get over obstacle
        if ((transform.position - prevPosition).sqrMagnitude < 0.000025f)
        {
            rigidbody.AddForce(Vector3.up * (rigidbody.useGravity ? 2f : -2), ForceMode.VelocityChange);
        }
        prevPosition = transform.position;

        // Use raycast to turn around if it hits a wall
        CheckForWallCollision();

        // This checks if we are using gravity, and if not applies 
        // an opposite gravitational force
        if (!rigidbody.useGravity)
        {
            rigidbody.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }

        // This checks whether or not we are going over the preffered speed 
        // If we are, then it applies force in the opposite direction
        if (Mathf.Abs(rigidbody.velocity.x) > MaxHorizVelocity)
        {
            rigidbody.AddForce(-constantForce.force.x * 2f, 0f, 0f, ForceMode.Force); 
        }
    }

    // Helper functions
    void CheckForWallCollision()
    {
        // Checks for collision in front of the wobble to reverse direction
        float radius = (collider as CapsuleCollider).radius;
        Vector3 position = transform.position - Vector3.up * ((collider as CapsuleCollider).height * 0.5f - radius);
        Vector3 position2 = transform.position + Vector3.up * ((collider as CapsuleCollider).height * 0.5f - radius);
        Vector3 direction = (constantForce.force.x > 0f ? Vector3.right : Vector3.left);
        Ray ray = new Ray(position, direction);
        Ray ray2 = new Ray(position2, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, collider.bounds.extents.x * 1.1f))
        {
            string tag = hit.collider.tag;
            if (tag != "Platform" && tag != "Wobble" && tag != "Home" && tag != "HeadGear" && tag != "Battery" && tag != "Death")
            {
                SwitchDirection();
            }
        }
        else if (Physics.Raycast(ray2, out hit, collider.bounds.extents.x * 1.1f))
        {
            string tag = hit.collider.tag;
            if (tag != "Platform" && tag != "Wobble" && tag != "Home" && tag != "HeadGear" && tag != "Battery" && tag != "Death")
            {
                SwitchDirection();
            }
        }
    }    
    void SwitchDirection()
    {
        constantForce.force = new Vector3(-constantForce.force.x, 0f);
        rigidbody.AddForce(-rigidbody.velocity.x + (constantForce.force.x > 0f ? 1.5f : -1.5f), 0f, 0f, ForceMode.VelocityChange);
    }
}
