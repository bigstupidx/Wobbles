using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour
{
    public float Degrees;
    public float Speed;

    void Activate()
    {
        iTween.RotateTo(gameObject, iTween.Hash("z", Degrees, "speed", Speed, "easetype", iTween.EaseType.linear));
    }

    void Deactivate()
    {
        iTween.RotateTo(gameObject, iTween.Hash("z", 0f, "speed", Speed, "easetype", iTween.EaseType.linear));
    }
}
