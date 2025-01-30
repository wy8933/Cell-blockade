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

    [Header("Bubble")]
    public Slider bubbleSlider;
    public Gradient bubbleGradient;
    public Image bubbleFill;

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


    /// <summary>
    /// Set the max value of bubble slider
    /// </summary>
    /// <param name="value">The new max bubble</param>
    public void SetMaxBubble(float value)
    {
        bubbleSlider.maxValue = value;

        bubbleGradient.Evaluate(1f);
    }


    /// <summary>
    /// Set the current value of bubble slider, and change the fill color of the slider
    /// </summary>
    /// <param name="value">The curret bubble value</param>
    public void SetBubble(float value)
    {
        bubbleSlider.value = value;

        bubbleFill.color = bubbleGradient.Evaluate(bubbleSlider.normalizedValue);
    }
}
