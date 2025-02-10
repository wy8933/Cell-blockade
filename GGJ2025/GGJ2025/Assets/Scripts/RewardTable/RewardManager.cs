using System.Linq;
using UnityEngine;
using RewardTables;

public class RewardManager : MonoBehaviour
{
    public static RewardManager Instance { get; private set; }

    [Header("Reward Table")]
    [SerializeField] private RewardTable<BuffData> buffTable;

    [Header("Overall Rarity Chances")]
    [Tooltip("Overall chance for a Common reward")]
    public float commonChance = 0.6f;
    [Tooltip("Overall chance for an Uncommon reward")]
    public float uncommonChance = 0.2f;
    [Tooltip("Overall chance for a Rare reward")]
    public float rareChance = 0.1f;
    [Tooltip("Overall chance for an Epic reward")]
    public float epicChance = 0.07f;
    [Tooltip("Overall chance for a Legendary reward")]
    public float legendaryChance = 0.03f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public BuffData GetSingleBuff()
    {
        float roll = Random.value;
        float cumulative = commonChance;
        RarityLevel selectedRarity = RarityLevel.Common;
        if (roll <= cumulative)
        {
            selectedRarity = RarityLevel.Common;
        }
        else
        {
            cumulative += uncommonChance;
            if (roll <= cumulative)
                selectedRarity = RarityLevel.Uncommon;
            else
            {
                cumulative += rareChance;
                if (roll <= cumulative)
                    selectedRarity = RarityLevel.Rare;
                else
                {
                    cumulative += epicChance;
                    if (roll <= cumulative)
                        selectedRarity = RarityLevel.Epic;
                    else
                        selectedRarity = RarityLevel.Legendary;
                }
            }
        }

        var possibleEntries = buffTable.entries
            .Where(e => e.rarity == selectedRarity && e.Roll())
            .ToList();

        if (possibleEntries.Count == 0)
        {
            possibleEntries = buffTable.entries
                .Where(e => e.Roll())
                .ToList();
        }

        if (possibleEntries.Count > 0)
        {
            int index = Random.Range(0, possibleEntries.Count);
            var selectedEntry = possibleEntries[index];
            var reward = selectedEntry.Drop().FirstOrDefault();
            return reward as BuffData;
        }

        return null;
    }
}
