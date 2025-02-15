#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Recorder.OutputPath;


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
        Button loadButton = root.Q<Button>("LoadButton");
        Button saveButton = root.Q<Button>("SaveButton");

        if (saveButton != null)
        {
            saveButton.clicked -= OnSaveButtonClicked;
            saveButton.clicked += OnSaveButtonClicked;
        }
        if (loadButton != null)
        {
            loadButton.clicked -= OnLoadModuleClicked;
            loadButton.clicked += OnLoadModuleClicked;
        }
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

        LoadModules(typeof(ModifyStatsBuffModule));
        GenerateListView();
    }

    private void OnLoadModuleClicked()
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("All Modules"), false, () => LoadModules(null));
        menu.AddItem(new GUIContent("Modify Stats Module"), false, () => LoadModules(typeof(ModifyStatsBuffModule)));

        // Future Module Types: Add new menu options here when adding new Buff Modules
        // menu.AddItem(new GUIContent("NewBuffModuleType"), false, () => LoadModules(typeof(NewBuffModuleType)));

        menu.ShowAsContext();
    }


    private void LoadModules(Type moduleType)
    {
        moduleList.Clear();

        // Find all BaseBuffModule
        string[] guids = AssetDatabase.FindAssets("t:BaseBuffModule");
        Debug.Log("Found " + guids.Length + " BaseBuffModule.");

        // Load all the buff modules
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            BaseBuffModule asset = AssetDatabase.LoadAssetAtPath<BaseBuffModule>(path);

            Debug.Log("Found Buff Module: " + asset.moduleName + " | Type: " + asset.GetType());

            // If no specific type is selected, load all modules; otherwise, filter
            if (moduleType == null || asset.GetType() == moduleType)
            {
                moduleList.Add(asset);
            }
            
        }

        Debug.Log("Loaded " + moduleList.Count + " Buff Modules.");
        moduleListView.itemsSource = moduleList;
        moduleListView.Rebuild();
    }


    private void GenerateListView()
    {
        moduleListView.itemsSource = moduleList;
    
        moduleListView.makeItem = () =>
        {
            var row = moduleRowTemplate.CloneTree();
            if (row == null)
            {
                Debug.LogError("moduleRowTemplate is null! Check BuffModuleRowTemplate.uxml.");
            }
            return row;
        };

        moduleListView.bindItem = (element, index) =>
        {
            if (index < 0 || index >= moduleList.Count)
            {
                Debug.LogError($"Invalid index {index}! Module list has {moduleList.Count} items.");
                return;
            }

            BaseBuffModule module = moduleList[index];

            // Find the label inside the row template
            Label moduleNameLabel = element.Q<Label>("BuffModuleName");
            if (moduleNameLabel != null)
            {
                moduleNameLabel.text = module.moduleName;
            }
            else
            {
                Debug.LogError("ModuleName label not found in row template! Check BuffModuleRowTemplate.uxml.");
            }
        };

        moduleListView.onSelectionChange += OnListSelectionChange;
        moduleListView.fixedItemHeight = 60;
        moduleListView.Rebuild();
    }


    private void OnAddModuleClicked()
    {
        // Ask the user which module type they want to create
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Modify Stats Module"), false, () => CreateModule(typeof(ModifyStatsBuffModule)));

        menu.ShowAsContext();
    }

    private void CreateModule(Type moduleType)
    {
        // Ensure the module type is a subclass of BaseBuffModule
        if (!typeof(BaseBuffModule).IsAssignableFrom(moduleType))
        {
            Debug.LogError("Invalid module type: " + moduleType.Name);
            return;
        }

        // Create an instance of the selected module type
        BaseBuffModule newModule = (BaseBuffModule)ScriptableObject.CreateInstance(moduleType);
        newModule.moduleName = "New " + moduleType.Name;

        // Ensure the directory exists
        string folderPath = "Assets/ScriptableObject/Buff/BuffModule/";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("Invalid module path: " + folderPath);
        }

        // Generate a unique path for the asset
        string path = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{newModule.moduleName}.asset");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Failed to generate a valid asset path.");
            return;
        }

        // Save the new module as an asset
        AssetDatabase.CreateAsset(newModule, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Created new module: " + newModule.moduleName + " at " + path);

        // Refresh the editor UI
        LoadModules(typeof(ModifyStatsBuffModule));
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
            ShowModuleDetails(module); 
        });
        moduleDetailsSection.Add(moduleTypeField);

        // Show properties based on module type
        if (module is ModifyStatsBuffModule statsModule)
        {
            Label statsLabel = new Label("Modify Stats Values");
            statsLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            moduleDetailsSection.Add(statsLabel);

            // Use the class reference directly
            PlayerStats stats = statsModule.stats;

            FloatField maxHealthField = AddStatField(moduleDetailsSection, "Max Health", stats, nameof(PlayerStats.MaxHealth));
            FloatField healthField = AddStatField(moduleDetailsSection, "Health", stats, nameof(PlayerStats.Health));
            FloatField movementSpeedField = AddStatField(moduleDetailsSection, "Movement Speed", stats, nameof(PlayerStats.MovementSpeed));
            FloatField sprintSpeedField = AddStatField(moduleDetailsSection, "Sprint Speed", stats, nameof(PlayerStats.SprintSpeed));
            FloatField resistanceField = AddStatField(moduleDetailsSection, "Resistance", stats, nameof(PlayerStats.Resistance));

            FloatField shieldField = AddStatField(moduleDetailsSection, "Shield", stats, nameof(PlayerStats.Shield));
            FloatField damageReductionField = AddStatField(moduleDetailsSection, "Damage Reduction", stats, nameof(PlayerStats.DamageReduction));
            FloatField blockChanceField = AddStatField(moduleDetailsSection, "Block Chance", stats, nameof(PlayerStats.BlockChance));

            FloatField slowResistanceField = AddStatField(moduleDetailsSection, "Slow Resistance", stats, nameof(PlayerStats.SlowResistance));

            FloatField sizeMultiplierField = AddStatField(moduleDetailsSection, "Size Multiplier", stats, nameof(PlayerStats.SizeMultiplier));
            FloatField healthMultiplierField = AddStatField(moduleDetailsSection, "Health Multiplier", stats, nameof(PlayerStats.HealthMultiplier));
            FloatField atkMultiplierField = AddStatField(moduleDetailsSection, "Attack Multiplier", stats, nameof(PlayerStats.AtkMultiplier));
            FloatField damageReductionMultiplierField = AddStatField(moduleDetailsSection, "Damage Reduction Multiplier", stats, nameof(PlayerStats.DamageReductionMultiplier));
            FloatField resistanceMultiplierField = AddStatField(moduleDetailsSection, "Resistance Multiplier", stats, nameof(PlayerStats.ResistanceMultiplier));
            FloatField speedMultiplierField = AddStatField(moduleDetailsSection, "Speed Multiplier", stats, nameof(PlayerStats.SpeedMultiplier));
            FloatField goldDropMultiplierField = AddStatField(moduleDetailsSection, "Gold Drop Multiplier", stats, nameof(PlayerStats.GoldDropMultiplier));
        }
    }

    private FloatField AddStatField(VisualElement parent, string label, PlayerStats stats, string statName)
    {
        FloatField statField = new FloatField(label);

        // Get the field 
        var fieldInfo = typeof(PlayerStats).GetField(statName);
        if (fieldInfo == null)
        {
            Debug.LogError($"Field {statName} not found in PlayerStats!");
            return null;
        }

        // Get the current value and set it to the UI field
        float currentValue = (float)fieldInfo.GetValue(stats);
        statField.value = currentValue;

        // Register callback to update the correct stat field in PlayerStats
        statField.RegisterValueChangedCallback(evt =>
        {
            fieldInfo.SetValue(stats, evt.newValue);
            EditorUtility.SetDirty(activeModule); 
        });

        parent.Add(statField);
        return statField;
    }

    private void OnSaveButtonClicked()
    {
        if (activeModule == null)
        {
            Debug.LogWarning("No active module selected to save.");
            return;
        }

        Debug.Log("Saving module: " + activeModule.moduleName);

        // Mark ScriptableObject dirty to persist changes
        EditorUtility.SetDirty(activeModule);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RenameActiveBuffAsset();

        // Refresh ListView after renaming
        moduleListView.Rebuild();

        Debug.Log("Module saved successfully: " + activeModule.moduleName);
    }


    private void RenameActiveBuffAsset()
    {
        string assetPath = AssetDatabase.GetAssetPath(activeModule);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("Asset path not found for buff: " + activeModule.moduleName);
            return;
        }

        // Rename asset
        string errorMessage = AssetDatabase.RenameAsset(assetPath, "BuffModule_" + activeModule.moduleName);

        if (string.IsNullOrEmpty(errorMessage))
        {
            Debug.Log("Renamed buff asset to: " + activeModule.moduleName);
            EditorUtility.SetDirty(activeModule);
            AssetDatabase.SaveAssets();
        }
        else
        {
            Debug.LogError("Failed to rename buff asset: " + errorMessage);
        }
    }
}
#endif
