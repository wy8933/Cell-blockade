using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerInfo", menuName = "Scriptable Objects/TowerInfo")]
public class TowerInfo : ScriptableObject
{
    public List<TowerData> TowerList;
}

[Serializable] public class TowerData 
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public int ID { get; private set; }
    [field: SerializeField] public Vector2Int Size { get; private set; }
    [field: SerializeField] public GameObject TowerPrefab { get; private set; }
    [field: SerializeField] public string TowerDescription { get; private set; }
    
}

