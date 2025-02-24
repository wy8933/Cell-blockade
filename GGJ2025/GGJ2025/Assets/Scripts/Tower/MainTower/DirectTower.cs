using UnityEngine;

public class DirectTower : BaseTower
{

    [SerializeField] protected GameObject LaserHolder;

    [SerializeField] protected GameObject targetedEnemy;

    [SerializeField] protected float rotationSpeed = 100f;


    protected override void Attack(Collider collision)
    {
        //Debug.Log(collision.tag);

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
                DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, collision.gameObject, 10, DamageType.None));
            }
           
            Debug.Log("ITs in the area");
        }
    }

    protected override void ShowAttack(GameObject source, GameObject target)
    {
        //Debug.Log(target);
        if (target.tag == "Enemy")
        {
            if (target != null)
            {
                LaserHolder.transform.LookAt(target.transform.position);

                Debug.Log(target.transform.position);
            }
        }
       
    }
}
