using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
[System.Obsolete]
public class DaggerController : WeaponController
{
    //protected override for all child classes
    protected override void Start()
    {
        base.Start();
    }


    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedDagger = Instantiate(weaponData.Prefab); //when Attack is called, spawn a dagger

        spawnedDagger.transform.position = transform.position; //position of the spawned dagger set to be that of the object
                                                               //that this script is assigned to (Dagger Controller), which is parented by the player

        spawnedDagger.GetComponent<DaggerBehavior>().DirectionChecker(pm.lastMovedVector); //reference the direction in which the player is moving
        //give the direction checker function the player´s movedir, which the variable in the behavior script will be set to
        //sets the direction in which the knife is supposed to go
    }

}
