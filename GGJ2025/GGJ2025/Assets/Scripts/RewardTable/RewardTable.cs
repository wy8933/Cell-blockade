using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RewardTables
{
    public enum RarityLevel
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    [Serializable]
    public class RewardTable<T>
    {
        public List<RewardTableEntry<T>> entries = new List<RewardTableEntry<T>>();
    }
}
