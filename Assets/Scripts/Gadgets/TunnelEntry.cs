using UnityEngine;
using System.Collections;

public class TunnelEntry : MonoBehaviour 
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wobble"))
        {
            SendMessageUpwards("TriggerEntrance", other);
        }       
    }
}
