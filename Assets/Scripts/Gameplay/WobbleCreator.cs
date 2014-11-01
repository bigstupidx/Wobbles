using UnityEngine;
using System.Collections;

public class WobbleCreator : MonoBehaviour
{
    public GameObject Wobble;
    public int NumberOfWobbles;

    void Start()
    {
        // Send in information to configure scoring
        Camera.main.gameObject.SendMessage("AddWobble", NumberOfWobbles);        
    }

    void StartWobbles()
    {
        InvokeRepeating("CreateWobble", 0.2f, 2f);
    }

    void CreateWobble()
    {
        // Places a wobble the creator's position, adds a random depth so they layer
        Instantiate(Wobble, transform.position + Vector3.forward * Random.value * 0.05f, Quaternion.identity);
        if (--NumberOfWobbles <= 0)
        {
            CancelInvoke();
        }
    }
}
