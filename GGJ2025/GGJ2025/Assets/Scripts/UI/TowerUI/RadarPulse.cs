using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RadarPulse : MonoBehaviour
{
    RaycastHit hit;

    private Transform player;

    private Transform pulseTransform;

    private float pulseSpeed;
    private float currentRadius;
    private float maxRadius;

    private float pulseDelay;
    private float pulseCooldown;
    private bool isPulsing;

    private List<GameObject> activePings = new List<GameObject>();

    [Header("Radar Values")]

    [SerializeField] private RectTransform radarPanel;

    [SerializeField] private GameObject pingPrefab;

    private void Awake()
    {

    }

    private void Update()
    {
        if (isPulsing)
        {
            currentRadius += pulseSpeed * Time.deltaTime;

            Collider[] pulseHits = Physics.OverlapSphere(transform.position, currentRadius);

            foreach (Collider hit in pulseHits)
            {
                if (hit.tag == "Enemy")
                {
                    Vector3 relativePos = hit.transform.position - player.position;
                    Vector2 radarPos = new Vector2(relativePos.x, relativePos.z);

                    GameObject ping = Instantiate(pingPrefab, radarPanel);
                    ping.GetComponent<RectTransform>().anchoredPosition = radarPos;
                    activePings.Add(ping);
                }
            }

            if (currentRadius >= maxRadius)
            {
                isPulsing = false;
                pulseCooldown = pulseDelay;
                currentRadius = 0f;
            }
        }
        else
        {
            pulseCooldown -= Time.deltaTime;

            if (pulseCooldown <= 0f)
            {
                isPulsing = true;
                ClearPings();
            }
        }

        

    }

    private void ClearPings()
    {
        foreach (GameObject ping in activePings)
        {
            Destroy(ping);
        }
        activePings.Clear();
    }

    private void OnDrawGizmos()
    {
        if (isPulsing)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(transform.position, currentRadius);
        }
    }


}
