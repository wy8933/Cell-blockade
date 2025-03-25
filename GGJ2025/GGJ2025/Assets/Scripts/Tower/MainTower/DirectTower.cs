using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class DirectTower : BaseTower
{

    [SerializeField] private GameObject LaserHolder;

    [SerializeField] protected GameObject targetedEnemy;

    [SerializeField] private VisualEffect laserEffect;

    private void Awake()
    {
        //laserEffect.SetVector4("LaserColor", new Vector4(255, 0, 255, 1));
        LaserHolder.SetActive(false);
    }

    /// <summary>
    /// An override of the Attack method that takes a collision as an input
    /// checks if a target is within the detection radius
    /// </summary>
    /// <param name="collision"></param>
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
                DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, collision.gameObject, 1, DamageType.None));
            }
           
            //Debug.Log("ITs in the area");
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
        //Debug.Log(target);
        if (target.tag == "Enemy")
        {
            if (target != null)
            {

                if (targetedEnemy == target)
                {
                    LaserHolder.SetActive(true);
                    LaserHolder.transform.LookAt(new Vector3(target.transform.position.x, (target.transform.position.y + 1.0f), target.transform.position.z));
                }
                //Debug.Log(target.transform.position);
            }
        }
        else if (targetedEnemy == null)
        {
            LaserHolder.SetActive(false);
        }
    }
}