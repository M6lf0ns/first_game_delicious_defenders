using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Obsolete]
public class ShieldBehavior : MeeleWeaponBehavior
{
    List<GameObject> markedEnemies;
    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position); //Save position for knockback
            markedEnemies.Add(col.gameObject); //Mark the enemy so it doesnt take another instance of damage from this GO, remember this, could be improved upon
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable) && !markedEnemies.Contains(col.gameObject)) //if collision with object that has BreakableProps component and is not in markedEnemies list
            {
                breakable.TakeDamage(GetCurrentDamage());

                markedEnemies.Add(col.gameObject); //treating props as enemies
            }
        }
    }

}
