using UnityEngine;
using System.Collections;

public class BatteryDock : MonoBehaviour 
{
    public GameObject[] Devices;
    public GameObject ElectricBall;
    GameObject tempElectricityRef;

    float timeSinceTap;
    bool activated;
    GameObject battery;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); RaycastHit hit;
            if(collider.Raycast(ray, out hit, 1000f))
            {
                // Deactivate if battery was double tap returned
                if(timeSinceTap < 0.3f && timeSinceTap > 0f)
                {
                    activated = false;
                    battery = null;
                    foreach (GameObject obj in Devices)
                    {
                        obj.SendMessage("Deactivate");                                                  
                    }
                    if (tempElectricityRef)
                    {
                        Destroy(tempElectricityRef);
                    }  
                }
                else
                {
                    timeSinceTap = 0f;
                }
            }
        }
        timeSinceTap += Time.deltaTime;

        if (activated && battery && battery.transform.parent.GetComponent<GadgetDrag>().GetDrag())
        {
            Vector3 position = Vector3.zero;
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                if (Input.touchCount > 0)
                {
                    position = Input.touches[0].position;
                }
            }
            else
            {
                position = Input.mousePosition;
            }
            Ray ray = Camera.main.ScreenPointToRay(position); RaycastHit hit;
            if (collider.Raycast(ray, out hit, 1000f))
            {
                battery.transform.parent.GetComponent<GadgetDrag>().SetAllowDrag(false);
                battery.transform.parent.position = new Vector3(transform.position.x, transform.position.y, battery.transform.position.z);
            }
            else
            {
                battery.transform.parent.GetComponent<GadgetDrag>().SetAllowDrag(true);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Battery"))
        {
            activated = true;
            battery = other.gameObject;
            foreach (GameObject obj in Devices)
            {
                obj.SendMessage("Activate");                
            }
            tempElectricityRef = Instantiate(ElectricBall, transform.position, Quaternion.identity) as GameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Battery"))
        {
            activated = false;
            battery = null;
            foreach (GameObject obj in Devices)
            {
                obj.SendMessage("Deactivate");                
            }
            if (tempElectricityRef)
            {
                Destroy(tempElectricityRef);
            } 
        }
    }
}
