using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>
/// Component that gets attached to all projectile prefabs. 
/// All spawned projectiles will fly in the direction they´re facing and deal damage when hitting an object
/// </summary> 

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    protected Rigidbody2D rb;
    protected int pierce;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {

            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed;
        }


        //Prevent the area from being 0, as it hides the projectile
        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new Vector3(area * Mathf.Sign(transform.localScale.x),
        area * Mathf.Sign(transform.localScale.y), 1);
        //Debug.Log("area " + area);

        //Set the pierce value of this object
        pierce = stats.pierce;

        //Destroy the projectile after its lifespan expires
        //Debug.Log("lifespan " + stats.lifespan);
        if (stats.lifespan > 0)
        {
            Destroy(gameObject, stats.lifespan);
        }

        //If the projectile has AutoAim, find a suitable target (enemy) 
        if (hasAutoAim)
        {
            AcquireAutoAimFacing();

        }

    }


    //if projectile is homing, it will automatically find suitable target to move towards
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle; //Determines where to aim

        //Find all enemies on screen
        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        //Select random enemy (if there is >= 1)
        //Otherwise, pick random angle

        if (targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[UnityEngine.Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        }
        else
        {
            aimAngle = UnityEngine.Random.Range(0f, 360f);
        }

        //Point the projectile towards where we are aiming at
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);


    }

    protected virtual void FixedUpdate()
    {

        //Only drive movement ourselves if this is a kinematic
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);


            // Check if the projectile is facing left
            if (transform.right.x < 0)
            {

                transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
            }

        }
    }


    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        //Only collide with enemies or stuff that breaks
        if (es)
        {
            //If there is owner and damage source is owner
            //Calculate knockback using owner instead of projectile
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            //Deals damage and destroys projectile
            es.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();
            pierce--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }

        }
        else if (p) //If its breakable prop
        {
            p.TakeDamage(GetDamage());
            pierce--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect.transform, transform.position, Quaternion.identity), 5f);
            }
        }

        //Destroy this object if it has run out of health from hitting something else
        if (pierce <= 0)
        {
            Destroy(gameObject);
        }

    }

}








