#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BuffEditorWindow : EditorWindow
{
    private List<BuffData> buffList = new List<BuffData>();
    private VisualTreeAsset buffRowTemplate;
    private ListView buffListView;
    private ScrollView buffDetailsSection;
    private BuffData activeBuff;

    [MenuItem("Custom Editor/Buff Editor")]
    public static void ShowWindow()
    {
        BuffEditorWindow wnd = GetWindow<BuffEditorWindow>();
        wnd.titleContent = new GUIContent("Buff Editor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Load the main Buff Editor UI
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BuffEditorWindow.uxml");
        if (visualTree == null)
        {
            Debug.LogError(" Failed to load BuffEditorWindow.uxml");
            return;
        }
        root.Add(visualTree.CloneTree());

        // Load the row template
        buffRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BuffRowTemplate.uxml");
        if (buffRowTemplate == null)
        {
            Debug.LogError(" Failed to load BuffRowTemplate.uxml");
            return;
        }

        // Ensure Buff ListView exists in the UI
        buffListView = root.Q<ListView>("BuffListView");
        if (buffListView == null)
        {
            Debug.LogError(" Failed to find 'BuffListView' in BuffEditorWindow.uxml");
            return;
        }

        // Ensure Buff Details Section exists
        buffDetailsSection = root.Q<ScrollView>("BuffDetailsSection");
        if (buffDetailsSection == null)
        {
            Debug.LogError(" Failed to find 'BuffDetailsSection' in BuffEditorWindow.uxml");
            return;
        }

        // Assign Buttons
        Button addButton = root.Q<Button>("AddButton");
        Button deleteButton = root.Q<Button>("DeleteButton");

        if (addButton != null)
        {
            addButton.clicked -= OnAddBuffClicked;
            addButton.clicked += OnAddBuffClicked;
        }
        if (deleteButton != null)
        {
            deleteButton.clicked -= OnDeleteBuffClicked;
            deleteButton.clicked += OnDeleteBuffClicked;
        }

        // Load BuffData assets
        LoadBuffs();

        // Ensure there is data before generating UI
        if (buffList.Count == 0)
        {
            Debug.LogWarning("No BuffData assets found");
        }

        GenerateListView();
    }


    private void LoadBuffs()
    {
        if (buffList == null)
        {
            buffList = new List<BuffData>();
        }
        else
        {
            buffList.Clear();
        }

        string[] guids = AssetDatabase.FindAssets("t:BuffData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            BuffData buff = AssetDatabase.LoadAssetAtPath<BuffData>(path);
            if (buff != null)
                buffList.Add(buff);
        }

        if (buffList.Count == 0)
        {
            Debug.LogWarning("No BuffData assets found. Please create at least one BuffData asset.");
        }
    }


    private void GenerateListView()
    {
        if (buffListView == null)
        {
            Debug.LogError(" buffListView is null in GenerateListView! Ensure it's properly assigned in CreateGUI().");
            return;
        }

        if (buffRowTemplate == null)
        {
            Debug.LogError(" buffRowTemplate is null! Ensure it's correctly assigned in CreateGUI().");
            return;
        }

        if (buffList == null)
        {
            Debug.LogError(" buffList is null! Ensure LoadBuffs() is called before this method.");
            buffList = new List<BuffData>();
        }

        buffListView.itemsSource = buffList;

        // Ensure makeItem() does not return null
        buffListView.makeItem = () =>
        {
            var row = buffRowTemplate.CloneTree();
            if (row == null)
            {
                Debug.LogError(" makeItem() returned null! Check if buffRowTemplate is assigned properly.");
            }
            return row;
        };

        buffListView.bindItem = (element, index) =>
        {
            if (index < 0 || index >= buffList.Count)
            {
                Debug.LogError($"Invalid index {index} in bindItem! Buff list has {buffList.Count} items.");
                return;
            }

            BuffData buff = buffList[index];
            if (buff == null)
            {
                Debug.LogError($"BuffData at index {index} is null! Check LoadBuffs()");
                return;
            }

            Label buffNameLabel = element.Q<Label>("BuffName");
            if (buffNameLabel != null)
            {
                buffNameLabel.text = buff.buffName;
            }
            else
            {
                Debug.LogError("BuffName label not found in row template");
            }
        };

        buffListView.fixedItemHeight = 40;
        buffListView.onSelectionChange += OnListSelectionChange;
    }

    private void RenameActiveBuffAsset()
    {
        string assetPath = AssetDatabase.GetAssetPath(activeBuff);
        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.LogError("Asset path not found for buff: " + activeBuff.buffName);
            return;
        }

        // Rename asset
        string errorMessage = AssetDatabase.RenameAsset(assetPath, "BuffData_" + activeBuff.buffName);

        if (string.IsNullOrEmpty(errorMessage))
        {
            Debug.Log("Renamed buff asset to: " + activeBuff.buffName);
            EditorUtility.SetDirty(activeBuff);
            AssetDatabase.SaveAssets();
        }
        else
        {
            Debug.LogError("Failed to rename buff asset: " + errorMessage);
        }
    }


    private void OnAddBuffClicked()
    {
        // Create a new BuffData asset
        BuffData newBuff = ScriptableObject.CreateInstance<BuffData>();
        newBuff.buffName = "New Buff";
        newBuff.id = buffList.Count > 0 ? buffList.Max(b => b.id) + 1 : 1; // Auto-increment ID

        // Save asset in Unity project
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/ScriptableObject/Buff/BuffData/NewBuff.asset");
        AssetDatabase.CreateAsset(newBuff, path);
        AssetDatabase.SaveAssets();

        // Add to list and refresh UI
        buffList.Add(newBuff);
        buffListView.Rebuild();

        Debug.Log("Created new buff: " + newBuff.buffName);
    }


    private void OnDeleteBuffClicked()
    {
        if (activeBuff == null)
        {
            Debug.LogWarning("No buff selected for deletion.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(activeBuff);

        // Remove from list and delete the asset
        buffList.Remove(activeBuff);
        AssetDatabase.DeleteAsset(assetPath);
        AssetDatabase.SaveAssets();

        // Clear UI if no buffs are left
        if (buffList.Count == 0)
        {
            buffDetailsSection.visible = false;
            activeBuff = null;
        }

        // Refresh UI
        buffListView.Rebuild();

        Debug.Log("Deleted buff: " + activeBuff?.buffName);
    }

    private void OnListSelectionChange(IEnumerable<object> selectedItems)
    {
        activeBuff = selectedItems.First() as BuffData;
        if (activeBuff != null)
        {
            ShowBuffDetails(activeBuff);
            buffDetailsSection.visible = true;
        }
    }

    private void ShowBuffDetails(BuffData buff)
    {
        // Set active buff reference
        activeBuff = buff;

        // Ensure the UI is visible
        buffDetailsSection.visible = true;

        // Get UI elements
        TextField nameField = buffDetailsSection.Q<TextField>("BuffName");
        IntegerField idField = buffDetailsSection.Q<IntegerField>("BuffID");
        TextField descriptionField = buffDetailsSection.Q<TextField>("Description");
        ObjectField iconField = buffDetailsSection.Q<ObjectField>("BuffIcon");

        IntegerField maxStackField = buffDetailsSection.Q<IntegerField>("MaxStack");
        IntegerField priorityField = buffDetailsSection.Q<IntegerField>("Priority");
        Toggle foreverToggle = buffDetailsSection.Q<Toggle>("IsForever");
        FloatField durationField = buffDetailsSection.Q<FloatField>("Duration");
        FloatField tickTimeField = buffDetailsSection.Q<FloatField>("TickTime");
        EnumField buffUpdateField = buffDetailsSection.Q<EnumField>("BuffUpdateType");
        EnumField buffRemoveField = buffDetailsSection.Q<EnumField>("BuffRemoveStackType");

        ObjectField onCreateField = buffDetailsSection.Q<ObjectField>("OnCreate");
        ObjectField onRemoveField = buffDetailsSection.Q<ObjectField>("OnRemove");
        ObjectField onTickField = buffDetailsSection.Q<ObjectField>("OnTick");
        ObjectField onHitField = buffDetailsSection.Q<ObjectField>("OnHit");
        ObjectField onHurtField = buffDetailsSection.Q<ObjectField>("OnHurt");
        ObjectField onKillField = buffDetailsSection.Q<ObjectField>("OnKill");
        ObjectField onDeathField = buffDetailsSection.Q<ObjectField>("OnDeath");

        Button saveButton = buffDetailsSection.Q<Button>("SaveButton");

        if (saveButton == null)
        {
            Debug.LogError("SaveButton not found in BuffEditorWindow.uxml! Check UI Builder.");
            return;
        }

        // Assign Buff Values to UI Fields (without saving immediately)
        nameField.SetValueWithoutNotify(buff.buffName);
        idField.SetValueWithoutNotify(buff.id);
        descriptionField.SetValueWithoutNotify(buff.description);
        iconField.SetValueWithoutNotify(buff.icon);

        maxStackField.SetValueWithoutNotify(buff.maxStack);
        priorityField.SetValueWithoutNotify(buff.priority);
        foreverToggle.SetValueWithoutNotify(buff.isForever);
        durationField.SetValueWithoutNotify(buff.duration);
        tickTimeField.SetValueWithoutNotify(buff.tickTime);;

        buffUpdateField.Init(buff.buffUpdateType);
        buffUpdateField.SetValueWithoutNotify(buff.buffUpdateType);

        buffRemoveField.Init(buff.buffRemoveStackType);
        buffRemoveField.SetValueWithoutNotify(buff.buffRemoveStackType);

        onCreateField.SetValueWithoutNotify(buff.OnCreate);
        onRemoveField.SetValueWithoutNotify(buff.OnRemove);
        onTickField.SetValueWithoutNotify(buff.OnTick);
        onHitField.SetValueWithoutNotify(buff.OnHit);
        onHurtField.SetValueWithoutNotify(buff.OnHurt);
        onKillField.SetValueWithoutNotify(buff.OnKill);
        onDeathField.SetValueWithoutNotify(buff.OnDeath);

        // Store user edits but don't apply them yet
        saveButton.clicked -= () => { };
        saveButton.clicked += () =>
        {
            Debug.Log("Saving Buff: " + activeBuff.buffName);

            activeBuff.buffName = nameField.value;
            activeBuff.id = idField.value;
            activeBuff.description = descriptionField.value;
            activeBuff.icon = (Sprite)iconField.value;

            activeBuff.maxStack = maxStackField.value;
            activeBuff.priority = priorityField.value;
            activeBuff.isForever = foreverToggle.value;
            activeBuff.duration = durationField.value;
            activeBuff.tickTime = tickTimeField.value;
            activeBuff.buffUpdateType = (BuffUpdateTimeType)buffUpdateField.value;
            activeBuff.buffRemoveStackType = (BuffRemoveStackUpdateType)buffRemoveField.value;

            activeBuff.OnCreate = (BaseBuffModule)onCreateField.value;
            activeBuff.OnRemove = (BaseBuffModule)onRemoveField.value;
            activeBuff.OnTick = (BaseBuffModule)onTickField.value;
            activeBuff.OnHit = (BaseBuffModule)onHitField.value;
            activeBuff.OnHurt = (BaseBuffModule)onHurtField.value;
            activeBuff.OnKill = (BaseBuffModule)onKillField.value;
            activeBuff.OnDeath = (BaseBuffModule)onDeathField.value;

            // Save the changes
            SaveBuff();
        };

        // Force UI to refresh
        buffDetailsSection.MarkDirtyRepaint();
    }

    private void SaveBuff()
    {
        if (activeBuff == null)
        {
            Debug.LogError("SaveBuff() called but no active buff is selected!");
            return;
        }

        // Save the asset changes
        EditorUtility.SetDirty(activeBuff);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        RenameActiveBuffAsset();
        // Refresh ListView so the new buff name appears if changed
        buffListView.Rebuild();

        Debug.Log("Buff saved successfully: " + activeBuff.buffName);
    }
}
#endif
