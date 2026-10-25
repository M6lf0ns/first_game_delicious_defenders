using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete]
//Base script of all meele behaviors [To be placed on a prefab of a weapon that is meele]
public class MeeleWeaponBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float destroyAfterSecs;
    //Current stats
    protected float currentDamage;
    protected float currentSpeed;
    protected float currentCooldownDur;
    protected int currentPierce;
    void Awake()
    {
        currentDamage = weaponData.Damage; //why not use Getter?
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

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position);

        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());

            }
        }
    }
}