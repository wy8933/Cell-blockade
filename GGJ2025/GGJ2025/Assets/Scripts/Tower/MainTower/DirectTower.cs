using UnityEngine;

public class DirectTower : BaseTower
{

    protected LineRenderer laserAttack;

    protected override void Attack(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, collision.gameObject, 10, DamageType.None));
        }
    }
}
