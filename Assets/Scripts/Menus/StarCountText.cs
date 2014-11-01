using UnityEngine;
using System.Collections;

public class StarCountText : MonoBehaviour 
{
	void Start ()
    {
        tk2dTextMesh tm = GetComponent<tk2dTextMesh>();	
        if(tm)
        {
            tm.text = Save.GetTotalStars().ToString();
            tm.Commit();
        }
	}
}
