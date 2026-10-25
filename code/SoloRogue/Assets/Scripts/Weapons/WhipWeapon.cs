using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : ProjectileWeapon
{

    int currentSpawnCount; // How many times the whip has been attacking in this iteration.
    float currentSpawnYOffset; // If there are more than 2 whips, we will start offsetting it upwards.
    ParticleSystemRenderer sr;

    ParticleSystem pS;

    Transform ownerTransform;
    protected override void Start()
    {
        base.Start();
        ownerTransform = owner.GetComponent<Transform>();
        sr = data.baseStats.hitEffect.GetComponent<ParticleSystemRenderer>();
        pS = data.baseStats.hitEffect.GetComponent<ParticleSystem>();

    }

    protected override void Update()
    {
        base.Update();


    }


    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, leave a warning message.
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));

            currentCooldown = data.baseStats.cooldown;
            return false;
        }
        //Debug.Log("Attack attempt");
        //Debug.Log(CanAttack());
        // If there is no projectile assigned, set the weapon on cooldown.
        if (!CanAttack())
        {
            Debug.Log("Attack fired");
            return false;
        }


        // If this is the first time the attack has been fired,
        // we reset the currentSpawnCount.
        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0f;
        }

        // Otherwise, calculate the angle and offset of our spawned projectile.
        // Then, if <currentSpawnCount> is even (i.e. more than 1 projectile),
        // we flip the direction of the spawn.
        float spawnDir = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        //Debug.Log("Spawn direction " + spawnDir);
        Vector2 spawnOffset = new Vector2(
            spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            currentSpawnYOffset
            );
        //Debug.Log(spawnOffset);
        //Debug.Log("spawnAngle " + GetSpawnAngle());


        // And spawn a copy of the projectile.
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)spawnOffset,
            Quaternion.identity

        );

        //Debug.Log("prefab spawned" + prefab.name);
        prefab.owner = owner; // Set ourselves to be the owner.


        // Flip the projectile's sprite.
        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(
                Mathf.Abs(prefab.transform.localScale.x),
                prefab.transform.localScale.y,
                prefab.transform.localScale.z);
            //sr.flip = new Vector3(-1, 0, 0);



            Debug.Log(spawnDir + " | " + prefab.transform.localScale);
        }
        else if (spawnDir > 0)
        {
            prefab.transform.localScale = new Vector3(
                -Mathf.Abs(prefab.transform.localScale.x),
                prefab.transform.localScale.y,
                prefab.transform.localScale.z);
        }


        // Assign the stats.
        prefab.weapon = this;
        currentCooldown = data.baseStats.cooldown;
        attackCount--;

        // Determine where the next projectile should spawn.
        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
            currentSpawnYOffset += 1;

        // Do we perform another attack?
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }


}
