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
        public float chance;

        public int minCount = 1;
        public int maxCount = 1;


        public bool Roll() => Random.value <= chance;
        public int DropCount() => Random.Range(minCount, maxCount + 1);

        public IEnumerable<T> Drop()
        {
            var count = DropCount();
            for (var i = 0; i < count; i++)
                yield return value;
        }
    }
}