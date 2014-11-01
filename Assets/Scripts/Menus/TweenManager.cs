using UnityEngine;
using System.Collections;

public class TweenManager : MonoBehaviour
{
    public GameObject[] Sprites;
    public GameObject[] StartPos;
    public float[] Delays;


	void Start ()
    {
        int amount = Sprites.Length;
        for(int i=0; i<amount; ++i)
        {
            iTween.MoveFrom(Sprites[i], iTween.Hash(
                "position", StartPos[i].transform.position,
                "time", 0.75f,
                "delay", Delays[i] + Random.value * 0.25f,
                "easetype", iTween.EaseType.easeInExpo
                ));
        }
        Time.timeScale = 1f;
	}
}
