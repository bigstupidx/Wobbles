using UnityEngine;
using System.Collections;

public class DestroySplash : MonoBehaviour
{
	void Start ()
    {
        Destroy(gameObject, 0.6f);
	}
}
