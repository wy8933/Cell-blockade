using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    public enum DetectionMode
    {

    }

    public Collider _dectectionRadius;
    //public GameObject bulletPrefab;
    public TowerStats Stats;
    public TowerType towerType;
    //public GameObject playerModel;

    [Header("Direct")]
    //public Transform firepoint;
    public float maxBubble;
    public float currentBubble;
    public float bubbleHealthDeduct;
    public float bubbleGainAmount;
    public float bubbleCost;



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
    void Awake()
    {
        _dectectionRadius = GetComponent<CapsuleCollider>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    protected abstract void Attack(Collider other);

    protected abstract void ShowAttack(GameObject source, GameObject target);

    private void OnTriggerStay(Collider other)
    {
        Attack(other);

        ShowAttack(gameObject, other.gameObject);
    }
}
