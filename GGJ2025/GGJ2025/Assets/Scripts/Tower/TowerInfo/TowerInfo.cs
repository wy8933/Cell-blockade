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
    //The name of the tower
    [field: SerializeField] public string Name { get; private set; }
    //The ID number of the tower
    [field: SerializeField] public int ID { get; private set; }
    //The length and width of the tower
    [field: SerializeField] public Vector2Int Size { get; private set; }
    //The price of the tower
    [field: SerializeField] public int TowerPrice { get; private set; }
    //the prefab of the tower that would be instantiated 
    [field: SerializeField] public GameObject TowerPrefab { get; private set; }
    //a quick description of the tower
    [field: SerializeField] public string TowerDescription { get; private set; }

    

}

