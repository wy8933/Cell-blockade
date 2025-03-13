using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum RarityLevel
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

namespace RewardTables
{

    [Serializable]
    public class RewardTable<T>
    {
        public List<RewardTableEntry<T>> entries = new List<RewardTableEntry<T>>();
    }
}
