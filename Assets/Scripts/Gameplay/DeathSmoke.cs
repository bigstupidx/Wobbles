using UnityEngine;
using System.Collections;

public class DeathSmoke : MonoBehaviour
{
    tk2dAnimatedSprite sprite;

	void Start ()
    {
        sprite = GetComponent<tk2dAnimatedSprite>();
        sprite.animationCompleteDelegate = CompleteDelegate;
        iTween.ValueTo(gameObject, iTween.Hash(
                "from", 1f,
                "to", 0f,
                "easetype", iTween.EaseType.linear,
                "onupdate", "TintSprite",
                "onupdatetarget", gameObject,
                "time", 0.6f));
	}

    void TintSprite(float alpha)
    {
        sprite.color = new Color(1f, 1f, 1f, alpha);
    }

    void CompleteDelegate(tk2dAnimatedSprite sprite, int clipId)
    {
        Destroy(gameObject);
    }
}
