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

    public int PlaceObject(GameObject towerPrefab, Vector3 pos, Quaternion rot)
    {
        GameObject newObject = Instantiate(towerPrefab);
        newObject.transform.position = pos;
        foreach (Transform child in newObject.transform)
        {
            child.rotation = rot;
        }

        

        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
        {
            return;
        }
        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}