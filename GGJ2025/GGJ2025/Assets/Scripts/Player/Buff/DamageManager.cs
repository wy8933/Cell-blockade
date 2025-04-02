using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public static DamageManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void ManageDamage(DamageInfo damageInfo) { 
        BuffHandler creatorBuffHandler = damageInfo.creator?.GetComponent<BuffHandler>();
        BuffHandler targetBuffHandler = damageInfo.target?.GetComponent<BuffHandler>();

        if (creatorBuffHandler) {
            foreach (var buffInfo in creatorBuffHandler.ActiveBuffs) {
                buffInfo.target = targetBuffHandler.gameObject;
                
                if (buffInfo.buffData.OnHit) {
                    if (buffInfo.buffData.toPlayer)
                    {
                        BuffInfo tempBuffInfo = buffInfo;
                        tempBuffInfo.target = GameManager.Instance.Player.gameObject;
                        buffInfo.buffData.OnHit.Apply(tempBuffInfo, damageInfo);
                    }
                    else 
                    {
                        buffInfo.buffData.OnHit.Apply(buffInfo, damageInfo);
                    }
                }
                
            }
        
        }
        if (targetBuffHandler)
        {
            foreach (var buffInfo in targetBuffHandler.ActiveBuffs)
            {
                if (buffInfo.buffData.OnHurt) {
                    buffInfo.buffData.OnHurt.Apply(buffInfo, damageInfo);
                }
            }

            // Deal damage to entity 
            if (damageInfo.target.tag == "Enemy") {
                var enemy = damageInfo.target.GetComponent<BaseEnemy>();
                if (enemy) { 
                    enemy.TakeDamage(damageInfo.damage);

                }
            }
            else if (damageInfo.target.tag == "Player")
            {
                var player = damageInfo.target.GetComponent<PlayerController>();

                if (player)
                {
                    player.TakeDamage(damageInfo.damage);
                }
            }
        }
    }
}
