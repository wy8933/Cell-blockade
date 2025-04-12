using TMPro;
using UnityEngine;

public class Wound : MonoBehaviour
{
    public static Wound Instance;

    public FloatReference MaxHealth;
    public FloatReference CurrentHealth;

    public TextMeshProUGUI hpText;

    private void Start()
    {
        Instance = this;
    }

    /// <summary>
    /// Trigger game over if enemy enter the wound
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") {
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            enemy.Die();
            CurrentHealth.Value -= enemy.attackDamage;
            hpText.text = "Current Core HP:" + CurrentHealth.Value;
            if (CurrentHealth.Value <= 0)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}
