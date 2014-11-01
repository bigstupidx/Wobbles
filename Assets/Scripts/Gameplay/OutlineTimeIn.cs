using UnityEngine;
using System.Collections;

public class OutlineTimeIn : MonoBehaviour 
{
    public float Seconds;
    float elapsedTime = 0f;

	void Start ()
    {
        GetComponent<tk2dSprite>().color = new Color(0f, 0f, 0f, 0f);
	}

    void Update()
    {
        if (Menu.State == GameMenuState.Closed)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > Seconds)
            {
                GetComponent<tk2dSprite>().color = new Color(1f, 1f, 1f, 1f);
                Destroy(this);
            }
        }
    }
}
