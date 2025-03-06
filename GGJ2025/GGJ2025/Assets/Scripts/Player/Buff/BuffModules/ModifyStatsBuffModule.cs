using UnityEngine;

[CreateAssetMenu(fileName = "ModifyStatsBuffModule", menuName = "BuffSystem/ModifyStatsBuffModule")]
public class ModifyStatsBuffModule : BaseBuffModule
{
    public EntityStatsBuff stats;

     public override void Apply(BuffInfo buffInfo, DamageInfo damageInfo = null)
    {
        PlayerController player = buffInfo.target.GetComponent<PlayerController>();
        if (player)
        {
            // Base Stats
            player.Stats.MaxHealth.Value += stats.MaxHealth;
            player.Stats.CurrentHealth.Value += stats.CurrentHealth;
            player.Stats.MovementSpeed.Value += stats.MovementSpeed;
            player.Stats.SprintSpeed.Value += stats.SprintSpeed;
            player.Stats.Resistance.Value += stats.Resistance;

            // Defense Stats
            player.Stats.CurrentShield.Value += stats.CurrentShield;
            player.Stats.DamageReduction.Value += stats.DamageReduction;
            player.Stats.BlockChance.Value += stats.BlockChance;

            // Status Effect Mechanics
            player.Stats.SlowResistance.Value += stats.SlowResistance;

            // Stat Multipliers
            player.Stats.AtkMultiplier.Value *= (1 + stats.AtkMultiplier);
            player.Stats.DamageReductionMultiplier.Value *= (1 + stats.DamageReductionMultiplier);
            player.Stats.ResistanceMultiplier.Value *= (1 + stats.ResistanceMultiplier);
            player.Stats.SpeedMultiplier.Value *= (1 + stats.SpeedMultiplier);
            player.Stats.OxygenDropMultiplier.Value *= (1 + stats.OxygenDropMultiplier);

            // Update the HUD with the new values
            HUDManager.Instance.SetMaxHealth(player.Stats.MaxHealth.Value);
            HUDManager.Instance.SetHealth(player.Stats.CurrentHealth.Value);
        }
    }
}

