using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Timeline;

/// <summary>
///  Component that will be attached to all Weapon prefabs. 
/// Prefabs along with WeaponData SO´s will manage and run all behaviors of all weapons in game
/// All functions can be overriden by child classes
/// </summary> <summary>

public abstract class Weapon : Item //cannot attach this to a GO
{

    [System.Serializable]
    public struct Stats //allows for creating variables that contain other variables
    {

        public string name;
        public string description;


        [Header("Visuals")]
        public Projectile projectilePrefab; //If attached, a projectile will spawn every time the weapon cools down
        //public Aura auraprefab //If attached, an aura will spawn when weapon is equipped
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; //how long weapon lasts before destroying itself, if 0, it will last forever?
        public float damage, damageVariance, area /*area of effect*/, speed, cooldown, projectileInterval, knockback;
        public int number, pierce, maxInstances;

        //Allows the use of the + operator to add two Stats 
        //Important for increasing weapon stats
        public static Stats operator +(Stats s1, Stats s2) //Instructs how to add two objects of type stat
        {
            Stats result = new Stats();
            result.name = s2.name ?? s1.name; //gives value(name) of s2 if not null, otherwise gives value of s1
            result.description = s2.description ?? s1.description;
            result.projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab;
            //result.auraPrefab = s2.auraPrefab ?? s1.auraPrefab;
            result.hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect; //If s2 doesnt have a hit effect, choose the hit effect of s1, if not, choose s2´s hit effect
            result.spawnVariance = s2.spawnVariance; //value of s1 gets overwritten, makes the spawning of a projectile more random
            result.lifespan = s1.lifespan + s2.lifespan; //lifespan increases with increasing levels
            result.damage = s1.damage + s2.damage;  //same thing
            result.damageVariance = s1.damageVariance + s2.damageVariance; //""
            result.area = s1.area + s2.area;
            result.speed = s1.speed + s2.speed;
            result.cooldown = s1.cooldown + s2.cooldown;
            result.number = s1.number + s2.number;
            result.pierce = s1.pierce + s2.pierce;
            result.projectileInterval = s1.projectileInterval + s2.projectileInterval;
            result.knockback = s1.knockback + s2.knockback;
            return result; //This is where all of the changes are saved in
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance); //ensures that damage inflicted isnt always the same 
        }

    }

    protected Stats currentStats;

    public WeaponData data;

    protected float currentCooldown;

    protected PlayerMovement movement;

    //For dynamically created weapons call init to set everything up
    public virtual void Initialise(WeaponData data)
    {
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        currentCooldown = currentStats.cooldown;
    }

    protected virtual void Awake()
    {
        //Assign stats early because itll be used by othe scripts later
        if (data)
        {
            currentStats = data.baseStats;
        }
    }

    protected virtual void Start()
    {
        //Dont initialise weapon if weapondata is not assigned
        if (data)
        {
            Initialise(data);
            Debug.Log("data initialised");
        }
        else
        { Debug.LogWarning("NO WEAPONDATA ASSIGNED"); }
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f) //once cooldown over, attacc
        {
            Attack(currentStats.number);
        }
    }

    //Levels up weapon by 1 and calculates corresponding stats
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        //Prevent level up if already at max level
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cant level up {0} to level {1}, max level of {2} already reached", name, currentLevel, data.maxLevel));
            return false;
        }

        //Otherwise, add stats of the next level to our weapon
        currentStats += data.GetLevelData(++currentLevel);
        return true;

    }

    //Allows check of if weapon can attack atm
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0f;
    }


    //Performs an attack with the weapon
    //Returns true if attack successful
    //Doesnt actually attack, will be overriden in the child class to add behavior
    protected virtual bool Attack(int attackCount = 1) //some weapons might fire multiple times
    {
        if (CanAttack())
        {
            currentCooldown += currentStats.cooldown; //increase cooldown back to what it was
            return true; //Attack successful

        }
        return false; //Attack unsuccessful
    }

    //Gets amount of dmg that weapon is supposed to deal
    //Considering weapon´s Stats (like damageVariance) as well as might
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.CurrentMight;
    }

    //For retrieving the weapon´s stats
    public virtual Stats GetStats()
    {
        return currentStats;
    }

}
