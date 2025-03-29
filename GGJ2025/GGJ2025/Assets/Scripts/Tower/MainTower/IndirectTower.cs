using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectTower : BaseTower
{

    [SerializeField] protected FieldOfView fieldOfView;

    [SerializeField] private ParticleSystem burstEffect;

    // Uses the collision to detect if the enemy is in the area then use the field of view script to check if its in te cone area

    /// <summary>
    /// When ennemies step within the FOV or the detection area of the tower, any enemies within the FOV will be damaged
    /// </summary>
    /// <param name="collision">Just used to see what is in the radius of the player</param>
    protected override void Attack(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            if (fieldOfView.canSeeTarget)
            {
                for (int x = 0; x < fieldOfView.targetRefs.Count; x++)
                {
                    DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, fieldOfView.targetRefs[x].gameObject, 10, DamageType.None));
                }
                
            }
            
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    protected override void ShowAttack(GameObject source, GameObject target)
    {
        if (fieldOfView.canSeeTarget)
        {
            burstEffect.Play();
        }
        else
        {
            burstEffect.Stop();
        }
    }



}
