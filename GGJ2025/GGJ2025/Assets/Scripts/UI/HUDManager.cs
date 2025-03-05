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
