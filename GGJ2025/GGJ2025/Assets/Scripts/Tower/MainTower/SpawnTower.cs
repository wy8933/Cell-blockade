using System.Collections.Generic;
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
        if (isTowerActive)
        {
            SpawnHelperTower();
        }
        
    }

    protected void SpawnHelperTower()
    {
        if (spawnLimit > spawnedChildren.Count)
        {
            timer += Time.deltaTime;
            if (timer >= spawnDelay)
            {
                GameObject temp = Instantiate(spawnedAllyPrefab, transform.position + new Vector3(1, 0.5f, 1), Quaternion.identity);
                temp.GetComponent<NeutraphylLaserHelper>().ParentTransform = this.transform.position + new Vector3(1, 0.5f, 1);
                spawnedChildren.Add(temp);
                timer = 0;
            }

        }
    }
}
