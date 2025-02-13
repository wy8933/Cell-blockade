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



    private void OnAddBuffClicked()
    {
        BuffData newBuff = ScriptableObject.CreateInstance<BuffData>();
        newBuff.buffName = "New Buff";
        string path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewBuff.asset");
        AssetDatabase.CreateAsset(newBuff, path);
        AssetDatabase.SaveAssets();

        buffList.Add(newBuff);
        buffListView.Rebuild();
    }

    private void OnDeleteBuffClicked()
    {
        if (activeBuff != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(activeBuff);
            buffList.Remove(activeBuff);
            AssetDatabase.DeleteAsset(assetPath);
            buffListView.Rebuild();
            buffDetailsSection.visible = false;
        }
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


        // Assign Buff Values to UI Fields
        nameField.SetValueWithoutNotify(buff.buffName);
        idField.SetValueWithoutNotify(buff.id);
        descriptionField.SetValueWithoutNotify(buff.description);
        iconField.SetValueWithoutNotify(buff.icon);

        maxStackField.SetValueWithoutNotify(buff.maxStack);
        priorityField.SetValueWithoutNotify(buff.priority);
        foreverToggle.SetValueWithoutNotify(buff.isForever);
        durationField.SetValueWithoutNotify(buff.duration);
        tickTimeField.SetValueWithoutNotify(buff.tickTime);
        buffUpdateField.SetValueWithoutNotify(buff.buffUpdateType);
        buffRemoveField.SetValueWithoutNotify(buff.buffRemoveStackType);

        onCreateField.SetValueWithoutNotify(buff.OnCreate);
        onRemoveField.SetValueWithoutNotify(buff.OnRemove);
        onTickField.SetValueWithoutNotify(buff.OnTick);
        onHitField.SetValueWithoutNotify(buff.OnHit);
        onHurtField.SetValueWithoutNotify(buff.OnHurt);
        onKillField.SetValueWithoutNotify(buff.OnKill);
        onDeathField.SetValueWithoutNotify(buff.OnDeath);

        // Auto Save Changes on Edit
        nameField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.buffName = evt.newValue));
        idField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.id = evt.newValue));
        descriptionField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.description = evt.newValue));
        iconField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.icon = (Sprite)evt.newValue));

        maxStackField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.maxStack = evt.newValue));
        priorityField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.priority = evt.newValue));
        foreverToggle.RegisterValueChangedCallback(evt => SaveBuffChange(() =>
        {
            buff.isForever = evt.newValue;
            durationField.style.display = tickTimeField.style.display = buff.isForever ? DisplayStyle.None : DisplayStyle.Flex;
        }));

        durationField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.duration = evt.newValue));
        tickTimeField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.tickTime = evt.newValue));
        buffUpdateField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.buffUpdateType = (BuffUpdateTimeType)evt.newValue));
        buffRemoveField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.buffRemoveStackType = (BuffRemoveStackUpdateType)evt.newValue));

        onCreateField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.OnCreate = (BaseBuffModule)evt.newValue));
        onRemoveField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.OnRemove = (BaseBuffModule)evt.newValue));
        onTickField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.OnTick = (BaseBuffModule)evt.newValue));
        onHitField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.OnHit = (BaseBuffModule)evt.newValue));
        onHurtField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.OnHurt = (BaseBuffModule)evt.newValue));
        onKillField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.OnKill = (BaseBuffModule)evt.newValue));
        onDeathField.RegisterValueChangedCallback(evt => SaveBuffChange(() => buff.OnDeath = (BaseBuffModule)evt.newValue));

        // Force UI to refresh
        buffDetailsSection.MarkDirtyRepaint();
    }

    private void SaveBuffChange(Action updateBuffAction)
    {
        // Apply the change to the selected buff
        updateBuffAction?.Invoke();

        // Mark the asset dirty so Unity knows it changed
        EditorUtility.SetDirty(activeBuff);
        AssetDatabase.SaveAssets();
    }


}
#endif
