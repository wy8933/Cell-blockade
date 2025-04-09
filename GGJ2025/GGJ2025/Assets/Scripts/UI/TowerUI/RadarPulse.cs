using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RadarPulse : MonoBehaviour
{
    RaycastHit hit;

    private Transform pulseTransform;

    private float pulseSpeed;
    private float currentRadius;
    private float maxRadius;

    private float pulseDelay;
    private float pulseCooldown;
    private bool isPulsing;

    private List<GameObject> activePings = new List<GameObject>();

    [Header("Radar Values")]

    private RectTransform radarPanel;

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

            if (pulseCooldown <= 0)
            {
                isPulsing = true;
            }
        }

        

    }

    private void ClearPings()
    {

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
