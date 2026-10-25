using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Replaces WeaponSO class. Serves the purpose of storing all weapon evolution data
/// in a single object, instead of having multiple objects store one weapon like it is with WeaponSO class
/// </summary>

[CreateAssetMenu(fileName = "Weapon Data", menuName = "2D Top-Down Rogue-like/WeaponData")]
public class WeaponData : ItemData
{

    [HideInInspector]
    public string behavior; //contains name of script that weapon uses to control its beahvior
    public Weapon.Stats baseStats; //before any upgrade
    public Weapon.Stats[] linearGrowth; //array of Stats that contain information of how the weapons grows, should contain Levels -1 Elements, because level 1 is in basestats

    public Weapon.Stats[] randomGrowth; //is intended for limit break feature, maybe delete later

    //Gives stat growth/description of next level
    public Weapon.Stats GetLevelData(int level)
    {

        //Pick stats from next level
        if (level - 2 < linearGrowth.Length) //since we start at level 2 and array starts at 0
        {
            return linearGrowth[level - 2];
        }

        //Otherwise, pick one of stats from random growth
        if (randomGrowth.Length > 0)
        {
            return randomGrowth[Random.Range(0, randomGrowth.Length)];
        }

        //Return an empty value and a warning

        Debug.LogWarning(string.Format("Weapon doesn´t have level up stats configured for lvl {0}", level)); //if there are not enough entries in lineargrowth and none in randomgrowth
        return new Weapon.Stats();
    }
}
