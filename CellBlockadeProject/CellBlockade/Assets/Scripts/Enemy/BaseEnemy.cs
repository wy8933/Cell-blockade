using ObjectPoolings;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(NavMeshAgent))]
public class BaseEnemy : MonoBehaviour
{
    public Stats Stats;
    public Transform player;
    public NavMeshAgent agent;
    public PrefabPool pool;
    public bool isReleased;
    public AudioSource audioSource;
    public Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        EnemyPathFinding();

        // Check if the animation is finished and set animation back to walk
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Armature|ArmatureAction 0") && 
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= animator.GetCurrentAnimatorStateInfo(0).length)
        {
            animator.SetBool("IsAttack", false);
        }
    }

    /// <summary>
    /// Init the enemy behavior to make sure it will function correctly whenncreate and get from an object pool
    /// </summary>
    /// <param name="pool">The pool it is from</param>
    public void InitEnemy(PrefabPool pool) 
    {
        Stats.Health = Stats.MaxHealth;
        this.pool = pool;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = Stats.MovementSpeed;
        isReleased = false;
        ApplyScaling(EnemyWaveManager.Instance.enemyIncreaseFactor, EnemyWaveManager.Instance.currentWaveIndex);
    }

    /// <summary>
    /// Path finding towards player or the wound depend on which of the two is closer
    /// </summary>
    protected virtual void EnemyPathFinding()
    {
        Vector3 woundPosition = Wound.Instance.transform.position;
        Vector3 targetPosition = woundPosition;

        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            float distanceToWound = Vector3.Distance(transform.position, woundPosition);

            // If the player is closer than the wound, target the player instead
            if (distanceToPlayer <= distanceToWound)
            {
                targetPosition = player.position;
            }
        }

        agent.SetDestination(targetPosition);
    }

    /// <summary>
    /// Deal damage to enemy and trigger die method when hp is below 0
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage) {
        Stats.Health -= damage;

        if (Stats.Health <= 0) {
            Die();
        }
    }

    /// <summary>
    /// Release the enemy in the object pool and notify enemy wave manager that one enemy is defeated
    /// </summary>
    private void Die() 
    {
        if (!isReleased) {
            audioSource.Play();
            isReleased = true;
            EnemyWaveManager.Instance.EnemyDefeated();
            pool.Release(gameObject);
        }
    }

    /// <summary>
    /// Play enemy death animation(Cut off feature)
    /// </summary>
    private void BubbleDeathAnimation() { 
        
    }

    /// <summary>
    /// Apply enemy scaling to enemy's stats based on the wave number and enemy increase factor
    /// </summary>
    /// <param name="enemyIncreaseFactor">How much enemy stats will multiply by per wave</param>
    /// <param name="waveNumber">The current number of wave</param>
    public void ApplyScaling(float enemyIncreaseFactor, int waveNumber)
    {
        float scalingFactor = Mathf.Pow(enemyIncreaseFactor, waveNumber);

        Stats.MaxHealth *= scalingFactor * Stats.HealthMultiplier;
        Stats.Health = Stats.MaxHealth;
        Stats.MovementSpeed *= scalingFactor * Stats.SpeedMultiplier;
        Stats.RotationSpeed *= scalingFactor * Stats.SpeedMultiplier;
        Stats.SprintSpeed *= scalingFactor * Stats.SpeedMultiplier;
        Stats.Resistance *= scalingFactor * Stats.ResistanceMultiplier;
        Stats.Shield *= scalingFactor * Stats.DamageReductionMultiplier;
        Stats.DamageReduction *= scalingFactor * Stats.DamageReductionMultiplier;
        Stats.BlockChance *= scalingFactor;
        Stats.SlowResistance *= scalingFactor;
    }

}
