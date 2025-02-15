using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSkillTreeNode", menuName = "SkillTree/SkillTreeNode")]
public class SkillTreeNode : ScriptableObject
{
    [Header("Skill Information")]
    public string nodeName;
    [TextArea]
    public string description;
    public int cost;

    [Header("Persistence")]
    // Unique identifier used for saving/loading the tree structure
    public string uniqueID;

    [Header("Connections")]
    // Direct references to prerequisite nodes
    public List<SkillTreeNode> prerequisites;

    [Header("Buff Reference")]
    // Reference to a BuffData from your buff system 
    public BuffData buffData;

    [HideInInspector]
    public bool isUnlocked = false;
}
