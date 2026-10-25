using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete]
//Base Script for all weapon controllers
//Parent class that makes sure projectile is fired at the correct time after cooldown is over 
//And reset cooldown

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public WeaponScriptableObject weaponData;
    float currCooldown;
    //maximum amount of time a weapon can hit an enemy before it gets destroyed
    protected PlayerMovement pm; //so all child classes can use it too

    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        currCooldown = weaponData.CooldownDur; //ensure that weapon doesnt shoot immediately when Player gets it
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        currCooldown -= Time.deltaTime;
        if (currCooldown <= 0f) //Once cooldown becomes 0, attack
        {
            Attack();
        }
    }
    protected virtual void Attack()
    {
        currCooldown = weaponData.CooldownDur; //Once attacked, resets cooldown, creating an infinite loop
    }
}
