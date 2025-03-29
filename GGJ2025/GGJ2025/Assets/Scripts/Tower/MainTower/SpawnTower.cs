using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTower : BasicTowerInfo
{
    [SerializeField] protected List<GameObject> spawnedChildren = new List<GameObject>();

    [SerializeField] protected float spawnLimit;

    protected float spawnDelay = 0.5f;

    private float timer;

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

    private void AssignTarget(Transform target)
    {
        if (target != null)
        {
            if (target.tag == "Enemy")
            {
                for (int x = 0; x < spawnedChildren.Count; x++)
                {
                    if (target != null && spawnedChildren[x].GetComponent<NeutraphylSpawnedUnit>().TargetTransform == Vector3.zero)
                    {
                        spawnedChildren[x].GetComponent<NeutraphylSpawnedUnit>().TargetTransform = target.position;
                    }
                    else
                    {
                        spawnedChildren[x].GetComponent<NeutraphylSpawnedUnit>().TargetTransform = Vector3.zero;
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
