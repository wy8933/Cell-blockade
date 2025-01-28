using System;
using UnityEngine;
public enum WeaponType
{
    Scatter,
    RapidFire,
    ShotGun
}

/// <summary>
/// Stats used for both player and enemy
/// </summary>
[System.Serializable]
public struct Stats
{
    [Header("Basic Stats")]
    public float MaxHealth;
    public float Health;
    public float MovementSpeed;
    public float RotationSpeed;
    public float SprintSpeed;
    public float Resistance; 

    [Header("Defense Stats")]
    public float Shield;
    public float DamageReduction;
    public float BlockChance;

    [Header("Status Effect Mechanics")]
    public float SlowResistance;

    [Header("Stats Multiplier")]
    public float SizeMultiplier;
    public float HealthMultiplier;
    public float AtkMultiplier;
    public float DamageReductionMultiplier;
    public float ResistanceMultiplier;
    public float SpeedMultiplier;
    public float GoldDropMultiplier;
}
