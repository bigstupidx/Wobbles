using UnityEngine;
using System.Collections;

public class BasePlatformBehavior : MonoBehaviour 
{
    protected GadgetDrag platform;
    void Start()
    {
        platform = transform.parent.GetComponent<GadgetDrag>();
    }

    void Update()
    {
        if (platform)
        {
            // Disables the collider if the platform is being move and vice versa
            if (collider)
            {
                collider.enabled = !platform.GetDrag();
            }
            foreach (BoxCollider boxCollid in GetComponentsInChildren<BoxCollider>())
            {
                boxCollid.enabled = !platform.GetDrag();
            }
        }
    }
}
