using UnityEngine;
public enum BuffModuleType
{
    ModifyStats
}

public abstract class BaseBuffModule : ScriptableObject
{
    public string moduleName; 
    public BuffModuleType moduleType;
    public abstract void Apply(BuffInfo buffInfo, DamageInfo damageInfo = null);
}
