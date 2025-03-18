using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedGameObjects = new List<GameObject>();

    public int PlaceObject(GameObject towerPrefab, Vector3 pos)
    {
        GameObject newObject = Instantiate(towerPrefab);
        newObject.transform.position = pos;

        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }
}