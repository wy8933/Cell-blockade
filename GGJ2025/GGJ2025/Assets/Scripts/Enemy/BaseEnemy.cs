using ObjectPoolings;
using UnityEngine;
using UnityEngine.AI;

public enum PathfindingMode { AlwaysPlayer, AlwaysCore, Closest }
public enum EnemyAIState
{
    Idle,
    Chase,
    Attack,
    Destroy
}

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
    public int attackDamage = 10;

    [Header("Pathfinding Settings")]
    public float detectionRadius = 2.0f;
    public PathfindingMode pathfindingMode = PathfindingMode.AlwaysPlayer;
    protected EnemyAIState currentState = EnemyAIState.Chase;

    // The currently selected target based on pathfinding mode
    protected Transform currentTarget;

    protected NavMeshAgent agent;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = true;
        player = GameManager.Instance.Player.gameObject.transform;
    }

    protected virtual void Update()
    {
        EnemyPathFinding();

        // Set the destination toward the current target
        if (currentTarget != null)
        {
            agent.SetDestination(currentTarget.position);
        }

        // Reset the attack animation if finished
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
        // For initialization, default to targeting the core.
        currentTarget = Wound.Instance.transform;
        ApplyScaling(EnemyWaveManager.Instance.enemyIncreaseFactor, EnemyWaveManager.Instance.currentWaveIndex);
        GameManager.Instance.CurrentEnemyList.Add(gameObject);
    }

    /// <summary>
    /// Chooses the target based on the selected pathfinding mode
    /// </summary>
    protected virtual void EnemyPathFinding()
    {
        Transform coreTransform = Wound.Instance.transform;
        Transform chosenTarget = null;

        switch (pathfindingMode)
        {
            case PathfindingMode.AlwaysPlayer:
                chosenTarget = player;
                break;
            case PathfindingMode.AlwaysCore:
                chosenTarget = coreTransform;
                break;
            case PathfindingMode.Closest:
                if (player != null)
                {
                    float dPlayer = Vector3.Distance(transform.position, player.position);
                    float dCore = Vector3.Distance(transform.position, coreTransform.position);
                    chosenTarget = (dPlayer <= dCore) ? player : coreTransform;
                }
                else
                {
                    chosenTarget = coreTransform;
                }
                break;
        }

        currentTarget = chosenTarget;
    }

    /// <summary>
    /// Deal damage to the enemy and trigger death when health is 0 or below
    /// </summary>
    public void TakeDamage(float damage)
    {
        // TODO: change this to use the damage manager
        Stats.Health -= damage;
        DamageTextManager.Instance.ShowDamageText(transform.position, damage);
        if (Stats.Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Releases the enemy back to the object pool and notifies the enemy wave manager
    /// </summary>
    public void Die()
    {
        if (!isReleased)
        {
            audioSource.Play();
            GetComponent<BuffHandler>().TriggerAllOnDeath();
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