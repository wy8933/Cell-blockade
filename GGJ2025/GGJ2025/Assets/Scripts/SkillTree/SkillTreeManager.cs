using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance { get; private set; }

    [Header("Skill Tree Setup")]
    public List<SkillTreeNode> allNodes;

    // Use the same JSON file as the editor for the whole skill tree system
    public string jsonFilePath = "Assets/Saves/SkillTreeGraphData.json";

    [Header("Integration")]
    public GameObject player;

    [Header("Currency")]
    public int playerCurrency = 100;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        LoadSkillTreeData();
        ApplyUnlockedBuffs();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// Unlocks a skill tree node if all prerequisites are met
    /// And applies the buff associated with this node
    /// </summary>
    public bool UnlockNode(SkillTreeNode node)
    {
        if (node.isUnlocked)
        {
            Debug.Log($"{node.nodeName} is already unlocked");
            return false;
        }

        foreach (SkillTreeNode pre in node.prerequisites)
        {
            if (!pre.isUnlocked)
            {
                Debug.Log($"Cannot unlock {node.nodeName}. Prerequisite {pre.nodeName} is not unlocked");
                return false;
            }
        }

        node.isUnlocked = true;
        Debug.Log($"{node.nodeName} unlocked!");

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

        SaveSkillTreeData();
        return true;
    }

    /// <summary>
    /// Called from the UI when a Buy button is pressed
    /// Checks currency, prerequisites, then unlocks the skill
    /// </summary>
    public bool PurchaseSkill(SkillTreeNode node, SkillNodeUI nodeUI)
    {
        // Already unlocked? Nothing to do.
        if (node.isUnlocked)
        {
            Debug.Log($"{node.nodeName} is already unlocked");
            return false;
        }

        // Check currency
        if (playerCurrency < node.cost)
        {
            Debug.Log($"Not enough currency to unlock {node.nodeName}");
            return false;
        }

        // Check prerequisites
        foreach (SkillTreeNode pre in node.prerequisites)
        {
            if (!pre.isUnlocked)
            {
                Debug.Log($"Cannot unlock {node.nodeName}. Prerequisite {pre.nodeName} is not unlocked");
                return false;
            }
        }

        // Deduct currency
        playerCurrency -= node.cost;
        Debug.Log($"Unlocking {node.nodeName}. Remaining currency: {playerCurrency}");

        // Actually unlock the node
        bool unlocked = UnlockNode(node);
        if (unlocked && nodeUI != null && nodeUI.buyButton != null)
        {
            // Disable the Buy button and update text
            nodeUI.buyButton.interactable = false;
            TMPro.TextMeshProUGUI buttonText = nodeUI.buyButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = "Unlocked";
        }

        return unlocked;
    }


    /// <summary>
    /// Applies buffs for nodes that are already unlocked
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
    /// Saves the current state of the skill tree to the JSON file
    /// </summary>
    public void SaveSkillTreeData()
    {
        SkillTreeGraphSaveData data = new SkillTreeGraphSaveData();
        data.nodes = new List<SkillNodeData>();
        data.connections = new List<SkillConnectionData>();

        foreach (SkillTreeNode node in allNodes)
        {
            SkillNodeData nodeData = new SkillNodeData
            {
                GUID = node.uniqueID,
                title = node.nodeName,
                posX = node.positionX,
                posY = node.positionY,
                buffDataPath = (node.buffData != null) ? UnityEditor.AssetDatabase.GetAssetPath(node.buffData) : "",
                cost = node.cost,
                isUnlocked = node.isUnlocked
            };

            // Save prerequisites
            nodeData.prerequisiteIDs = new List<string>();
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
        Debug.Log("Skill tree data saved to " + jsonFilePath);
    }

    /// <summary>
    /// Loads the skill tree data from the JSON file
    /// This data includes both design and runtime information
    /// </summary>
    public void LoadSkillTreeData()
    {
        if (!File.Exists(jsonFilePath))
        {
            Debug.LogWarning("JSON file not found. Starting with default state.");
            return;
        }

        string json = File.ReadAllText(jsonFilePath);

        SkillTreeGraphSaveData data = JsonUtility.FromJson<SkillTreeGraphSaveData>(json);
        if (data == null || data.nodes == null)
        {
            Debug.LogWarning("JSON file is empty or invalid.");
            return;
        }

        // Clear the existing list so we can rebuild it from JSON
        allNodes.Clear();

        // Dictionary to quickly find newly created nodes by GUID
        Dictionary<string, SkillTreeNode> nodeLookup = new Dictionary<string, SkillTreeNode>();

        foreach (SkillNodeData nodeData in data.nodes)
        {
            if (string.IsNullOrEmpty(nodeData.GUID))
            {
                Debug.LogWarning("Skipping a node in saved data with a null or empty GUID ");
                continue;
            }

            // Create a new in-memory ScriptableObject or an instance of SkillTreeNode
            SkillTreeNode newNode = ScriptableObject.CreateInstance<SkillTreeNode>();
            newNode.uniqueID = nodeData.GUID;
            newNode.nodeName = nodeData.title;
            newNode.positionX = nodeData.posX;
            newNode.positionY = nodeData.posY;
            newNode.cost = nodeData.cost;
            newNode.isUnlocked = nodeData.isUnlocked;

            // If there's a buffDataPath, attempt to load the asset (Editor-only)
            // If you need a runtime approach, use Resources.Load or Addressables
            if (!string.IsNullOrEmpty(nodeData.buffDataPath))
            {
                BuffData buff = UnityEditor.AssetDatabase.LoadAssetAtPath<BuffData>(nodeData.buffDataPath);
                newNode.buffData = buff;
            }

            // We'll link prerequisites in a second pass
            newNode.prerequisites = new List<SkillTreeNode>();

            // Add this node to the manager's list and to our lookup
            allNodes.Add(newNode);
            nodeLookup[newNode.uniqueID] = newNode;
        }

        // Second pass: link prerequisites by GUID
        foreach (SkillNodeData nodeData in data.nodes)
        {
            if (string.IsNullOrEmpty(nodeData.GUID)) continue;
            if (!nodeLookup.TryGetValue(nodeData.GUID, out SkillTreeNode thisNode))
            {
                continue;
            }

            // For each prerequisite ID, find the node in nodeLookup
            if (nodeData.prerequisiteIDs != null)
            {
                foreach (string preGuid in nodeData.prerequisiteIDs)
                {
                    if (nodeLookup.TryGetValue(preGuid, out SkillTreeNode prereqNode))
                    {
                        thisNode.prerequisites.Add(prereqNode);
                    }
                    else
                    {
                        Debug.LogWarning($"Prerequisite node with GUID '{preGuid}' not found for node '{thisNode.nodeName}'");
                    }
                }
            }
        }

        Debug.Log($"Skill tree data loaded from {jsonFilePath}, created {allNodes.Count} nodes");
    }

}
