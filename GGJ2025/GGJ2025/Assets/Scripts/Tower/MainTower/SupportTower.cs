using UnityEngine;

public class SupportTower : BaseTower
{

    [SerializeField] protected GameObject SupportAOE;

    [SerializeField] protected GameObject targetedAlly;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    protected override void Attack(Collider collision)
    {
        //Debug.Log(collision.tag);

        if (collision.tag == "Player" || collision.tag == "Ally")
        {

            if (targetedAlly == collision.gameObject)
            {
                DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, collision.gameObject, 10, DamageType.None));
            }

            Debug.Log("ITs in the area");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    protected override void ShowAttack(GameObject source, GameObject target)
    {
        //Debug.Log(target);
        if (target.tag == "Enemy")
        {
            if (target != null)
            {

                if (targetedAlly == target)
                {
                    SupportAOE.transform.LookAt(target.transform.position);
                }


                //Debug.Log(target.transform.position);
            }
        }

    }
}
