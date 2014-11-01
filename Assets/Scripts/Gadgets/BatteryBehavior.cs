using UnityEngine;
using System.Collections;

public class BatteryBehavior : MonoBehaviour
{

    protected GadgetDrag platform;
    void Start()
    {
        platform = transform.parent.GetComponent<GadgetDrag>();
    }

    void Update()
    {
        if(platform.GetDrag())
        {
            gameObject.layer = 8;
        }
        else
        {
            gameObject.layer = 8;
        }
    }
}
