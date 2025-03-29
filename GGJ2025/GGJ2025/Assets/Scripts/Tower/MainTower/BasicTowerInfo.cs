using UnityEngine;

public class BasicTowerInfo : MonoBehaviour
{
    public enum DetectionMode
    {
        Nearest,
        Farthest,
        Strongest,
    }

    public bool isTowerActive = false;

    public Collider _dectectionRadius;
    //public GameObject bulletPrefab;
    public TowerStats Stats;
    public TowerType towerType;
    //public GameObject playerModel;

    public DetectionMode detectionMode;

    [Header("Direct")]
    //public Transform firepoint;
    public float maxBubble;
    public float currentBubble;
    public float bubbleHealthDeduct;
    public float bubbleGainAmount;
    public float bubbleCost;

    void Awake()
    {
        _dectectionRadius = GetComponent<CapsuleCollider>();
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
