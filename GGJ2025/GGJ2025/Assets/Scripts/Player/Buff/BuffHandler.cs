using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffHandler : MonoBehaviour
{
    [SerializeField] private List<BuffInfo> _initialBuffs = new List<BuffInfo>();
    [SerializeField] private List<BuffInfo> _activeBuffs = new List<BuffInfo>();

    private BuffComparer _buffComparer = new BuffComparer();

    public List<BuffInfo> ActiveBuffs
    {
        get => _activeBuffs;
        private set => _activeBuffs = value;
    }

    #region Core Buff Management
    private void Start()
    {
        foreach (var buff in _initialBuffs)
        {
            AddBuff(buff);
        }
    }

    private void Update()
    {
        ProcessBuffTicks();
        UpdateBuffDurations();
    }

    public void AddBuff(BuffInfo newBuff)
    {
        BuffInfo clonedBuff = CloneBuff(newBuff);
        clonedBuff.Initialize();

        BuffInfo existing = FindBuff(clonedBuff.buffData.id);

        if (existing != null)
        {
            HandleStacking(existing, clonedBuff);
            return;
        }
        else
        {
            existing = clonedBuff;
        }

        InsertSorted(clonedBuff);
        ExecuteModule(existing.buffData.OnCreate, clonedBuff);
    }

    private BuffInfo CloneBuff(BuffInfo source)
    {
        return new BuffInfo
        {
            buffData = source.buffData,
            creator = source.creator,
            target = source.target,
            currentStack = source.currentStack,
            durationTimer = source.durationTimer,
            tickTimer = source.tickTimer
        };
    }
    #endregion

    #region Stacking & Sorting
    private void HandleStacking(BuffInfo existing, BuffInfo newBuff)
    {
        if (existing.currentStack >= existing.buffData.maxStack) return;

        existing.currentStack++;
        ExecuteModule(existing.buffData.OnCreate, existing);

        switch (existing.buffData.buffUpdateType)
        {
            case BuffUpdateTimeType.Add:
                existing.durationTimer += newBuff.buffData.duration;
                break;
            case BuffUpdateTimeType.Replace:
                existing.durationTimer = newBuff.buffData.duration;
                break;
        }
    }

    private void InsertSorted(BuffInfo buff)
    {
        int index = _activeBuffs.BinarySearch(buff, _buffComparer);
        _activeBuffs.Insert(index < 0 ? ~index : index, buff);
    }
    #endregion

    #region Duration & Tick Management
    private void UpdateBuffDurations()
    {
        List<BuffInfo> toRemove = new List<BuffInfo>();

        foreach (var buff in _activeBuffs)
        {
            if (!buff.buffData.isForever)
            {
                buff.durationTimer -= Time.deltaTime;
                if (buff.durationTimer <= 0) toRemove.Add(buff);
            }
        }

        RemoveBuffs(toRemove);
    }
    private void ProcessBuffTicks()
    {
        foreach (var buff in _activeBuffs)
        {
            if (buff.buffData.OnTick != null)
            {
                buff.tickTimer -= Time.deltaTime;
                if (buff.tickTimer <= 0)
                {
                    ExecuteModule(buff.buffData.OnTick, buff);
                    buff.tickTimer = buff.buffData.tickTime;
                }
            }
        }
    }
    #endregion

    #region Module Execution
    private void ExecuteModule(BaseBuffModule module, BuffInfo buff, DamageInfo damageInfo = null)
    {
        if (module != null)
        {
            module.Apply(buff, damageInfo);
        }
    }

    #endregion

    #region Public Interface
    public void RemoveBuff(BuffInfo buff)
    {
        switch (buff.buffData.buffRemoveStackType)
        {
            case BuffRemoveStackUpdateType.Clear:
                _activeBuffs.Remove(buff);
                ExecuteModule(buff.buffData.OnRemove, buff);
                break;

            case BuffRemoveStackUpdateType.Reduce:
                buff.currentStack--;
                ExecuteModule(buff.buffData.OnRemove, buff);

                if (buff.currentStack <= 0)
                    _activeBuffs.Remove(buff);
                else
                    buff.durationTimer = buff.buffData.duration;
                break;
        }
    }

    public void TriggerOnHit(BuffInfo buff, DamageInfo damageInfo)
    {
        ExecuteModule(buff.buffData.OnHit, buff, damageInfo);
    }

    public void TriggerOnHurt(BuffInfo buff, DamageInfo damageInfo)
    {
        ExecuteModule(buff.buffData.OnHurt, buff, damageInfo);
    }

    public void TriggerOnKill(BuffInfo buff)
    {
        ExecuteModule(buff.buffData.OnKill, buff);
    }

    public void TriggerOnDeath(BuffInfo buff)
    {
        ExecuteModule(buff.buffData.OnDeath, buff);
    }

    public void TriggerAllOnDeath() {
        foreach (var buff in _activeBuffs) 
        {
            ExecuteModule(buff.buffData.OnDeath, buff);
        }
    }
    #endregion

    #region Helper Methods
    public BuffInfo FindBuff(int buffId)
    {
        return _activeBuffs.Find(b => b.buffData.id == buffId);
    }

    private void RemoveBuffs(List<BuffInfo> toRemove)
    {
        foreach (var buff in toRemove)
        {
            RemoveBuff(buff);
        }
    }

    private class BuffComparer : IComparer<BuffInfo>
    {
        public int Compare(BuffInfo a, BuffInfo b)
        {
            return b.buffData.priority.CompareTo(a.buffData.priority);
        }
    }
    #endregion
}

public static class BuffInfoExtensions
{
    public static void Initialize(this BuffInfo buff)
    {
        buff.durationTimer = buff.buffData.duration;
        buff.tickTimer = buff.buffData.tickTime;
        buff.currentStack = 1;
    }
}