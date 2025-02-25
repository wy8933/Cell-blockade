using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class DirectTower : BaseTower
{

    [SerializeField] private GameObject LaserHolder;

    [SerializeField] protected GameObject targetedEnemy;

    [SerializeField] private VisualEffect laserEffect;

    private void Start()
    {
        laserEffect.SetVector4("LaserColor", new Vector4(255, 0, 255, 1));

    }

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
               
                if (targetedEnemy == target)
                {
                    LaserHolder.transform.LookAt(target.transform.position);
                }
                

                //Debug.Log(target.transform.position);
            }
        }
       
    }
}
