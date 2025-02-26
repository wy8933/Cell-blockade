using UnityEngine;

public class DirectTower : BaseTower
{

    protected LineRenderer laserAttack;

    protected LineRenderController laserControl;

    private void Awake()
    {
        laserControl = GetComponent<LineRenderController>();
    }

    protected override void Attack(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, collision.gameObject, 10, DamageType.None));
        }
    }

    protected override void ShowAttack(GameObject source, GameObject target)
    {
        
    }
}
