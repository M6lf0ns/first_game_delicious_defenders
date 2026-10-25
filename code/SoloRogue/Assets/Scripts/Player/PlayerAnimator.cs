using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    //References to get the Animator, the Script and the Sprite Renderer from the assigned Object
    Animator am;
    PlayerMovement pm;
    SpriteRenderer sr;

    void Start()
    {
        am = GetComponent<Animator>();
        pm = GetComponent<PlayerMovement>();
        sr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        if (pm.moveDir.x != 0 || pm.moveDir.y != 0) //check if player is moving by checking if x or y values of moveDir Vector is zero
        {
            am.SetBool("Move", true);

            SpriteDirectionChecker();
        }
        else
        {
            am.SetBool("Move", false);
        }
    }

    void SpriteDirectionChecker() //used to flip the Player sprite so it faces in the right direction
    {
        if (pm.lastHorizontalVector < 0) //if Player moves left --> Sprite flips (if there was still input)
        {
            sr.flipX = false;
        }
        else //if P moves right, no flip
        {
            sr.flipX = true;
        }

    }

    public void SetAnimatorController(RuntimeAnimatorController controller)
    {   //Ensure that am has an Animator
        if (!am) am = GetComponent<Animator>();
        am.runtimeAnimatorController = controller; //If not, assign the corresponding one


    }

    // Allows us to update the default sprite and animations.
    public void SetSprites(Sprite sprite, RuntimeAnimatorController controller = null)
    {
        sr = GetComponent<SpriteRenderer>();
        if (controller)
        {
            am = GetComponent<Animator>();
            am.runtimeAnimatorController = controller;
        }
        sr.sprite = sprite;
    }
}

