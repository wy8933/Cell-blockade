using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [Header("UI References")]
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMP_Text toolTipTitle;
    [SerializeField] private TMP_Text tooltipText;
    [SerializeField] private RectTransform backgroundRectTransform;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HideTooltip();
    }

    public void ShowTooltip(string title, string content)
    {
        toolTipTitle.text = title;
        tooltipText.text = content;
        tooltipPanel.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void ShowTooltip(Vector3 position, string title, string content)
    {
        tooltipPanel.transform.position = position + Vector3.up * 30f;
        ShowTooltip(title, content);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
