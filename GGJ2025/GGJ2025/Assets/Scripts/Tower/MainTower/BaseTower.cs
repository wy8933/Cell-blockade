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
}
