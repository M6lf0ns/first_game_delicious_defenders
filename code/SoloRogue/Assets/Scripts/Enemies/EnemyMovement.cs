using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    SpriteRenderer sr;
    EnemyStats enemy;
    Transform playerTransform;
    PlayerMovement player;
    Vector2 knockbackSpeed;
    float knockbackDuration;
    bool playerMove;
    float currentDir;

    void Start()
    {
        playerTransform = FindObjectOfType<PlayerMovement>().transform;
        enemy = gameObject.GetComponent<EnemyStats>();
        sr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {

        //If currently being knocked back, process the knockback
        if (knockbackDuration > 0) //basically continue
        {
            transform.position += (Vector3)knockbackSpeed * Time.deltaTime;
            knockbackDuration -= Time.deltaTime; //Decrease duration
        }

        //if not, keep moving towards player
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.transform.position, enemy.currentMoveSpeed * Time.deltaTime); //Constantly move enemy towards player
        }

        if (playerTransform.position.x > gameObject.transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }






    }

    void SpriteDirectionChecker()
    {
        if (player.lastHorizontalVector > 0)
        {
            sr.flipX = true;
        }
        else if (player.lastHorizontalVector < 0)
        {
            sr.flipX = false;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Director"))
        {
            GameObject director = col.gameObject;
            SpriteDirectionChecker();
        }

    }

    public void Knockback(Vector2 speed, float duration)
    {
        //If knockback hasnt ended yet, return
        if (knockbackDuration > 0)
        {
            return;
        }
        //Begin knockback by setting the necessary values
        knockbackSpeed = speed;
        knockbackDuration = duration;

    }
}
