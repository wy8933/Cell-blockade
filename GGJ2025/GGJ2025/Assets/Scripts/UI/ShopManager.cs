using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private List<Button> buffButtons;
    [SerializeField] private List<TMP_Text> buffNames;
    [SerializeField] private List<TMP_Text> buffDescriptions;
    [SerializeField] private List<Image> buffIcons;
    [SerializeField] private List<TMP_Text> buffCosts;
    [SerializeField] private TMP_Text currencyText;

    [Header("Buff Data")]
    private List<BuffData> generatedBuffs = new List<BuffData>();

    [Header("Buff Grid UI")]
    [SerializeField] private GameObject buffItemPrefab;
    [SerializeField] private Transform buffGridParent;

    [Header("References")]
    [SerializeField] private BuffHandler buffHandler;
    private GameObject player;
    [SerializeField] private GameObject buffList;
    [SerializeField] private GameObject shopList;

    private Dictionary<string, BuffItemUI> purchasedBuffUIs = new Dictionary<string, BuffItemUI>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        shopPanel.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
        buffHandler = player.GetComponent<BuffHandler>();
    }

    private void UpdateCurrencyText()
    {
        currencyText.text = "Currency: " + GameManager.Instance.Currency.Value.ToString("F0");
    }

    public void ShowShop()
    {
        generatedBuffs.Clear();
        // Generate 4 unique buffs.
        for (int i = 0; i < 4; i++)
        {
            generatedBuffs.Add(RewardManager.Instance.GetSingleBuff());
        }

        UpdateShopUI();
        shopPanel.SetActive(true);
    }

    private void UpdateShopUI()
    {
        UpdateCurrencyText();
        for (int i = 0; i < buffButtons.Count; i++)
        {
            if (i < generatedBuffs.Count)
            {
                BuffData buff = generatedBuffs[i];

                // Use buffName to build localization keys
                string nameKey = buff.buffName + " - Name";
                string descriptionKey = buff.buffName + " - Description";

                // Fetch localized strings
                string localizedName = LocalizationManager.Instance.GetLocalizedString("Buffs", nameKey);
                string localizedDescription = LocalizationManager.Instance.GetLocalizedString("Buffs", descriptionKey);

                // Set UI text
                buffNames[i].text = localizedName;
                buffDescriptions[i].text = localizedDescription;
                buffIcons[i].sprite = buff.icon;
                buffCosts[i].text = buff.price.ToString();

                int index = i;
                buffButtons[i].onClick.RemoveAllListeners();
                buffButtons[i].onClick.AddListener(() => PurchaseBuff(index));
                buffButtons[i].gameObject.SetActive(true);
            }
            else
            {
                buffButtons[i].gameObject.SetActive(false);
            }
        }
        GameManager.Instance.currencyText.text = $"{((int)GameManager.Instance.Currency.Value)}";
    }


    public void PurchaseBuff(int index)
    {
        if (index < 0 || index >= generatedBuffs.Count)
        {
            Debug.LogWarning("Invalid buff selection index");
            return;
        }

        BuffData selectedBuff = generatedBuffs[index];

        if (GameManager.Instance.Currency.Value < selectedBuff.price)
        {
            Debug.Log("Not enough currency to purchase this buff");
            return;
        }

        GameManager.Instance.Currency.Value -= selectedBuff.price;

        BuffInfo buffInfo = new BuffInfo
        {
            buffData = selectedBuff,
            currentStack = 1,
            target = player,
            creator = player
        };

        buffHandler.AddBuff(buffInfo);

        AddOrUpdateBuffInGrid(selectedBuff);

        generatedBuffs.RemoveAt(index);
        UpdateShopUI();
    }
    public void RerollShop()
    {
        if (GameManager.Instance.Currency.Value <= 9) {
            return;
        }

        generatedBuffs.Clear();

        for (int i = 0; i < 4; i++)
        {
            generatedBuffs.Add(RewardManager.Instance.GetSingleBuff());
        }
        GameManager.Instance.ModifyCurrency(-10);
        UpdateShopUI();
    }

    private void AddOrUpdateBuffInGrid(BuffData buff)
    {
        string buffKey = buff.buffName;
        if (purchasedBuffUIs.ContainsKey(buffKey))
        {
            purchasedBuffUIs[buffKey].IncrementStack();
        }
        else
        {
            GameObject newBuffItem = Instantiate(buffItemPrefab, buffGridParent);
            BuffItemUI itemUI = newBuffItem.GetComponent<BuffItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(buff);
                purchasedBuffUIs.Add(buffKey, itemUI);
            }
        }
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false);
    }

    public void ShowShopList()
    {
        shopList.SetActive(true);
        buffList.SetActive(false);
    }

    public void ShowBuffList()
    {
        shopList.SetActive(false);
        buffList.SetActive(true);
    }
}
