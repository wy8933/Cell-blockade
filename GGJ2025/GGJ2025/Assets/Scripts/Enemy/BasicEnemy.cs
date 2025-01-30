using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using ObjectPoolings;

public class BasicEnemy : BaseEnemy
{
    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public DamageType attackDamageType = DamageType.None;
    private bool canAttack = true;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        EnemyPathFinding();
        TryAttack();
    }

    /// <summary>
    /// Check if player is in range and can be attacked
    /// </summary>
    protected void TryAttack()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && canAttack)
        {
            Attack();
        }
    }

    /// <summary>
    /// Play the attack animation and attack the player
    /// </summary>
    private void Attack()
    {
        animator.SetBool("IsAttack", true);
        canAttack = false;

        // Check if player exist and deal damage to player
        if (player.TryGetComponent(out PlayerController playerController))
        {
            DamageInfo damageInfo = new DamageInfo(gameObject, player.gameObject, attackDamage * Stats.AtkMultiplier, attackDamageType);
            DamageManager.Instance.ManageDamage(damageInfo);
        }

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    /// <summary>
    /// Reset attack state
    /// </summary>
    private void ResetAttack()
    {
        canAttack = true;
    }
}
