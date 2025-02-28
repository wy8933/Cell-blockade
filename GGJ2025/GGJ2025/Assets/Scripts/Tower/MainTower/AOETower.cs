using UnityEngine;
using UnityEngine.VFX;

public class AOETower : BaseTower
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

        if (targetedEnemy != null && !targetedEnemy.activeSelf)
        {
            targetedEnemy = null;
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
