using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using ObjectPoolings;
using Unity.VisualScripting;

public class BasicEnemy : BaseEnemy
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public DamageType attackDamageType = DamageType.None;
    private bool canAttack = true;

    protected override void Start()
    {
        base.Start();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Update()
    {
        base.Update();
        TryAttack();
    }

    protected void TryAttack()
    {
        Vector3 targetPosition = Vector3.zero;
        if (currentFlowFieldTarget == FlowFieldTarget.Player)
        {
            if (player != null)
                targetPosition = player.position;
            else
                return;
        }
        else if (currentFlowFieldTarget == FlowFieldTarget.Core)
        {
            targetPosition = Wound.Instance.transform.position;
        }
        else
        {
            targetPosition = Wound.Instance.transform.position;
        }

        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance <= attackRange && canAttack)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (animator != null)
            animator.SetBool("IsAttack", true);
        canAttack = false;

        if (currentFlowFieldTarget == FlowFieldTarget.Player)
        {
            if (player.TryGetComponent(out PlayerController playerController))
            {
                DamageInfo damageInfo = new DamageInfo(gameObject, player.gameObject, attackDamage * Stats.AtkMultiplier, attackDamageType);
                DamageManager.Instance.ManageDamage(damageInfo);
            }
        }
        else if (currentFlowFieldTarget == FlowFieldTarget.Core)
        {
            Debug.Log("attack core");
        }

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    /// <summary>
    /// Resets the enemy's attack state
    /// </summary>
    private void ResetAttack()
    {
        canAttack = true;
    }
}
