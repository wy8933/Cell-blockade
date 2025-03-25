using UnityEngine;

public class Wound : MonoBehaviour
{
    public static Wound Instance;

    public FloatReference MaxHealth;
    public FloatReference CurrentHealth;

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
            Debug.Log(CurrentHealth.Value);
            if (CurrentHealth.Value <= 0)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}
