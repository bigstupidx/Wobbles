using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour
{
    public Vector3 Amount;
    public float Speed;

    Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Activate()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", originalPosition + Amount, "speed", Speed, "easetype", iTween.EaseType.linear));
    }

    void Deactivate()
    {
        iTween.MoveTo(gameObject, iTween.Hash("position", originalPosition, "speed", Speed, "easetype", iTween.EaseType.linear));
    }
}
