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
    [SerializeField] private List<BuffData> availableBuffs;
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
        List<BuffData> buffPool = new List<BuffData>(availableBuffs);
        int count = Mathf.Min(4, buffPool.Count);

        // Generate 4 unique buffs.
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, buffPool.Count);
            generatedBuffs.Add(buffPool[randomIndex]);
            buffPool.RemoveAt(randomIndex);
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
                buffNames[i].text = buff.buffName;
                buffDescriptions[i].text = buff.description;
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
        generatedBuffs.Clear();
        List<BuffData> buffPool = new List<BuffData>(availableBuffs);
        int count = Mathf.Min(4, buffPool.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, buffPool.Count);
            generatedBuffs.Add(buffPool[randomIndex]);
            buffPool.RemoveAt(randomIndex);
        }

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
