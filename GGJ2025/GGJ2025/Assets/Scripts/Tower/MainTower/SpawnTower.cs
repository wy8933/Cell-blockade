using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTower : BasicTowerInfo
{
    protected List<GameObject> spawned;

    protected float spawnLimit;

    protected float spawnDelay = 0.5f;

    protected GameObject spawnedAlly;

    private void Update()
    {
        if (spawned.Count < spawnLimit)
        {
            spawned.Add(Instantiate(spawnedAlly, gameObject.transform));
        }
    }
}
