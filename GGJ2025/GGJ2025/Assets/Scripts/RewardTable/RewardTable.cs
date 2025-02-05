using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace RewardTables
{
    [Serializable]
    public class RewardTable<T>
    {
        public List<RewardTableEntry<T>> entries = new();

        public List<T> Roll()
        {
            return entries.Where(e => e.Roll()).OrderBy(_ => Random.value).SelectMany(e => e.Drop()).ToList();
        }
    }
}