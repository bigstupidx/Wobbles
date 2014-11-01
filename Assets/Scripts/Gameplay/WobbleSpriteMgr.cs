using UnityEngine;
using System.Collections;

public class WobbleSpriteMgr : MonoBehaviour
{
    bool goingRight = true;
    bool rightSideUp = true;
    tk2dSpriteAnimator sprite;
    

    // Unity functions
    void Start()
    {
        sprite = GetComponent<tk2dSpriteAnimator>();
    }
    void Update()
    {
        CheckAnimations();
        CheckSpriteDirection();
    }

    // Helper functions
    void PlayAnimation(string name)
    {
        sprite.Play(name);
    }
    void CheckAnimations()
    {
        if (Mathf.Abs(rigidbody.velocity.y) > 0.1f)
        {
            if (!sprite.IsPlaying("Fall") && !sprite.IsPlaying("Jump"))
                sprite.Play("Fall");
        }
        else
        {
            if (!sprite.IsPlaying("Walk") && !sprite.IsPlaying("Jump"))
                sprite.Play("Walk");
        }
    }
    void CheckSpriteDirection()
    {
        // Flips the sprite to the direction it's moving
        if (constantForce.force.x > 0f)
        {
            if (!goingRight)
            {
                sprite.Sprite.FlipX = false;
                goingRight = true;
            }
        }
        else
        {
            if (goingRight)
            {
                sprite.Sprite.FlipX = true;
                goingRight = false;
            }
        }
        // Flips vertically for reverse gravity
        if (rigidbody.useGravity)
        {
            if (!rightSideUp)
            {
                sprite.Sprite.FlipY = false;
                rightSideUp = true;
            }
        }
        else
        {
            if (rightSideUp)
            {
                sprite.Sprite.FlipY = true;
                rightSideUp = false;
            }
        }
    }
}
