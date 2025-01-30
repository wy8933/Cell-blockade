using ObjectPoolings;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Cut feature due to time
/// </summary>
public class BossEnemy : BasicEnemy
{
    [Header("Boss Stats Mult")]
    public float healthMult;
    public float speedMult;

    /// <summary>
    /// Init the enemy behavior to make sure it will function correctly whenncreate and get from an object pool
    /// </summary>
    /// <param name="pool">The pool it is from</param>
    public void InitEnemy(PrefabPool pool)
    {
        this.pool = pool;

        Stats.Health *= healthMult;
        Stats.MovementSpeed *= speedMult;

        Stats.Health = Stats.MaxHealth;
        agent.speed = Stats.MovementSpeed;
    }

    private void Update()
    {
        EnemyPathFinding();
        TryAttack();
    }

    /// <summary>
    /// Boss Enemy only follow player
    /// </summary>
    protected override void EnemyPathFinding() 
    {
        if (agent != null)
        {
            agent.destination = player.position;
        }
    }
}
