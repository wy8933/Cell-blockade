using UnityEngine;

[CreateAssetMenu(fileName = "ModifyStatsBuffModule", menuName = "BuffSystem/ModifyStatsBuffModule")]
public class ModifyStatsBuffModule : BaseBuffModule
{
    public Stats stats;

    public override void Apply(BuffInfo buffInfo, DamageInfo damageInfo = null)
    {
        PlayerController player = buffInfo.target.GetComponent<PlayerController>();
        if (player)
        {
            // Base Stats
            player.Stats.MaxHealth += stats.MaxHealth;
            player.Stats.Health += stats.Health;
            player.Stats.MovementSpeed += stats.MovementSpeed;
            player.Stats.RotationSpeed += stats.RotationSpeed;
            player.Stats.SprintSpeed += stats.SprintSpeed;
            player.Stats.Resistance += stats.Resistance;

            // Defense Stats
            player.Stats.Shield += stats.Shield;
            player.Stats.DamageReduction += stats.DamageReduction;
            player.Stats.BlockChance += stats.BlockChance;

            // Status Effect Mechanics
            player.Stats.SlowResistance += stats.SlowResistance;

            // Stat Multipliers
            player.Stats.AtkMultiplier *= (1 + stats.AtkMultiplier);
            player.Stats.DamageReductionMultiplier *= (1 + stats.DamageReductionMultiplier);
            player.Stats.ResistanceMultiplier *= (1 + stats.ResistanceMultiplier);
            player.Stats.SpeedMultiplier *= (1 + stats.SpeedMultiplier);
            player.Stats.GoldDropMultiplier *= (1 + stats.GoldDropMultiplier);

        }
    }
}
