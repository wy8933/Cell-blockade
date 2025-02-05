using System.Collections.Generic;
using UnityEngine;

namespace RewardTables
{
    public class RewardManager : MonoBehaviour
    {
        public static RewardManager Instance { get; private set; }

        [Header("Buff Table")]
        [SerializeField] private RewardTable<BuffData> buffTable;

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
            List<BuffData> buffs = buffTable.Roll();
            return buffs.Count > 0 ? buffs[Random.Range(0, buffs.Count)] : null;
        }

        public BuffInfo GetSingleBuffInfo(GameObject creator, GameObject target)
        {
            BuffData buffData = GetSingleBuff();
            if (buffData == null)
                return null;

            return new BuffInfo
            {
                buffData = buffData,
                creator = creator,
                target = target,
                currentStack = 1,
                durationTimer = buffData.isForever ? float.MaxValue : buffData.duration,
                tickTimer = buffData.tickTime
            };
        }
    }
}
