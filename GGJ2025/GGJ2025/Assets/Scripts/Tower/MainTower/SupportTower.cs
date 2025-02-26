using UnityEngine;

public class SupportTower : BaseTower
{
    public enum DetectionMode
    {
        Nearest,
        Farthest,
        Strongest,
    }

    [SerializeField] protected GameObject LaserHolder;

    [SerializeField] protected GameObject targetedAlly;

    protected override void Attack(Collider collision)
    {
        //Debug.Log(collision.tag);

        if (collision.tag == "Player" || collision.tag == "Ally")
        {
            if (targetedAlly == null)
            {
                targetedAlly = collision.gameObject;
            }
            else if (targetedAlly != collision.gameObject)
            {
                targetedAlly = collision.gameObject;
            }

            if (targetedAlly == collision.gameObject)
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

                if (targetedAlly == target)
                {
                    LaserHolder.transform.LookAt(target.transform.position);
                }


                //Debug.Log(target.transform.position);
            }
        }
        else
        {
            LaserHolder.transform.LookAt(target.transform.position);
        }

    }
}
