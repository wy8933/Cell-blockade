using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IntReference", menuName = "Variables/IntReference")]
public class IntReference : ScriptableObject
{
    [Header("Values")]
    [Tooltip("Value used at the start of the game or when resetting")]
    public int initialValue;

    [Tooltip("Current value at runtime")]
    [SerializeField]
    private int runtimeValue;

    [Header("Debug Settings")]
    [Tooltip("If enabled, logs changes to the console")]
    public bool debugEnabled;

    public enum SaveOn
    {
        None,
        SceneLoaded,
    }

    [Header("Save/Reset Options")]
    [Tooltip("When the value should be reset to 'initialValue'")]
    public SaveOn saveOn = SaveOn.SceneLoaded;

    [Tooltip("Check this if you want to reset to 'initialValue' at the specified time")]
    public bool resetToInitialValue;

    [Header("Value Changed Event")]
    [Tooltip("Event invoked whenever 'runtimeValue' changes")]
    public UnityEvent<int> OnValueChanged;


    private void OnEnable()
    {
        // Only reset if the game is actually playing and you've enabled reset
        if (resetToInitialValue && saveOn == SaveOn.SceneLoaded)
        {
            ResetValue();
        }
    }


    /// <summary>
    /// Access or modify the current runtime value, invokes OnValueChanged if it changes
    /// </summary>
    public int Value
    {
        get => runtimeValue;
        set
        {
            // If there's no actual change, do nothing
            if (runtimeValue == value) return;

            runtimeValue = value;
            OnValueChanged?.Invoke(runtimeValue);

            if (debugEnabled)
            {
                Debug.Log($"[IntVariable] New Value: {runtimeValue}", this);
            }
        }
    }

    /// <summary>
    /// Sets the runtime value directly, triggering the OnValueChanged event
    /// </summary>
    public void SetValue(int newValue)
    {
        Value = newValue;
    }

    /// <summary>
    /// Resets the runtime value to the initial value, triggering the OnValueChanged event
    /// </summary>
    public void ResetValue()
    {
        Value = initialValue;
    }
}
