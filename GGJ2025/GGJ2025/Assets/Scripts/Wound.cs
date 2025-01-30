using UnityEngine;

public class Wound : MonoBehaviour
{
    public static Wound Instance;

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
            GameManager.Instance.GameOver();
        }
    }
}
