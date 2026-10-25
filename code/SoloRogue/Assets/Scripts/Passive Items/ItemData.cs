using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base Class for all weapons/passive items.
/// Is used so that both WeaponData and PassiveItemData can be used interchangeably
/// </summary> 
public abstract class ItemData : ScriptableObject
{
    public Sprite icon;
    public int maxLevel;
}
