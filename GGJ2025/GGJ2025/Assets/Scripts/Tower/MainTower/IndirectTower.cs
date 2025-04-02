using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectTower : BaseTower
{

    [SerializeField] protected FieldOfView fieldOfView;

    [SerializeField] private ParticleSystem burstEffect;

    [SerializeField] protected GameObject targetedEnemy;

    // Uses the collision to detect if the enemy is in the area then use the field of view script to check if its in te cone area

    private void Awake()
    {
        fieldOfView = gameObject.GetComponentInChildren<FieldOfView>();
    }

    /// <summary>
    /// When ennemies step within the FOV or the detection area of the tower, any enemies within the FOV will be damaged
    /// </summary>
    /// <param name="collision">Just used to see what is in the radius of the player</param>
    protected override void Attack(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            if (targetedEnemy == null)
            {
                targetedEnemy = collision.gameObject;
            }
            else if (targetedEnemy != collision.gameObject)
            {
                targetedEnemy = collision.gameObject;
            }

            if (targetedEnemy == collision.gameObject)
            {
                fieldOfView.gameObject.transform.LookAt(collision.transform.position);
                burstEffect.gameObject.transform.LookAt(collision.transform.position);

                if (fieldOfView.canSeeTarget)
                {
                    for (int x = 0; x < fieldOfView.targetRefs.Count; x++)
                    {
                        DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, fieldOfView.targetRefs[x].gameObject, 0.25f, DamageType.None));
                    }

                }
            }
        }

        if (targetedEnemy != null && !targetedEnemy.activeSelf)
        {
            targetedEnemy = null;
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
