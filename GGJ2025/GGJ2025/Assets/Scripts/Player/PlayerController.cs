using ObjectPoolings;
using System.Collections;
using System;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody _playerRB;
    public Camera mainMamera;
    public GameObject bulletPrefab;
    public EntityStats Stats;
    public WeaponType weaponType;
    public GameObject playerModel;

    [Header("Movement/Rotation")]
    public float maxSpeed;
    public Vector2 moveDirection;
    public Vector2 mousePosition;

    [Header("Bubble")]
    public Transform firepoint;
    public float maxBubble;
    public float currentBubble;
    public float bubbleHealthDeduct;
    public float bubbleGainAmount;
    public float bubbleCost;

    [Header("Machine Gun")]
    public bool isShooting;
    public float shootCooldown;
    public float shootshootCooldownTimer;
    public int machineGunAngleOffset;

    [Header("Shot Gun")]
    public float shotGunBubbleCost;
    public int bulletCount;
    public int shotGunAngleOffset;

    void Start()
    {
        Time.timeScale = 1;
        _playerRB = GetComponent<Rigidbody>();
        _playerRB.maxLinearVelocity = maxSpeed;

        ResetStats();

        // Init the HUD UI
        Stats.CurrentHealth.Value = Stats.MaxHealth.Value;
        HUDManager.Instance.SetMaxHealth(Stats.MaxHealth.Value);
        HUDManager.Instance.SetHealth(Stats.CurrentHealth.Value);
    }

    void FixedUpdate()
    {
        PlayerMovement();
        PlayerRotation();
        HUDManager.Instance.WriteAllStatsToUI(Stats);
    }

    private void Update()
    {
        // Constantly shoot bullet by cooldown
        if (isShooting) 
        {
            shootshootCooldownTimer -= Time.deltaTime;
            if (shootshootCooldownTimer < 0) 
            {
                Attack();
                shootshootCooldownTimer += shootCooldown;
            }
        }
    }

    /// <summary>
    /// Move the player by add force in player's rigidbody towards the input direction
    /// </summary>
    public void PlayerMovement() 
    {
        // Physics doesn't need delta time
        _playerRB.AddForce(new Vector3(moveDirection.x,0,moveDirection.y) * Stats.MovementSpeed.Value);
    }

    /// <summary>
    /// Rotate the player towards the mouse position
    /// </summary>
    public void PlayerRotation()
    {
        Ray ray = mainMamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        // Get the mouse position and rotate player
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 targetPosition = ray.GetPoint(distance);
            Vector3 direction = (targetPosition - transform.position).normalized;

            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-direction);
                playerModel.transform.rotation = Quaternion.Slerp(playerModel.transform.rotation, targetRotation, Time.deltaTime * 10000);
            }
        }
    }

    /// <summary>
    /// Perform different bullet pattern based on the weapon type of player
    /// </summary>
    public void Attack() 
    {
        switch (weaponType)
        {
            case WeaponType.Scatter:
                if (currentBubble >= bubbleCost) {
                    SpawnBullet(machineGunAngleOffset);
                    ReduceBubble(bubbleCost);
                }
                break;
            case WeaponType.ShotGun:
                if (currentBubble >= shotGunBubbleCost) {
                    for (int i = 0; i < bulletCount; i++)
                    {
                        SpawnBullet(shotGunAngleOffset);
                    }
                    ReduceBubble(shotGunBubbleCost);
                }
                break;
            case WeaponType.RapidFire:
                if (currentBubble >= bubbleCost)
                {
                    SpawnBullet(0);
                    ReduceBubble(bubbleCost);
                }
                break;
        }
    }

    /// <summary>
    /// Create or get the bullet from object pool and set to firepoint transform with offsets, and init bullet
    /// </summary>
    /// <param name="offset">The offset range for bullet angle</param>
    public void SpawnBullet(int offset)
    {
        // Calculate the bullet shoot angle based on random offset
        Vector3 currentRotation = playerModel.transform.eulerAngles;
        float randomOffset = Random(offset);
        currentRotation.y += randomOffset + 180;
        Quaternion rotation = Quaternion.Euler(currentRotation);

        // Create or get the bullet from the object pool
        var (objectInstance, pool) = ObjectPooling.GetOrCreate(bulletPrefab, firepoint.position, rotation);
        var bulletController = objectInstance.GetComponent<BulletController>();

        // Init bullet
        if (bulletController)
        {
            bulletController.InitBullet(pool, Stats.AtkMultiplier.Value);
        }
    }

    /// <summary>
    /// Create random number from positive to negative
    /// </summary>
    /// <param name="num">The positive and negative range</param>
    /// <returns>The random number</returns>
    private float Random(int num)
    { 
        return new System.Random().Next(-num,num) - new System.Random().Next(-num, num);
    }

    /// <summary>
    /// Take damage and trigger death when hp is lower than 0
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        // deal damage and update UI
        Stats.CurrentHealth.Value -= (damage * (1-Stats.BlockChance.Value));
        HUDManager.Instance.SetHealth(Stats.CurrentHealth.Value);

        if (Stats.CurrentHealth.Value <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Trigger gameover when player died
    /// </summary>
    private void Die()
    {
        Stats.CurrentHealth.Value = 0;
        HUDManager.Instance.SetHealth(0);
        HUDManager.Instance.WriteAllStatsToUI(Stats);
        GameManager.Instance.GameOver();
    }

    /// <summary>
    /// Gain bubble ammo 
    /// </summary>
    /// <param name="amount">Amound of bubble ammo gain</param>
    public void GainBubble(float amount)
    {
        currentBubble += amount;

        if (currentBubble > maxBubble)
        { 
            currentBubble = maxBubble;
        }
    }

    /// <summary>
    /// Reduce bubble ammo
    /// </summary>
    /// <param name="amount">the amonud of bubble ammo reduced</param>
    public void ReduceBubble(float amount)
    {
        currentBubble -= 0;

        if (currentBubble < 0)
        {
            currentBubble = 0;
        }

    }

    /// <summary>
    /// Gain bubble ammo and reduce health
    /// </summary>
    public void WaterLogic() {
        TakeDamage(bubbleHealthDeduct);
        GainBubble(bubbleGainAmount);
    }

    private void ResetStats()
    {
        Stats.MaxHealth.Value = Stats.MaxHealth.initialValue;
        Stats.CurrentHealth.Value = Stats.CurrentHealth.initialValue;
        Stats.MovementSpeed.Value = Stats.MovementSpeed.initialValue;
        Stats.SprintSpeed.Value =  Stats.SprintSpeed.initialValue;
        Stats.Resistance.Value = Stats.Resistance.initialValue;

        Stats.CurrentShield.Value = Stats.CurrentShield.initialValue;
        Stats.DamageReduction.Value = Stats.DamageReduction.initialValue;
        Stats.BlockChance.Value = Stats.BlockChance.initialValue;

        Stats.SlowResistance.Value = Stats.SlowResistance.initialValue;

        Stats.AtkMultiplier.Value = (Stats.AtkMultiplier.initialValue);
        Stats.DamageReductionMultiplier.Value = (Stats.DamageReductionMultiplier.initialValue);
        Stats.ResistanceMultiplier.Value = (Stats.ResistanceMultiplier.initialValue);
        Stats.SpeedMultiplier.Value = (Stats.SpeedMultiplier.initialValue);
        Stats.OxygenDropMultiplier.Value = (Stats.OxygenDropMultiplier.initialValue);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DeadZone") {
            Die();
        }
    }
}
