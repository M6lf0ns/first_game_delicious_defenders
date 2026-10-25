using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class for both Passive and Weapon Classes. Intended to handle weapon evolution, 
/// since both should be able to be evolved
/// </summary> 
public abstract class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;

    protected PlayerStats owner;

    public virtual void Initialise(ItemData data)
    {
        maxLevel = data.maxLevel;
        owner = FindObjectOfType<PlayerStats>();


    }


    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;

    }

    //Whenever an item levels up. attempt to make it evolve
    public virtual bool DoLevelUp()
    {
        //Weapon Evolution logic will go here later
        return true;
    }

    //What effects you receive on equipping an item
    public virtual void OnEquip()
    {

    }

    public virtual void OnUnequip()
    {

    }


}
