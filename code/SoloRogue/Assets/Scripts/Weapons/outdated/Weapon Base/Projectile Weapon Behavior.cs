using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete]
//Base script of all projectile behaviors [To be placed on a prefab of a weapon that is a projectile]
public class ProjectileWeaponBehavior : MonoBehaviour
{
    //Thrust
    public WeaponScriptableObject weaponData;
    protected Vector3 direction; //tracks direction the weapon should face 
    public float destroyAfterSecs; //set time to destroy weapon

    //Current Stats

    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDur;
    protected int currentPierce; //how often you can attack with a weapon before it gets destroyed

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentSpeed = weaponData.Speed;
        currentCooldownDur = weaponData.CooldownDur;
        currentPierce = weaponData.Pierce;
    }

    public float GetCurrentDamage() //implementing Getter
    {
        return currentDamage * FindObjectOfType<PlayerStats>().CurrentMight; //Gets damage and modifies it
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSecs);
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir; //direction is set to the direction given to the function
        float dirx = direction.x; //getting the values
        float diry = direction.y;

        Vector3 scale = transform.localScale; //scale of transform relative to parent
        Vector3 rotation = transform.rotation.eulerAngles; //check why

        if (dirx < 0 && diry == 0) //player moves left
        {
            scale.x = scale.x * -1; //since default direction of dagger is right, we just have to set both x and y scale to -1
            scale.y = scale.y * -1;
        }
        else if (dirx < 0 && diry < 0) //left down
        {
            rotation.z = 45f;
            scale.y = scale.y * -1;
        }
        else if (dirx < 0 && diry > 0) //left up 
        {
            rotation.z = -45f;
        }
        else if (dirx > 0 && diry > 0) //right up
        {
            rotation.z = 45f;
        }
        else if (dirx > 0 && diry < 0) //right down
        {
            scale.y = scale.y * -1;
            rotation.z = -45f;
        }

        else if (dirx == 0 && diry > 0) //up
        {
            rotation.z = -90f;
        }
        else if (dirx == 0 && diry < 0) //down
        {
            rotation.z = 90f;
        }

        transform.localScale = scale; //if if was entered, so if player is facing left, daggers scale is changed, if not, it stays relative to its parent
        transform.rotation = Quaternion.Euler(rotation); //cant simply set vector because quaternion cant be converted to a vector3
    }
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        //Reference script from the collided collider and deal damage using TakeDamage()
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            //Make enemy take damage and pass in sourcePosition of projectile
            enemy.TakeDamage(GetCurrentDamage(), transform.position); //make sure to use currentDamage instead of weaponData.damage in case of any damage multipliers in the future
            ReducePierce();

        }
        else if ((col.CompareTag("Prop")))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                ReducePierce();
            }
        }


    }
    void ReducePierce()
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject);
        }
    }


}
