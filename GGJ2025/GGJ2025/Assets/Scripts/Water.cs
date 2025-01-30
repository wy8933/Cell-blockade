using UnityEngine;

public class Water : MonoBehaviour
{
    private PlayerController _player;
    public float time;
    public float timer;
    private bool isPlayerIn;


    private void Update()
    {
        if (isPlayerIn) 
        {
            timer -= Time.deltaTime;

            if (timer < 0) {
                timer = time;
                _player.WaterLogic();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _player = other.GetComponent<PlayerController>();
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isPlayerIn = false;
        }
    }
}
