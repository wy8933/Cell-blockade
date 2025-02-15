#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


public class BuffModuleEditorWindow : EditorWindow
{
    private List<BaseBuffModule> moduleList = new List<BaseBuffModule>();
    private VisualTreeAsset moduleRowTemplate;
    private ListView moduleListView;
    private ScrollView moduleDetailsSection;
    private BaseBuffModule activeModule;

    [MenuItem("Custom Editor/Buff Module Editor")]
    public static void ShowWindow()
    {
        BuffModuleEditorWindow wnd = GetWindow<BuffModuleEditorWindow>();
        wnd.titleContent = new GUIContent("Buff Module Editor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BuffModuleEditorWindow.uxml");
        if (visualTree == null)
        {
            Debug.LogError("Failed to load BuffModuleEditorWindow.uxml");
            return;
        }
        root.Add(visualTree.CloneTree());

        moduleRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BuffModuleRowTemplate.uxml");
        if (moduleRowTemplate == null)
        {
            Debug.LogError("Failed to load BuffModuleRowTemplate.uxml");
            return;
        }

        moduleListView = root.Q<ListView>("BuffModuleListView");
        if (moduleListView == null)
        {
            Debug.LogError("Failed to find 'BuffModuleListView' in BuffModuleEditorWindow.uxml");
            return;
        }

        moduleDetailsSection = root.Q<ScrollView>("BuffModuleDetailsSection");
        if (moduleDetailsSection == null)
        {
            Debug.LogError("Failed to find 'BuffModuleDetailsSection' in BuffModuleEditorWindow.uxml");
            return;
        }

        Button addButton = root.Q<Button>("AddButton");
        Button deleteButton = root.Q<Button>("DeleteButton");

        if (addButton != null)
        {
            addButton.clicked -= OnAddModuleClicked;
            addButton.clicked += OnAddModuleClicked;
        }
        if (deleteButton != null)
        {
            deleteButton.clicked -= OnDeleteModuleClicked;
            deleteButton.clicked += OnDeleteModuleClicked;
        }

        LoadModules();
        GenerateListView();
    }

