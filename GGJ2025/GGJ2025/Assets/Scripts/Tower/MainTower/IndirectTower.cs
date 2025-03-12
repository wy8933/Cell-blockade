using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectTower : BaseTower
{
    [SerializeField] protected GameObject targetedEnemy;

    [SerializeField] protected List<GameObject> targetedEnemies;

    [SerializeField] protected FieldOfView FieldOfView;

    private void Start()
    {
        
    }

    // Uses the collision to detect if the enemy is in the area then use the field of view script to check if its in te cone area
    protected override void Attack(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            if ()
            {

            }

            if (FieldOfView.canSeeTarget)
            {
                DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, collision.gameObject, 10, DamageType.None);
            }

            
        }

    }

    protected override void ShowAttack(GameObject source, GameObject target)
    {

    }



}
