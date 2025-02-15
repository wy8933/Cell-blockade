using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class SkillTreeEditorWindow : EditorWindow
{
    private SkillTreeGraphView graphView;

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

        Button addNodeButton = new Button(() => { graphView.CreateNode("New Skill"); });
        addNodeButton.text = "Add Skill Node";
        toolbar.Add(addNodeButton);

        Button saveButton = new Button(() => { graphView.SaveGraph(); });
        saveButton.text = "Save Graph";
        toolbar.Add(saveButton);

        Button loadButton = new Button(() => { graphView.LoadGraph(); });
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
