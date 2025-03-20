using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTower : BasicTowerInfo
{
    protected List<GameObject> spawned;

    protected float spawnLimit;

    protected float spawnDelay = 0.5f;

    private float timer;

    protected GameObject spawnedAlly;

    private void Update()
    {
        if (spawned.Count < spawnLimit)
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                spawned.Add(Instantiate(spawnedAlly, gameObject.transform));
                timer = 0;
            }
            
        }
    }
}
