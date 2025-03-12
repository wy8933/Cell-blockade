using ObjectPoolings;
using UnityEngine;
using UnityEngine.AI;

public enum PathfindingMode { AlwaysPlayer, AlwaysCore, Closest }
public enum EnemyAIState { Chase }

[RequireComponent(typeof(NavMeshAgent))]
public class BaseEnemy : MonoBehaviour
{
    public EnemyStats Stats;
    public Transform player;
    public PrefabPool pool;
    public bool isReleased;
    public AudioSource audioSource;
    public Animator animator;
    public float currencyAmount = 10;

    [Header("Pathfinding Settings")]
    public float steeringWeight = 1.0f;
    public float detectionRadius = 2.0f;
    public PathfindingMode pathfindingMode = PathfindingMode.AlwaysPlayer;
    protected EnemyAIState currentState = EnemyAIState.Chase;
    public FlowFieldTarget currentFlowFieldTarget = FlowFieldTarget.Player;

    protected virtual void Start()
    {
        GetComponent<NavMeshAgent>().enabled = false;
    }

    protected virtual void Update()
    {
        EnemyPathFinding();
        FlowFieldSteeringMovement();

        if (animator && animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|ArmatureAction 0") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
        {
            animator.SetBool("IsAttack", false);
        }
    }

    /// <summary>
    /// Initializes the enemy when created or released from an object pool
    /// </summary>
    public void InitEnemy(PrefabPool pool)
    {
        Stats.Health = Stats.MaxHealth;
        this.pool = pool;
        isReleased = false;
        currentState = EnemyAIState.Chase;
        currentFlowFieldTarget = FlowFieldTarget.Core;
        ApplyScaling(EnemyWaveManager.Instance.enemyIncreaseFactor, EnemyWaveManager.Instance.currentWaveIndex);
        GameManager.Instance.CurrentEnemyList.Add(gameObject);
    }

    protected virtual void EnemyPathFinding()
    {
        Transform coreTransform = Wound.Instance.transform;

        // Choose a target based on the selected pathfinding mode
        if (currentState == EnemyAIState.Chase)
        {
            switch (pathfindingMode)
            {
                case PathfindingMode.AlwaysPlayer:
                    currentFlowFieldTarget = FlowFieldTarget.Player;
                    break;
                case PathfindingMode.AlwaysCore:
                    currentFlowFieldTarget = FlowFieldTarget.Core;
                    break;
                case PathfindingMode.Closest:
                    if (player != null)
                    {
                        float dPlayer = Vector3.Distance(transform.position, player.position);
                        float dCore = Vector3.Distance(transform.position, coreTransform.position);
                        currentFlowFieldTarget = (dPlayer <= dCore) ? FlowFieldTarget.Player : FlowFieldTarget.Core;
                    }
                    else
                    {
                        currentFlowFieldTarget = FlowFieldTarget.Core;
                    }
                    break;
            }

            // Check whether the normal flow field is blocked
            Vector3 normalDirection = FlowFieldManager.Instance.GetFlowDirection(currentFlowFieldTarget, transform.position);
            if (normalDirection.sqrMagnitude < 0.001f)
            {
                // Path is completely blocked; switch to the blocked flow field
                if (currentFlowFieldTarget == FlowFieldTarget.Player)
                    currentFlowFieldTarget = FlowFieldTarget.BlockedPlayer;
                else if (currentFlowFieldTarget == FlowFieldTarget.Core)
                    currentFlowFieldTarget = FlowFieldTarget.BlockedCore;
            }
            else
            {
                // If a valid path exists and enemy are in a blocked mode, revert to the normal target
                if (currentFlowFieldTarget == FlowFieldTarget.BlockedPlayer)
                    currentFlowFieldTarget = FlowFieldTarget.Player;
                else if (currentFlowFieldTarget == FlowFieldTarget.BlockedCore)
                    currentFlowFieldTarget = FlowFieldTarget.Core;
            }
        }
    }

    protected void FlowFieldSteeringMovement()
    {
        // Get the global direction from the flow field
        Vector3 globalDirection = FlowFieldManager.Instance.GetFlowDirection(currentFlowFieldTarget, transform.position);
        Vector3 steering = CalculateSteering();
        Vector3 finalDirection = (globalDirection + steeringWeight * steering).normalized;
        transform.position += finalDirection * Stats.MovementSpeed * Time.deltaTime;
    }

    protected Vector3 CalculateSteering()
    {
        // TODO: Add local steering logic for fine-tuning movement, expand here for obstacle avoidance
        Transform target = null;
        if ((currentFlowFieldTarget == FlowFieldTarget.Player || currentFlowFieldTarget == FlowFieldTarget.BlockedPlayer) && player != null)
        {
            target = player;
        }
        else
        {
            target = Wound.Instance.transform;
        }

        return (target.position - transform.position).normalized;
    }

    /// <summary>
    /// Deal damage to the enemy and trigger death when health is 0 or below
    /// </summary>
    public void TakeDamage(float damage)
    {
        Stats.Health -= damage;
        DamageTextManager.Instance.ShowDamageText(transform.position,damage);
        if (Stats.Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Releases the enemy back to the object pool and notifies the enemy wave manager
    /// </summary>
    private void Die()
    {
        if (!isReleased)
        {
            audioSource.Play();
            isReleased = true;
            EnemyWaveManager.Instance.EnemyDefeated();
            pool.Release(gameObject);
            GameManager.Instance.ModifyCurrency(currencyAmount);
            GameManager.Instance.CurrentEnemyList.Remove(gameObject);
        }
    }

    /// <summary>
    /// Applies scaling to enemy stats based on wave number and an increase factor
    /// </summary>
    public void ApplyScaling(float enemyIncreaseFactor, int waveNumber)
    {
        float scalingFactor = Mathf.Pow(enemyIncreaseFactor, waveNumber);

        Stats.MaxHealth *= scalingFactor * Stats.HealthMultiplier;
        Stats.Health = Stats.MaxHealth;
        Stats.MovementSpeed *= scalingFactor * Stats.SpeedMultiplier;
        Stats.SprintSpeed *= scalingFactor * Stats.SpeedMultiplier;
        Stats.Resistance *= scalingFactor * Stats.ResistanceMultiplier;
        Stats.Shield *= scalingFactor * Stats.DamageReductionMultiplier;
        Stats.DamageReduction *= scalingFactor * Stats.DamageReductionMultiplier;
        Stats.BlockChance *= scalingFactor;
        Stats.SlowResistance *= scalingFactor;

        currencyAmount *= scalingFactor;
    }
}
