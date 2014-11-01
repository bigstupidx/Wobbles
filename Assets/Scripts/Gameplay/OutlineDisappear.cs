using UnityEngine;
using System.Collections;

public class OutlineDisappear : MonoBehaviour
{
	void Update ()
    {
        Ray ray = new Ray(transform.position, Vector3.back);RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 100f))
        {
            bool destroy = false;
            if(hit.collider.gameObject.GetComponent<GadgetDrag>())
            {
                destroy = !hit.collider.gameObject.GetComponent<GadgetDrag>().GetDrag();
            }
            else if (hit.collider.transform.parent &&
                hit.collider.transform.parent.GetComponent<GadgetDrag>())
            {
                destroy = !hit.collider.transform.parent.GetComponent<GadgetDrag>().GetDrag();
            }
            else if (hit.collider.transform.parent && 
                hit.collider.transform.parent.parent &&
                hit.collider.transform.parent.parent.GetComponent<GadgetDrag>())
            {
                destroy = !hit.collider.transform.parent.GetComponent<GadgetDrag>().GetDrag();
            }
            if (destroy)
            {
                Destroy(gameObject);
            }
        }
	}
}
