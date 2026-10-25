using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete("Will be replaced by Weapon Data")]
[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
   [SerializeField]
   GameObject prefab;
   public GameObject Prefab { get => prefab; private set => prefab = value; } //no one else should be able to set any values, and it shouldn´t be possible to set a value on accident
   //Base stats for weapons

   [SerializeField]
   float damage;
   public float Damage { get => damage; private set => damage = value; }

   [SerializeField]
   float speed;
   public float Speed { get => speed; private set => speed = value; }

   [SerializeField]
   float cooldownDur;
   public float CooldownDur { get => cooldownDur; private set => cooldownDur = value; }

   [SerializeField]
   int pierce;
   public int Pierce { get => pierce; private set => pierce = value; }

   [SerializeField]
   int level; //not to modify in game, only in Editor
   public int Level { get => level; private set => level = value; }

   [SerializeField]
   GameObject nextLevelPrefab; //Prefab of the next level, i.e. what it becomes after leveling up
   //Not to be confused with the prefab to be spawned at the next level
   public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }

   [SerializeField]
   new string name;
   public string Name { get => name; private set => name = value; }

   [SerializeField]
   string description; //if weapon is an upgrade, place description of upgrade here
   public string Description { get => description; private set => description = value; }


   [SerializeField]
   Sprite icon; //Not to be modified during runtime
   public Sprite Icon { get => icon; private set => icon = value; }
}

