using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// GO that is spawned as an effect of weapon firing, for example projectiles, auras, pulses
/// Avoids repetition when creating a new effect
/// </summary> 
public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector]
    public PlayerStats owner; //Character to whom the weapon belongs

    [HideInInspector]
    public Weapon weapon; //Weapon that the effect corresponds to

    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}
