#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class SkillTreeEditorWindow : EditorWindow
{
    private SkillTreeGraphView graphView;
    private string currentSkillTreeName = "SkillTreeGraphData"; // Default name without .json
    private List<string> availableSkillTrees = new List<string> { "SkillTreeGraphData", "SkillTree1", "SkillTree2" };

    [MenuItem("Custom Editor/Skill Tree Editor")]
    public static void OpenSkillTreeEditor()
    {
        SkillTreeEditorWindow window = GetWindow<SkillTreeEditorWindow>();
        window.titleContent = new GUIContent("Skill Tree Editor");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        GenerateMiniMap();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void ConstructGraphView()
    {
        graphView = new SkillTreeGraphView
        {
            name = "Skill Tree Graph"
        };

        graphView.StretchToParentSize();
        rootVisualElement.Add(graphView);
    }

    private void GenerateToolbar()
    {
        Toolbar toolbar = new Toolbar();

        // Add Skill Node Button
        Button addNodeButton = new Button(() => { graphView.CreateNode("New Skill"); });
        addNodeButton.text = "Add Skill Node";
        toolbar.Add(addNodeButton);

        // Dropdown for selecting skill tree
        DropdownField skillTreeDropdown = new DropdownField("Select Skill Tree", availableSkillTrees, 0);
        skillTreeDropdown.RegisterValueChangedCallback(evt =>
        {
            currentSkillTreeName = evt.newValue;
            // Update the file path to use the "Assets/Saves" folder
            graphView.filePath = "Assets/Saves/" + currentSkillTreeName + ".json";
            Debug.Log("Selected Skill Tree: " + currentSkillTreeName);
        });
        toolbar.Add(skillTreeDropdown);

        // Save Button with confirmation pop-up
        Button saveButton = new Button(() =>
        {
            bool confirm = EditorUtility.DisplayDialog("Confirm Save", "Are you sure you want to save the current skill tree?", "Yes", "No");
            if (confirm)
            {
                graphView.SaveGraph();
            }
        });
        saveButton.text = "Save Graph";
        toolbar.Add(saveButton);

        // Load Button with confirmation pop-up
        Button loadButton = new Button(() =>
        {
            bool confirm = EditorUtility.DisplayDialog("Confirm Load", "Are you sure you want to load the selected skill tree? This will clear the current graph.", "Yes", "No");
            if (confirm)
            {
                graphView.LoadGraph();
            }
        });
        loadButton.text = "Load Graph";
        toolbar.Add(loadButton);

        rootVisualElement.Add(toolbar);
    }

    private void GenerateMiniMap()
    {
        MiniMap miniMap = new MiniMap { anchored = true };
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        graphView.Add(miniMap);
    }
}
#endif