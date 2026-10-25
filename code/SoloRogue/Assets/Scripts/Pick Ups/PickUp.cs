using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public float lifespan = 0.5f;
    protected PlayerStats target; // If the pickup has a target, then fly towards the target.
    protected float speed; // The speed at which the pickup travels.

    Vector2 initialPosition;
    float initialOffset;

    // To represent the bobbing animation of the object.
    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }
    public BobbingAnimation bobbingAnimation = new BobbingAnimation
    {
        frequency = 2f,
        direction = new Vector2(0, 0.3f)
    };

    [Header("Bonuses")]
    public int experience;
    public int health;

    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialOffset = UnityEngine.Random.Range(0, bobbingAnimation.frequency);

    }

    protected virtual void Update()
    {
        if (target)
        {
            // Move it towards the player and check the distance between.
            //Vector from the item to the target, will be updated every frame
            Vector2 distance = target.transform.position - transform.position;
            //Check if the distance from the item to the player is greater than the amount of distance the item will be travelling in this frame
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                //If so, item has yet to reach player, push gem towards player by normalizing distance vector and multiplying by distance that it would travel in that frame
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                //If distance is smaller than distance that item would travel in this frame, then item has reached player, so destroy
                Destroy(gameObject);

        }
        else
        {
            // Handle the animation of the object, make the bobbing different for every item
            transform.position = initialPosition + bobbingAnimation.direction * Mathf.Sin((Time.time + initialOffset) * bobbingAnimation.frequency);
        }
    }

    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            this.target = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan; //Ensures there is a valid lifespan and accounts for custom lifespans across different objects
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan)); //Pick up will exist for at least one frame before being destroyed
            return true;
        }
        return false;
    }



    protected virtual void OnDestroy() //Virtual for different types of pickups in the future
    {
        if (!target) return; //In case that player dies before item reaches them
        if (experience != 0) target.IncreaseExperience(experience); //Award player with exp 
        if (health != 0) target.RestoreHealth(health); //Give player health before item is destroyed
    }

}