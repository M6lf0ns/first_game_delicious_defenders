using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] //can prevent errors and avoid the use of Game Objects without SR
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    //Current Stats
    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;
    public float despawnDistance = 20f;
    Transform player;

    [Header("Dmg Feedback")]

    public Color dmgColor = new Color(1, 0, 0, 1);
    public float dmgFlashDuration = 0.2f; //How long color change lasts
    public float deathFadeTime = 0.6f; //How long till enemy fades after dying
    Color originalColor; //Color that sprite is normally
    SpriteRenderer sr;
    BoxCollider2D col;
    EnemyMovement movement;

    void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        movement = GetComponent<EnemyMovement>();
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }

    }
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash()); //when taking dmg --> flash

        if (knockbackForce > 0) //If force is still being applied 
        {
            //Get direction of knockback, Spitze - Fuß, erzeugt einen Vektor, der von Projektil zum Gegner geht
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration); //Calls Knockback and gives it speed and duration
        }


        if (currentHealth <= 0)
        {
            Kill();
        }

    }



    //Coroutine that creates flash when enemy takes dmg

    IEnumerator DamageFlash()
    {
        sr.color = dmgColor;
        yield return new WaitForSeconds(dmgFlashDuration); //wait the specified amount of time before continuing Coroutine
        sr.color = originalColor; //Set color back to the original
    }
    public void Kill()
    {
        //test
        col.enabled = false;
        StartCoroutine(KillFade());

    }

    IEnumerator KillFade() //makes enemy fade away when killed
    {
        //Waits for one frame since color should fade smoothly
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float time = 0, //tracks for how long Coroutine has been running
        originalAlpha = sr.color.a;

        //while deathFade is still in progress
        while (time < deathFadeTime)
        {
            yield return w; //wait until end of frame to continue routine
            time += Time.deltaTime; //then increase time

            //Set color for current frame
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - time / deathFadeTime) * originalAlpha); //the bigger time gets, the more transparent the enemy gets
        }
        Destroy(gameObject);
    }

    /*void OnCollisionStay2D(Collision2D col)
    {
        //Reference script from collided collider and deal damage using TakeDamage()
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage); //use currentDamage instead of weaponData.damage in case of any damage multipliers in the future
        }
    }*/

    void OnTriggerStay2D(Collider2D col)
    {
        //Reference the script from the collided collider and deal damage using TakeDamage()
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage); // Make sure to use currentDamage instead of weaponData.Damage in case any damage multipliers in the future
        }
    }

    private void OnDestroy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        es.OnEnemyKilled();
    }

    void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[UnityEngine.Random.Range(0, es.relativeSpawnPoints.Count)].position;

    }

}
