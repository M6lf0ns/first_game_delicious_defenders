using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete]
public class DaggerBehavior : ProjectileWeaponBehavior
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime; //Set movement of the dagger, s = v * t * d
    }

}
