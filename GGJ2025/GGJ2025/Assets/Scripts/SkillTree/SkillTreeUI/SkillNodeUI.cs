#if UNITY_EDITOR
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillNodeUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI costText;
    public Button buyButton;

    [Header("Node Name")]
    // The name of the skill node as it appears in SkillTreeManager's allNodes list
    public string skillName;

    private SkillTreeNode node;

    private void Start()
    {
        // Attempt to retrieve the node from the manager using skillName
        if (SkillTreeManager.Instance != null && SkillTreeManager.Instance.allNodes != null)
        {
            node = SkillTreeManager.Instance.allNodes.Find(n => n.nodeName == skillName);
        }

        if (node == null)
        {
            Debug.LogWarning($"{gameObject.name}: No node found with skillName '{skillName}' or manager not initialized" +
                             "UI will be disabled");
            if (buyButton != null)
            {
                buyButton.interactable = false;
            }
            return;
        }

        // Now that we have the node, set up the UI
        UpdateUI();

        // Wire up the Buy button
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }
    }

    private void UpdateUI()
    {
        if (node == null) return;

        // Update text fields
        if (skillNameText != null)
            skillNameText.text = node.nodeName;
        if (costText != null)
            costText.text = "Cost: " + node.cost;

        // If node is already unlocked, disable the button
        if (node.isUnlocked && buyButton != null)
        {
            buyButton.interactable = false;
            var buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = "Unlocked";
        }
    }

    private void OnBuyButtonClicked()
    {
        if (node == null) return;

        bool purchased = SkillTreeManager.Instance.PurchaseSkill(node, null);
        if (purchased)
        {
            buyButton.interactable = false;
            var buttonText = buyButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null) buttonText.text = "Unlocked";
        }
    }
}
#endif