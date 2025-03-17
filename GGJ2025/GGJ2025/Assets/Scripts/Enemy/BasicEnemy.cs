using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using ObjectPoolings;

[RequireComponent(typeof(NavMeshAgent))]
public class BasicEnemy : BaseEnemy
{
    [Header("Basic Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;
    public DamageType attackDamageType = DamageType.None;
    private bool canAttack = true;

    [Header("Obstacle Destruction Settings")]
    [Tooltip("Checks times/sec to see if there's a tower blocking the path")]
    public int destructibleObjectCheckRate = 10;
    [Tooltip("Distance of raycast")]
    public float checkDistance = 1f;
    [Tooltip("Time between each attack")]
    public float destructibleAttackDelay = 1f;
    [Tooltip("Damage dealt each time to a tower used as an obstacle")]
    public int destructibleAttackDamage = 10;
    [Tooltip("Layer considered destructible obstacles")]
    public LayerMask destructibleLayers;

    // Coroutines for checking and attacking obstacles
    private Coroutine checkCoroutine;
    private Coroutine obstacleAttackCoroutine;

    public enum AdvancedEnemyState { Chase, Destroy }
    public AdvancedEnemyState extendedState = AdvancedEnemyState.Chase;

    protected override void Start()
    {
        base.Start();

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;

        // Start a coroutine to regularly check for a blocking tower in our path
        if (checkCoroutine != null) StopCoroutine(checkCoroutine);
        checkCoroutine = StartCoroutine(CheckForBlockingTower());
    }

    protected override void Update()
    {
        // BaseEnemy handles target selection in EnemyPathFinding()
        base.Update();

        // Move toward the target and try normal attacks if the enemy is in chase state
        if (extendedState == AdvancedEnemyState.Chase)
        {
            if (currentTarget != null && agent.enabled)
            {
                agent.SetDestination(currentTarget.position);
                TryAttack();
            }
        }

        // Reset attack animation when finished
        if (animator &&
            animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|ArmatureAction 0") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("IsAttack", false);
        }
    }


    /// <summary>
    /// Continuously checks if the path are blocked by a BaseTower on destructibleLayers
    /// </summary>
    private IEnumerator CheckForBlockingTower()
    {
        yield return null;
        WaitForSeconds wait = new WaitForSeconds(1f / destructibleObjectCheckRate);

        // only check the current position to the next target position
        Vector3[] corners = new Vector3[2];

        while (true)
        {
            if (extendedState == AdvancedEnemyState.Chase && agent.enabled)
            {
                // Retrieve corners of the current path
                int cornerCount = agent.path.GetCornersNonAlloc(corners);
                if (cornerCount > 1)
                {
                    Vector3 start = corners[0];
                    Vector3 end = corners[1];
                    Vector3 direction = (end - start).normalized;

                    // Raycast forward from the first corner to detect a tower
                    if (Physics.Raycast(start, direction, out RaycastHit hit, checkDistance, destructibleLayers))
                    {
                        Debug.DrawRay(start, direction * hit.distance, Color.red);

                        BaseTower blockingTower = hit.collider.GetComponentInParent<BaseTower>();

                        // If not found in the parent, check children
                        if (blockingTower == null)
                        {
                            blockingTower = hit.collider.GetComponentInChildren<BaseTower>();
                        }


                        if (blockingTower != null)
                        {
                            SwitchToDestroyState(blockingTower);
                        }
                    }
                    else
                    {
                        Debug.DrawRay(start, direction * checkDistance, Color.green);
                    }
                }
            }
            yield return wait;
        }
    }

    /// <summary>
    /// Switches the enemy to the Destroy state, stops the agent, and begins attacking the tower
    /// </summary>
    private void SwitchToDestroyState(BaseTower blockingTower)
    {
        extendedState = AdvancedEnemyState.Destroy;
        agent.isStopped = true;

        // Stop any previous obstacle-attack coroutine
        if (obstacleAttackCoroutine != null)
        {
            StopCoroutine(obstacleAttackCoroutine);
        }
        obstacleAttackCoroutine = StartCoroutine(AttackBlockingTower(blockingTower));
    }

    /// <summary>
    /// Attacks the blocking tower until it is destroyed or becomes inactive,
    /// then reverts to Chase state
    /// </summary>
    private IEnumerator AttackBlockingTower(BaseTower tower)
    {
        WaitForSeconds wait = new WaitForSeconds(destructibleAttackDelay);

        while (tower != null && tower.gameObject.activeSelf)
        {
            if (animator != null)
                animator.SetBool("IsAttack", true);

            // Deal damage to the tower
            tower.TakeDamage(destructibleAttackDamage);

            // Wait for the delay before hitting again
            yield return wait;
        }

        // The tower is destroyed or inactive, so revert to chase
        HandleObstacleDestroyed();
    }

    /// <summary>
    /// Resumes chasing the main target once the blocking tower is destroyed
    /// </summary>
    private void HandleObstacleDestroyed()
    {
        if (obstacleAttackCoroutine != null)
        {
            StopCoroutine(obstacleAttackCoroutine);
            obstacleAttackCoroutine = null;
        }

        // Re-enable pathfinding
        agent.isStopped = false;

        extendedState = AdvancedEnemyState.Chase;
    }


    /// <summary>
    /// Checks if we are in range to attack the current target
    /// </summary>
    protected void TryAttack()
    {
        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance <= attackRange && canAttack)
        {
            AttackCurrentTarget();
        }
    }

    /// <summary>
    /// Attacks the target with standard damage logic.
    /// </summary>
    private void AttackCurrentTarget()
    {
        if (animator != null)
            animator.SetBool("IsAttack", true);

        canAttack = false;

        // If the target is the player
        if (currentTarget == player)
        {
            if (player.TryGetComponent<PlayerController>(out PlayerController playerController))
            {
                DamageInfo damageInfo = new DamageInfo(
                    creator: gameObject,
                    target: player.gameObject,
                    damage: attackDamage * Stats.AtkMultiplier,
                    damageType: attackDamageType
                );
                DamageManager.Instance.ManageDamage(damageInfo);
            }
        }
        else
        {
           
        }

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void ResetAttack()
    {
        canAttack = true;
    }
}