using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //Movement

    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector] //inspector stays clean, no confusion about whether variable needs to be set
    public Vector2 moveDir; //used to check the players moving direction
    [HideInInspector]
    public Vector2 lastMovedVector;

    //References
    PlayerStats player;
    Rigidbody2D rb; //used as a reference
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //reference
        lastMovedVector = new Vector2(-1, 0f); //since weapon default is right and the player does not move at the start, we have to set the lastmovedVector so projectile moves
        player = gameObject.GetComponent<PlayerStats>();
    }


    void Update()
    {
        InputManagement(); //so the input gets updated regularly
    }


    void FixedUpdate() /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled. Is more suited for physics calculations
    {
        Move(); //because move relies on physics in its calculations it goes inside FixedUpdate
    }


    void InputManagement() //to process all our inputs
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }
        float moveX = Input.GetAxisRaw("Horizontal"); //gets the Input on the respective Axis, here Left, Right, a and d
        float moveY = Input.GetAxisRaw("Vertical"); //here Up, Down, w and s, and to tell which buttons are being pressed

        moveDir = new Vector2(moveX, moveY).normalized;
        if (moveDir.x != 0) //if Player moving horizontally
        {
            lastHorizontalVector = moveDir.x; //save the last x value before player stopped moving horizontally
            lastMovedVector = new Vector2(lastHorizontalVector, 0f); //last moved x
        }

        if (moveDir.y != 0) //if Player moving vertically
        {
            lastVerticalVector = moveDir.y; //save last y value before player stopped moving vertically
            lastMovedVector = new Vector2(0f, lastVerticalVector); //last moved y

        }
        if (moveDir.x != 0 && moveDir.y != 0) //while moving, last moved are just the last direction
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }

    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rb.velocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed); //sets the velocity of the rigidbody attached to the object to a new Vector with speed applied?


    }
}

