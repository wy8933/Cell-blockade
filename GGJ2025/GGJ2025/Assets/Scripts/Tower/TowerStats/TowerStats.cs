using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Tower ScriptObjs/TowerStats")]
public class TowerStats : ScriptableObject
{
    public enum TowerType
    {
        Direct,
        Indirect,
        AOE,
        Support,
    }

    public string towerName;

    public string towerDescription;

    public float attackSpeed;

    public float damage;

    public bool canDestroyArmored;

    public TowerType towerType;
}
