using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RewardTables
{
    [Serializable]
    public class RewardTableEntry<T>
    {
        public T value;

        [Range(0, 1f)]
        public float chance = 1f;

        public RarityLevel rarity = RarityLevel.Common;

        public bool Roll() => Random.value <= chance;

        public IEnumerable<T> Drop()
        {
            yield return value;
        }
    }
}