    private void LoadModules()
    {
        moduleList.Clear();
        string[] guids = AssetDatabase.FindAssets("t:BaseBuffModule");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            BaseBuffModule module = AssetDatabase.LoadAssetAtPath<BaseBuffModule>(path);
            if (module != null)
                moduleList.Add(module);
        }
    }

    private void GenerateListView()
    {
        moduleListView.itemsSource = moduleList;
        moduleListView.makeItem = () => moduleRowTemplate.CloneTree();
        moduleListView.bindItem = (element, index) =>
        {
            BaseBuffModule module = moduleList[index];
            Label moduleNameLabel = element.Q<Label>("ModuleName");
            if (moduleNameLabel != null)
                moduleNameLabel.text = module.moduleName;
        };

        moduleListView.fixedItemHeight = 40;
        moduleListView.onSelectionChange += OnListSelectionChange;
    }

    private void OnAddModuleClicked()
    {
        BaseBuffModule newModule = ScriptableObject.CreateInstance<BaseBuffModule>();
        newModule.moduleName = "New Buff Module";
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObject/Buff/BuffModule/NewBuffModule.asset");
        AssetDatabase.CreateAsset(newModule, path);
        AssetDatabase.SaveAssets();
        moduleList.Add(newModule);
        moduleListView.Rebuild();
    }

    private void OnDeleteModuleClicked()
    {
        if (activeModule != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(activeModule);
            moduleList.Remove(activeModule);
            AssetDatabase.DeleteAsset(assetPath);
            moduleListView.Rebuild();
            moduleDetailsSection.visible = false;
        }
    }

    private void OnListSelectionChange(IEnumerable<object> selectedItems)
    {
        activeModule = selectedItems.First() as BaseBuffModule;
        if (activeModule != null)
        {
            ShowModuleDetails(activeModule);
            moduleDetailsSection.visible = true;
        }
    }

    private void ShowModuleDetails(BaseBuffModule module)
    {
        moduleDetailsSection.visible = true;
        moduleDetailsSection.Clear();

        // Module Name
        TextField nameField = new TextField("Module Name") { value = module.moduleName };
        nameField.RegisterValueChangedCallback(evt => module.moduleName = evt.newValue);
        moduleDetailsSection.Add(nameField);

        // Module Type Selection
        EnumField moduleTypeField = new EnumField("Module Type", module.moduleType);
        moduleTypeField.RegisterValueChangedCallback(evt =>
        {
            module.moduleType = (BuffModuleType)evt.newValue;
            ShowModuleDetails(module); // Refresh UI to show relevant fields
        });
        moduleDetailsSection.Add(moduleTypeField);

        // Show properties based on module type
        if (module is ModifyStatsBuffModule statsModule)
        {
            Label statsLabel = new Label("Modify Stats Values");
            statsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            moduleDetailsSection.Add(statsLabel);

            AddStatField(moduleDetailsSection, "Max Health", statsModule, nameof(PlayerStats.MaxHealth));
            AddStatField(moduleDetailsSection, "Health", statsModule, nameof(PlayerStats.Health));
            AddStatField(moduleDetailsSection, "Movement Speed", statsModule, nameof(PlayerStats.MovementSpeed));
            AddStatField(moduleDetailsSection, "Sprint Speed", statsModule, nameof(PlayerStats.SprintSpeed));
            AddStatField(moduleDetailsSection, "Resistance", statsModule, nameof(PlayerStats.Resistance));

            AddStatField(moduleDetailsSection, "Shield", statsModule, nameof(PlayerStats.Shield));
            AddStatField(moduleDetailsSection, "Damage Reduction", statsModule, nameof(PlayerStats.DamageReduction));
            AddStatField(moduleDetailsSection, "Block Chance", statsModule, nameof(PlayerStats.BlockChance));

            AddStatField(moduleDetailsSection, "Slow Resistance", statsModule, nameof(PlayerStats.SlowResistance));

            AddStatField(moduleDetailsSection, "Size Multiplier", statsModule, nameof(PlayerStats.SizeMultiplier));
            AddStatField(moduleDetailsSection, "Health Multiplier", statsModule, nameof(PlayerStats.HealthMultiplier));
            AddStatField(moduleDetailsSection, "Attack Multiplier", statsModule, nameof(PlayerStats.AtkMultiplier));
            AddStatField(moduleDetailsSection, "Damage Reduction Multiplier", statsModule, nameof(PlayerStats.DamageReductionMultiplier));
            AddStatField(moduleDetailsSection, "Resistance Multiplier", statsModule, nameof(PlayerStats.ResistanceMultiplier));
            AddStatField(moduleDetailsSection, "Speed Multiplier", statsModule, nameof(PlayerStats.SpeedMultiplier));
            AddStatField(moduleDetailsSection, "Gold Drop Multiplier", statsModule, nameof(PlayerStats.GoldDropMultiplier));
        }

        // Save Button
        Button saveButton = new Button(() => SaveModule(module)) { text = "Save Module" };
        moduleDetailsSection.Add(saveButton);
    }

    private void AddStatField(VisualElement parent, string label, ModifyStatsBuffModule module, string statName)
    {
        FloatField statField = new FloatField(label);

        // Get the current value using reflection
        float currentValue = (float)typeof(PlayerStats).GetField(statName).GetValue(module.stats);
        statField.value = currentValue;

        // Register callback to update the correct stat field
        statField.RegisterValueChangedCallback(evt =>
        {
            typeof(PlayerStats).GetField(statName).SetValue(module.stats, evt.newValue);
        });

        parent.Add(statField);
    }

    private void SaveModule(BaseBuffModule module)
    {
        EditorUtility.SetDirty(module);
        AssetDatabase.SaveAssets();
        Debug.Log("Saved module: " + module.moduleName);
    }
}
#endif
