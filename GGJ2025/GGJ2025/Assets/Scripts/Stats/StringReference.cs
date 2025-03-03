using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "StringReference", menuName = "Variables/StringReference")]
public class StringReference : ScriptableObject
{
    [Header("Values")]
    [Tooltip("Value used at the start of the game or when resetting")]
    public string initialValue;

    [Tooltip("Current value at runtime")]
    [SerializeField]
    private string runtimeValue;

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
    public UnityEvent<string> OnValueChanged;


    private void OnEnable()
    {
        // Only reset if the game is actually playing and you've enabled reset
        if (Application.isPlaying && resetToInitialValue && saveOn == SaveOn.SceneLoaded)
        {
            ResetValue();
        }
    }


    /// <summary>
    /// Access or modify the current runtime value, invokes OnValueChanged if it changes
    /// </summary>
    public string Value
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
                Debug.Log($"[StringVariable] New Value: {runtimeValue}", this);
            }
        }
    }

    /// <summary>
    /// Sets the runtime value directly, triggering the OnValueChanged event
    /// </summary>
    public void SetValue(string newValue)
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
