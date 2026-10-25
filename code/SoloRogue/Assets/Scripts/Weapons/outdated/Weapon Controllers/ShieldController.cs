using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete]
public class ShieldController : WeaponController
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedShield = Instantiate(weaponData.Prefab);
        spawnedShield.transform.position = transform.position; //assign position to be that of this object, which is parented to player
        spawnedShield.transform.parent = transform; //so that it spawns below this object
    }
}
