using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStats", menuName = "Stats/EntityStats")]
public class EntityStats : ScriptableObject
{
    [Header("Player-Specific Stats")]
    [Tooltip("This should only be used by player")]
    public FloatReference CurrentHealth;
    public FloatReference CurrentShield;

    [Header("Basic Stats")]
    public FloatReference MaxHealth;
    public FloatReference Resistance;

    [Header("Defense Stats")]
    public FloatReference DamageReduction;
    public FloatReference BlockChance;

    [Header("Status Effect Mechanics")]
    public FloatReference SlowResistance;

    [Header("Stats Multiplier")]
    public FloatReference SizeMultiplier;
    public FloatReference HealthMultiplier;
    public FloatReference AtkMultiplier;
    public FloatReference DamageReductionMultiplier;
    public FloatReference ResistanceMultiplier;
    public FloatReference SpeedMultiplier;
    public FloatReference OxygenDropMultiplier;

    [Header("Movement")]
    public FloatReference MovementSpeed;
    public FloatReference SprintSpeed;
}

[Serializable]
public class EntityStatsBuff
{
    [Header("Player-Specific Stats")]
    [Tooltip("This should only be used by player")]
    public float CurrentHealth;
    public float CurrentShield;

    [Header("Basic Stats")]
    public float MaxHealth;
    public float Resistance;

    [Header("Defense Stats")]
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
    public float OxygenDropMultiplier;

    [Header("Movement")]
    public float MovementSpeed;
    public float SprintSpeed;
}
