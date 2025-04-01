using UnityEngine;
[CreateAssetMenu(fileName = "DamageModule", menuName = "BuffSystem/DamageModule")]
public class DamageModule : BaseBuffModule
{
    public float damage;

    public override void Apply(BuffInfo buffInfo, DamageInfo damageInfo = null)
    {
        PlayerController player = buffInfo.target.GetComponent<PlayerController>();
        if (player)
            player.TakeDamage(damage);
        else { 
            BaseEnemy enemy = buffInfo.target.GetComponent<BaseEnemy>();
            enemy.TakeDamage(damage);
        }
    }
}
