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
    public enum TowerType
    {
        Direct,
        Indirect,
        AOE,
        Support,
    }

    [field: SerializeField] public string towerName { get; private set; }
    [field: SerializeField] public int towerId { get; private set; }
    [field: SerializeField] public GameObject towerPrefab { get; private set; }
    [field: SerializeField] public string towerDescription { get; private set; }
    [field: SerializeField] public TowerType towerType { get; private set; }
}

