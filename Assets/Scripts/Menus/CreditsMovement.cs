using UnityEngine;
using System.Collections;

public class CreditsMovement : MonoBehaviour
{
	void Update ()
    {
        if(Input.GetMouseButton(0))
        {
            transform.Translate(Vector3.up * 3f * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.up * Time.deltaTime);
        }
	}

    void Home()
    {
        Application.LoadLevel("MainMenu");
    }
}
