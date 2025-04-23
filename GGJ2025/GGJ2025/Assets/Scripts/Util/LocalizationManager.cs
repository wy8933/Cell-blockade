using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public string GetLocalizedString(string tableName, string entryKey)
    {
        var table = LocalizationSettings.StringDatabase.GetTable(tableName);
        if (table == null)
        {
            Debug.LogError($"Localization Table '{tableName}' not found!");
            return $"[{entryKey}]";
        }

        var entry = table.GetEntry(entryKey);
        if (entry == null)
        {
            Debug.LogError($"Entry '{entryKey}' not found in Table '{tableName}'!");
            return $"[{entryKey}]";
        }

        return entry.GetLocalizedString();
    }
}
