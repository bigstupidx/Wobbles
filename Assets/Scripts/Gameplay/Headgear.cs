using UnityEngine;
using System.Collections;

public class Headgear : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.name == "water")
        {
            rigidbody.drag = 7f;
            transform.Translate(Vector3.forward);
        }
    }

}
