using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour 
{
    public float Time;
    public bool Clockwise;

	void Start () 
    {
        float amount = Clockwise ? -360f : 360f;
        iTween.RotateAdd(gameObject, iTween.Hash(
            "z", amount,
            "time", Time,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.loop
            ));
	}
}
