using NUnit.Framework;
using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "FloatReference", menuName = "Variables/FloatReference")]
public class FloatReference : ScriptableObject
{
    [Header("Values")]
    [Tooltip("Value used at the start of the game or when resetting")]
    public float initialValue;

    [Tooltip("Current value at runtime")]
    [SerializeField]
    private float runtimeValue;

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

    [Tooltip("Event invoked whenever 'runtimeValue' changes")]
    public event Action<float> OnValueChanged;


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
    public float Value
    {
        get => runtimeValue;
        set
        {
            // If there's no actual change, do nothing
            if (Mathf.Approximately(runtimeValue, value)) return;

            runtimeValue = value;
            OnValueChanged?.Invoke(runtimeValue);

            if (debugEnabled)
            {
                Debug.Log($"[FloatVariable] New Value: {runtimeValue}", this);
            }
        }
    }

    /// <summary>
    /// Sets the runtime value directly, triggering the OnValueChanged event
    /// </summary>
    public void SetValue(float newValue)
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
