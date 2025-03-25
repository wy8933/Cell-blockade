using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTower : BasicTowerInfo
{
    protected List<GameObject> spawnedChildren;

    protected float spawnLimit;

    protected float spawnDelay = 0.5f;

    private float timer;

    protected GameObject spawnedAllyPrefab;

    private void Update()
    {
        SpawnHelperTower();
    }

    protected void SpawnHelperTower()
    {
        if (spawnedChildren.Count < spawnLimit)
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                spawnedChildren.Add(Instantiate(spawnedAllyPrefab, gameObject.transform));
                timer = 0;
            }

        }
    }

    private void AssignTarget(Transform target)
    {
        if (target != null)
        {
            if (target.tag == "Enemy")
            {
                for (int x = 0; x < spawnedChildren.Count; x++)
                {
                    if (spawnedChildren[x].GetComponent<NeutraphylSpawnedUnit>().TargetTransform != null)
                    {
                        spawnedChildren[x].GetComponent<NeutraphylSpawnedUnit>().TargetTransform = target;
                    }

                }
            }
        }


    }

    private void OnTriggerStay(Collider other)
    {
       AssignTarget(other.transform);
    }
}
