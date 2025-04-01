using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class SpawnTower : BasicTowerInfo
{
    [SerializeField] protected List<GameObject> spawnedChildren = new List<GameObject>();

    [SerializeField] protected float spawnLimit;

    protected float spawnDelay = 0.5f;

    private float timer;

    private List<Collider> collidersInTrigger = new List<Collider>();

    [SerializeField] protected GameObject spawnedAllyPrefab;

    private void Update()
    {
        SpawnHelperTower();
    }

    protected void SpawnHelperTower()
    {
        if (spawnLimit > spawnedChildren.Count)
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                spawnedChildren.Add(Instantiate(spawnedAllyPrefab, this.transform));
                timer = 0;
            }

        }
    }

    private void AssignTarget(Collider target)
    {
        
        if (target != null)
        {
            if (target.tag == "Enemy")
            {
                foreach (Collider col in collidersInTrigger)
                {
                    for (int x = 0; x < spawnedChildren.Count; x++)
                    {
                        if (spawnedChildren[x] == null)
                        {
                            spawnedChildren.RemoveAt(x);
                            break;
                        }
                        if (spawnedChildren[x] != null)
                        {
                            spawnedChildren[x].GetComponent<NeutraphylSpawnedUnit>().TargetObject = target.gameObject;
                        }

                    }
                }
                
            }
        }


    }

    private void OnTriggerStay(Collider other)
    {
       AssignTarget(other);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!collidersInTrigger.Contains(other)) // Avoid duplicates
        {
            collidersInTrigger.Add(other);
        }
    }
}
