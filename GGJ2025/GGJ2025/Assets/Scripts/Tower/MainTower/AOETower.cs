using UnityEngine;
using UnityEngine.VFX;

public class AOETower : BaseTower
{

    [SerializeField] private ParticleSystem AOEAttackEffect;

    [SerializeField] protected FieldOfView fieldOfView;

    private void Awake()
    {
        fieldOfView = gameObject.GetComponentInChildren<FieldOfView>();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    protected override void Attack(Collider collision)
    {
        if (collision.tag == "Enemy")
        {
            if (fieldOfView.canSeeTarget)
            {
                for (int x = 0; x < fieldOfView.targetRefs.Count; x++)
                {
                    DamageManager.Instance.ManageDamage(new DamageInfo(gameObject, fieldOfView.targetRefs[x].gameObject, 2f, DamageType.None));
                }

            }
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    protected override void ShowAttack(GameObject source, GameObject target)
    {
        if (fieldOfView.canSeeTarget)
        {
            AOEAttackEffect.Play();
        }
        else
        {
            AOEAttackEffect.Stop();
        }
    }
}
