using UnityEngine;
using System.Collections;

public class TunnelBehavior : BasePlatformBehavior 
{
    public BoxCollider Left;
    public BoxCollider Right;

    bool checkAllow;
    ArrayList wobblesInTransit;
    ArrayList wobbleWaitList;
    
    void Start()
    {
        wobblesInTransit = new ArrayList(8);
        wobbleWaitList = new ArrayList(8);
        platform = transform.parent.GetComponent<GadgetDrag>();
    }
    void Update()
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
        // This detects when the tunnel is picked up
        // It stops all wobbles being transported and places them all
        // Then it puts them all in the wait list and removes them one second later
        if (platform.GetDrag() && checkAllow)
        {
            checkAllow = false;
            StopAllCoroutines();
            foreach (object obj in wobblesInTransit)
            {
                PlaceWobble(obj as GameObject);
                wobbleWaitList.Add(obj);
            }
            wobblesInTransit.Clear();
            Invoke("ClearWobbleWaitList", 1f);
        }
        if(!platform.GetDrag())
        {
            checkAllow = true;
        }
    }    
    void TriggerEntrance(Collider wobble)
    {
        // If the wobble is going the right direction for its location (to prevent trapping), transport wobble
        if ((wobble.gameObject.transform.position.x < transform.position.x && wobble.gameObject.constantForce.force.x > 0f) ||
            (wobble.gameObject.transform.position.x > transform.position.x && wobble.gameObject.constantForce.force.x < 0f))
        {
            StartCoroutine("TransportWobble", wobble.gameObject);
        }
    }

    IEnumerator TransportWobble(GameObject wobble)
    {
        if (!wobbleWaitList.Contains(wobble))
        {
            wobble.SetActive(false);                // Removes the wobble from the scene
            wobblesInTransit.Add(wobble);           // Adds the wobble to the transport list
            yield return new WaitForSeconds(1f);    // Wait while they "walk" through
            PlaceWobble(wobble);                    // Place wobble
            wobblesInTransit.Remove(wobble);        // Remove from in transport list
            wobbleWaitList.Add(wobble);             // Add to the no grab list
            yield return new WaitForSeconds(1f);    // Wait so they don't get picked up again
            wobbleWaitList.Remove(wobble);          // Then remove from from no grab list
        }
    }
    void PlaceWobble(GameObject wobble)
    {
        wobble.SetActive(true);
        bool placeLeft = false;

        if(wobble.transform.position.x < transform.position.x)
        {
            Vector3 rayStart = Right.transform.position + Vector3.right * 0.5f; rayStart.z = -10f;
            Ray ray = new Ray(rayStart, Vector3.forward); RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000f, 1))
            {
                placeLeft = true;
                wobble.SendMessage("SwitchDirection");
            }
            else
            {
                placeLeft = false;
            }
        }
        else
        {
            Vector3 rayStart = Left.transform.position + Vector3.left * 0.5f; rayStart.z = -10f;
            Ray ray = new Ray(rayStart, Vector3.forward); RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, 1))
            {
            placeLeft = false;
            wobble.SendMessage("SwitchDirection");
            }
            else
            {
            placeLeft = true;
            }
        }
        
        BoxCollider boxCollider = placeLeft ? Left : Right;
        Vector3 newPosition = boxCollider.transform.position;
        newPosition += Vector3.right * (boxCollider.bounds.extents.x + (wobble.collider as CapsuleCollider).radius + 0.05f) * (placeLeft ? -1f : 1f);
        newPosition.z = -1f;
        wobble.transform.position = newPosition; 
        wobble.rigidbody.AddForce(-wobble.rigidbody.velocity, ForceMode.VelocityChange);
    }
    void ClearWobbleWaitList()
    {
        wobbleWaitList.Clear();
    }
}