using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GridData : MonoBehaviour
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="objectSize"></param>
    /// <param name="ID"></param>
    /// <param name="placedObjectIndex"></param>
    /// <exception cref="Exception"></exception>
    public void AddObjectAt(Vector3Int gridPos, Vector2Int objectSize, int ID, int placedObjectIndex)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPos, objectSize);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);

        foreach(var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                throw new Exception($"Theres already a tower at{pos}");
            }

            placedObjects[pos] = data;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="objectSize"></param>
    /// <returns></returns>
    private List<Vector3Int> CalculatePositions(Vector3Int gridPos, Vector2Int objectSize)
    {
        List<Vector3Int> returnVal = new List<Vector3Int>();

        for (int x = 0; x < objectSize.x; x++)
        {
            for (int y = 0; y < objectSize.y; y++)
            {
                returnVal.Add(gridPos + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="objectSize"></param>
    /// <returns></returns>
    public bool CanPlaceObjectAt(Vector3Int gridPos, Vector2Int objectSize)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPos, objectSize);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    internal int GetRepresentationIndex(Vector3Int gridPos)
    {
        if (placedObjects.ContainsKey(gridPos) == false)
        {
            return -1;
        }
        return placedObjects[gridPos].PlacedObjectIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gridPos"></param>
    internal void RemoveObjectAt(Vector3Int gridPos)
    {
        foreach (var pos in placedObjects[gridPos].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;

    public int ID { get; private set; }

    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
