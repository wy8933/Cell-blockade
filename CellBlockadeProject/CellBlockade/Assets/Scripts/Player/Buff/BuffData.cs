using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "BuffData", menuName = "BuffSystem/BuffData")]
public class BuffData : ScriptableObject
{
    [Header("Basic Information")]
    public int id;
    public string buffName;
    public string description;
    public Sprite icon;
    public int maxStack;
    public int priority;
    public List<string> tags;

    [Header("Time Information")]
    public bool isForever;
    public float duration;
    public float tickTime;

    [Header("Update Enums")]
    public BuffUpdateTimeType buffUpdateType;
    public BuffRemoveStackUpdateType buffRemoveStackType;

    [Header("Basic Callbacks")]
    public BaseBuffModule OnCreate;
    public BaseBuffModule OnRemove;
    public BaseBuffModule OnTick;

    [Header("Damage Callbacks")]
    public BaseBuffModule OnHit;
    public BaseBuffModule OnHurt;
    public BaseBuffModule OnKill;
    public BaseBuffModule OnDeath;
}
