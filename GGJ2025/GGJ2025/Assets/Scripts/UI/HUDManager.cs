using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    public PlayerController player;

    [Header("Health")]
    public Slider healthSlider;
    public Gradient healthGradient;
    public Image healthFill;

    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private TextMeshProUGUI hpText;
    public void WriteAllStatsToUI(EntityStats playerStats)
    {
        string twoLineInfo = $"{playerStats.CurrentHealth.Value}\n------\n{playerStats.MaxHealth.Value}";
        hpText.text = twoLineInfo;

        if (statsText == null)
        {
            return;
        }

        // Build the stats string (customize as needed)
        string statsInfo =
            $"<b>Basic Stats</b>\n" +
            $"Max Health: {playerStats.MaxHealth.Value}\n" +
            $"Health: {playerStats.CurrentHealth.Value}\n" +
            $"Movement Speed: {playerStats.MovementSpeed.Value}\n" +
            $"Sprint Speed: {playerStats.SprintSpeed.Value}\n" +
            $"Resistance: {playerStats.Resistance.Value}\n\n" +

            $"<b>Defense Stats</b>\n" +
            $"Shield: {playerStats.CurrentShield.Value}\n" +
            $"Damage Reduction: {playerStats.DamageReduction.Value}\n" +
            $"Block Chance: {playerStats.BlockChance.Value}\n\n" +

            $"<b>Status Effect Mechanics</b>\n" +
            $"Slow Resistance: {playerStats.SlowResistance.Value}\n\n" +

            $"<b>Stats Multiplier</b>\n" +
            $"Size Multiplier: {playerStats.SizeMultiplier.Value}\n" +
            $"Health Multiplier: {playerStats.HealthMultiplier.Value}\n" +
            $"Atk Multiplier: {playerStats.AtkMultiplier.Value}\n" +
            $"Damage Reduction Multiplier: {playerStats.DamageReductionMultiplier.Value}\n" +
            $"Resistance Multiplier: {playerStats.ResistanceMultiplier.Value}\n" +
            $"Speed Multiplier: {playerStats.SpeedMultiplier.Value}\n" +
            $"Gold Drop Multiplier: {playerStats.OxygenDropMultiplier.Value}\n";

        statsText.text = statsInfo;
    }

    public void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Set the max value of health slider
    /// </summary>
    /// <param name="health">The new max health</param>
    public void SetMaxHealth(float health)
    {
        healthSlider.maxValue = health;

        healthGradient.Evaluate(1f);
    }

    /// <summary>
    /// Set the current value of health slider and change the fill color of the slider
    /// </summary>
    /// <param name="health">The current health</param>
    public void SetHealth(float health)
    {
        healthSlider.value = health;

        healthFill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }


}
