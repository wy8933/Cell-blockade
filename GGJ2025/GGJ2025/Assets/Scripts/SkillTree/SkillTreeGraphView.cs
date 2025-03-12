#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SkillTreeGraphView : GraphView
{
    public string filePath = "Assets/Saves/SkillTreeGraphData.json";

    public SkillTreeGraphView()
    {
        // Load the style sheet from Resources
        StyleSheet styleSheet = Resources.Load<StyleSheet>("SkillTreeGraphViewStyle");
        if (styleSheet != null)
        {
            styleSheets.Add(styleSheet);
        }
        else
        {
            Debug.LogWarning("StyleSheet 'SkillTreeGraphViewStyle' not found.");
        }

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        // Enable dragging and selection
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        // Add a grid background for visual guidance
        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        // Register callback to process new edge connections
        this.graphViewChanged += OnGraphViewChanged;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(port => port != startPort && port.direction != startPort.direction).ToList();
    }

    /// <summary>
    /// Creates a new skill node element
    /// </summary>
    /// <param name="nodeName">The display name for the new node</param>
    public void CreateNode(string nodeName)
    {
        SkillTreeNodeElement nodeElement = new SkillTreeNodeElement
        {
            title = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        // Set a default size and position for the node
        nodeElement.SetPosition(new Rect(Vector2.zero, new Vector2(150, 200)));
        AddElement(nodeElement);
    }

    public void ConnectNodes(Port outputPort, Port inputPort)
    {
        // Create the edge and assign ports
        Edge edge = new Edge
        {
            output = outputPort,
            input = inputPort
        };

        // Connect the edge to both ports
        edge.input.Connect(edge);
        edge.output.Connect(edge);

        // Add the edge to the GraphView so it appears visually
        AddElement(edge);
    }

    public GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
        if (change.edgesToCreate != null)
        {
            Debug.Log($"Creating {change.edgesToCreate.Count} new edge(s).");
            foreach (Edge edge in change.edgesToCreate)
            {
                edge.input.Connect(edge);
                edge.output.Connect(edge);
            }
        }
        return change;
    }

    /// <summary>
    /// Saves the current graph data to a JSON file
    /// </summary>
    public void SaveGraph()
    {
        SkillTreeGraphSaveData saveData = new SkillTreeGraphSaveData();
        saveData.nodes = new List<SkillNodeData>();
        saveData.connections = new List<SkillConnectionData>();

        // Create node data objects for each node
        // and store them in a dictionary for easy lookup by GUID
        Dictionary<string, SkillNodeData> nodeDict = new Dictionary<string, SkillNodeData>();

        foreach (var node in nodes.ToList())
        {
            if (node is SkillTreeNodeElement skillNode)
            {
                SkillNodeData nodeData = new SkillNodeData();
                nodeData.GUID = skillNode.GUID;
                nodeData.title = skillNode.title;
                Rect nodeRect = skillNode.GetPosition();
                nodeData.posX = nodeRect.x;
                nodeData.posY = nodeRect.y;
                nodeData.cost = skillNode.cost;
                nodeData.buffDataPath = (skillNode.buffData != null)
                    ? AssetDatabase.GetAssetPath(skillNode.buffData)
                    : "";
                nodeData.isUnlocked = false;
                nodeData.prerequisiteIDs = new List<string>();

                saveData.nodes.Add(nodeData);
                nodeDict[nodeData.GUID] = nodeData;
            }
        }

        foreach (var edge in edges.ToList())
        {
            SkillTreeNodeElement outputNode = edge.output.node as SkillTreeNodeElement;
            SkillTreeNodeElement inputNode = edge.input.node as SkillTreeNodeElement;
            if (outputNode != null && inputNode != null)
            {
                // Store the edge in connections
                SkillConnectionData connectionData = new SkillConnectionData();
                connectionData.outputNodeGUID = outputNode.GUID;
                connectionData.inputNodeGUID = inputNode.GUID;
                saveData.connections.Add(connectionData);

                // Also set prerequisites: "input node" has "output node" as a prerequisite
                if (nodeDict.TryGetValue(inputNode.GUID, out SkillNodeData inputNodeData))
                {
                    inputNodeData.prerequisiteIDs.Add(outputNode.GUID);
                }
            }
        }

        // Serialize to JSON and write to file
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Graph saved to " + filePath);
    }

    /// <summary>
    /// Loads graph data from a JSON file
    /// </summary>
    public void LoadGraph()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found: " + filePath);
            return;
        }

        // Clear existing nodes and edges
        foreach (var element in graphElements.ToList())
        {
            RemoveElement(element);
        }

        string json = File.ReadAllText(filePath);
        SkillTreeGraphSaveData saveData = JsonUtility.FromJson<SkillTreeGraphSaveData>(json);

        // Recreate nodes
        Dictionary<string, SkillTreeNodeElement> nodeLookup = new Dictionary<string, SkillTreeNodeElement>();
        foreach (var nodeData in saveData.nodes)
        {
            SkillTreeNodeElement nodeElement = new SkillTreeNodeElement();
            nodeElement.GUID = nodeData.GUID;
            nodeElement.title = nodeData.title;
            Rect rect = new Rect(nodeData.posX, nodeData.posY, 150, 200);
            nodeElement.SetPosition(rect);

            // Restore skill name in the TextFiele
            if (nodeElement.skillNameField != null)
            {
                nodeElement.skillNameField.value = nodeData.title;
            }

            // Restore cost value
            nodeElement.cost = nodeData.cost;
            if (nodeElement.costField != null)
            {
                nodeElement.costField.value = nodeData.cost;
            }

            // Restore BuffData asset if available
            if (!string.IsNullOrEmpty(nodeData.buffDataPath))
            {
                nodeElement.buffData = AssetDatabase.LoadAssetAtPath<BuffData>(nodeData.buffDataPath);
                // Update the ObjectField so it shows the loaded BuffData
                if (nodeElement.buffObjectField != null)
                {
                    nodeElement.buffObjectField.value = nodeElement.buffData;
                }
            }

            AddElement(nodeElement);
            nodeLookup[nodeElement.GUID] = nodeElement;
        }

        // Recreate edges (connections)
        foreach (var connectionData in saveData.connections)
        {
            if (string.IsNullOrEmpty(connectionData.outputNodeGUID) || string.IsNullOrEmpty(connectionData.inputNodeGUID))
            {
                Debug.LogWarning("Skipping connection with null or empty GUID");
                continue;
            }

            if (nodeLookup.TryGetValue(connectionData.outputNodeGUID, out SkillTreeNodeElement outputNode) &&
                nodeLookup.TryGetValue(connectionData.inputNodeGUID, out SkillTreeNodeElement inputNode))
            {
                // Assume first output/input port is used
                Port outputPort = outputNode.outputContainer[0] as Port;
                Port inputPort = inputNode.inputContainer[0] as Port;
                Edge edge = new Edge
                {
                    output = outputPort,
                    input = inputPort
                };
                edge.input.Connect(edge);
                edge.output.Connect(edge);
                AddElement(edge);
            }
        }

        Debug.Log("Graph loaded from " + filePath);
    }
}
#endif