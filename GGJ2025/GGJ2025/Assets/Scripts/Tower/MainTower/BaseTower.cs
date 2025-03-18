using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseTower : BasicTowerInfo
{


    /*
    [Header("AOE")]
    public bool isShooting;
    public float shootCooldown;
    public float shootshootCooldownTimer;
    public int machineGunAngleOffset;

    [Header("Support")]
    public float shotGunBubbleCost;
    public int bulletCount;
    public int shotGunAngleOffset;
    */
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected abstract void Attack(Collider other);

    protected abstract void ShowAttack(GameObject source, GameObject target);

    private void OnTriggerStay(Collider other)
    {
        Attack(other);

        ShowAttack(gameObject, other.gameObject);

        //Debug.Log(other.gameObject.transform.position);
    }

    /// <summary>
    /// Deal damage to the tower
    /// </summary>
    /// <param name="damage">The amount of damage</param>
    public void TakeDamage(float damage)
    {
        // TODO: Modify the take damage method to use the buff system's damage system
        Stats.Health -= damage;

        if (Stats.Health <= 0)
        {
            Stats.Health = 0;
            gameObject.SetActive(false);

            NavMeshManager.Instance.BakeNavMesh();

            Destroy(gameObject);
        }
        else
        {
            
        }
    }

}
