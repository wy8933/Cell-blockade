
using UnityEngine;

public enum DamageType {
    None
}


[System.Serializable]
public class DamageInfo
{
    public GameObject creator;
    public GameObject target;
    public float damage;
    public DamageType damageType;

    public DamageInfo(GameObject creator, GameObject target, float damage, DamageType damageType) { 
        this.creator = creator;
        this.target = target;
        this.damage = damage;
        this.damageType = damageType;
    }
}
