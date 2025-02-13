using System.ComponentModel;
using UnityEngine;

public enum BuffUpdateTimeType
{ 
    Add,
    Replace,
    Keep
}

public enum BuffRemoveStackUpdateType
{ 
    Clear,
    Reduce
}

[System.Serializable]
public class BuffInfo {
    public BuffData buffData;
    public GameObject creator;
    public GameObject target;
    public int currentStack = 1;
    [ReadOnly] public float durationTimer;
    [ReadOnly] public float tickTimer;
}