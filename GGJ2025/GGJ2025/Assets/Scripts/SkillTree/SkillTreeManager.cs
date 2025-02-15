using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SkillTreeManager : MonoBehaviour
{
    [Header("Skill Tree Setup")]
    // List of all available skill tree nodes
    public List<SkillTreeNode> allNodes;

    // File path for JSON persistence (adjust the path as needed)
    public string jsonFilePath = "Assets/SkillTreeData.json";

    [Header("Integration")]
    // The player GameObject (which should have a BuffHandler component)
    public GameObject player;

    private void Start()
    {
        LoadSkillTree();
        ApplyUnlockedBuffs();
    }

    /// <summary>
    /// Unlocks a skill tree node if all prerequisites are met.
    /// Also applies the buff associated with this node.
    /// </summary>
    public bool UnlockNode(SkillTreeNode node)
    {
        if (node.isUnlocked)
        {
            Debug.Log($"{node.nodeName} is already unlocked.");
            return false;
        }

        // Check prerequisites
        foreach (SkillTreeNode pre in node.prerequisites)
        {
            if (!pre.isUnlocked)
            {
                Debug.Log($"Cannot unlock {node.nodeName}. Prerequisite {pre.nodeName} is not unlocked.");
                return false;
            }
        }

        node.isUnlocked = true;
        Debug.Log($"{node.nodeName} unlocked!");

        // Create a new BuffInfo from the node's BuffData and add it to the player's BuffHandler
        if (node.buffData != null)
        {
            BuffHandler buffHandler = player.GetComponent<BuffHandler>();
            if (buffHandler != null)
            {
                BuffInfo newBuff = new BuffInfo
                {
                    buffData = node.buffData,
                    creator = player,
                    target = player
                };
                newBuff.Initialize();
                buffHandler.AddBuff(newBuff);
            }
        }

        SaveSkillTree();
        return true;
    }

    /// <summary>
    /// On game start, ensure that buffs for already unlocked nodes are applied.
    /// </summary>
    private void ApplyUnlockedBuffs()
    {
        BuffHandler buffHandler = player.GetComponent<BuffHandler>();
        if (buffHandler == null) return;

        foreach (SkillTreeNode node in allNodes)
        {
            if (node.isUnlocked && node.buffData != null)
            {
                BuffInfo newBuff = new BuffInfo
                {
                    buffData = node.buffData,
                    creator = player,
                    target = player
                };
                newBuff.Initialize();
                buffHandler.AddBuff(newBuff);
            }
        }
    }

    /// <summary>
    /// Saves the current state of the skill tree to a JSON file.
    /// </summary>
    public void SaveSkillTree()
    {
        SkillTreeData data = new SkillTreeData();
        data.nodes = new List<SkillTreeNodeData>();

        foreach (SkillTreeNode node in allNodes)
        {
            SkillTreeNodeData nodeData = new SkillTreeNodeData
            {
                uniqueID = node.uniqueID,
                isUnlocked = node.isUnlocked,
                prerequisiteIDs = new List<string>()
            };

            if (node.prerequisites != null)
            {
                foreach (SkillTreeNode pre in node.prerequisites)
                {
                    nodeData.prerequisiteIDs.Add(pre.uniqueID);
                }
            }
            data.nodes.Add(nodeData);
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(jsonFilePath, json);
        Debug.Log("Skill tree saved to JSON.");
    }

    /// <summary>
    /// Loads the skill tree state from a JSON file.
    /// </summary>
    public void LoadSkillTree()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogWarning("JSON file not found. Starting with default state.");
            return;
        }

        string json = File.ReadAllText(jsonFilePath);
        SkillTreeData data = JsonUtility.FromJson<SkillTreeData>(json);

        // Build a lookup table for nodes by uniqueID
        Dictionary<string, SkillTreeNode> nodeLookup = new Dictionary<string, SkillTreeNode>();
        foreach (SkillTreeNode node in allNodes)
        {
            nodeLookup[node.uniqueID] = node;
        }

        // Update nodes based on saved data
        foreach (SkillTreeNodeData nodeData in data.nodes)
        {
            if (nodeLookup.TryGetValue(nodeData.uniqueID, out SkillTreeNode node))
            {
                node.isUnlocked = nodeData.isUnlocked;
            }
        }
        Debug.Log("Skill tree loaded from JSON.");
    }
}

// Helper classes for JSON serialization

[System.Serializable]
public class SkillTreeData
{
    public List<SkillTreeNodeData> nodes;
}

[System.Serializable]
public class SkillTreeNodeData
{
    public string uniqueID;
    public bool isUnlocked;
    public List<string> prerequisiteIDs;
}
