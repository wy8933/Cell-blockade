using System.Collections.Generic;
using UnityEngine;

public class BuffHandler : MonoBehaviour
{
    [SerializeField] public List<BuffInfo> buffListInspector = new List<BuffInfo>();

    // Linked List has more efficient insertions and deletions at runtime
    public LinkedList<BuffInfo> buffList = new LinkedList<BuffInfo>();

    private void Start()
    {
        // Load data into LinkedList
        foreach (var buff in buffListInspector)
        {
            AddBuff(buff);
        }
    }

    private void Update()
    {
        // Buff tick
        BuffUpdate();
    }


    public void AddBuff(BuffInfo buffInfo)
    {
        // Check if player alreay have the buff
        BuffInfo findBuffInfo = FindBuff(buffInfo.buffData.id);
        if (findBuffInfo != null)
        {
            // If buff exist
            if (findBuffInfo.currentStack < buffInfo.buffData.maxStack)
            {
                findBuffInfo.currentStack += 1;

                // Determine how the buff duration will be calculated
                switch (findBuffInfo.buffData.buffUpdateType)
                {
                    case BuffUpdateTimeType.Add:
                        findBuffInfo.durationTimer += findBuffInfo.buffData.duration;
                        break;
                    case BuffUpdateTimeType.Replace:
                        findBuffInfo.durationTimer = findBuffInfo.buffData.duration;
                        break;
                    case BuffUpdateTimeType.Keep:
                        // Do nothing with duration
                        break;
                    default:
                        break;
                }
                if (findBuffInfo.buffData.OnCreate)
                    findBuffInfo.buffData.OnCreate.Apply(findBuffInfo);
            }
        }
        else {
            buffInfo.durationTimer = buffInfo.buffData.duration;
            if(buffInfo.buffData.OnCreate)
                buffInfo.buffData.OnCreate.Apply(buffInfo);
            buffList.AddLast(buffInfo);

            // Sort the list
            InsertionSort(buffList);
        }
    
    }

    public void RemoveBuff(BuffInfo buffInfo)
    {
        switch (buffInfo.buffData.buffRemoveStackType) {
            case BuffRemoveStackUpdateType.Clear:
                if (buffInfo.buffData.OnRemove) 
                { 
                    buffInfo.buffData.OnRemove.Apply(buffInfo);
                }

                buffList.Remove(buffInfo);
                break;
            case BuffRemoveStackUpdateType.Reduce:
                buffInfo.currentStack -= 1; 
                
                if (buffInfo.buffData.OnRemove)
                {
                    buffInfo.buffData.OnRemove.Apply(buffInfo);
                }
                
                if (buffInfo.currentStack <= 0)
                {
                    buffList.Remove(buffInfo);
                }
                else { 
                    buffInfo.durationTimer = buffInfo.buffData.duration;
                }
                break;
            default:
                break;
        }
    }

    private BuffInfo FindBuff(int buffDataID)
    {
        foreach (BuffInfo buffInfo in buffList) {
            if (buffInfo.buffData.id == buffDataID) { 
                return buffInfo;
            }
        }

        return default;
    }

    /// <summary>
    /// 
    /// </summary>
    private void BuffUpdate() {

        LinkedListNode<BuffInfo> node = buffList.First;

        while (node != null)
        {
            LinkedListNode<BuffInfo> nextNode = node.Next;

            BuffInfo buffInfo = node.Value;

            // Update buff's tick first so the effect will still apply before the buff has been removed
            if (buffInfo.buffData.OnTick != null)
            {
                if (buffInfo.tickTimer < 0)
                {
                    buffInfo.buffData.OnTick.Apply(buffInfo);
                    buffInfo.tickTimer = buffInfo.buffData.tickTime;
                }
                else
                {
                    buffInfo.tickTimer -= Time.deltaTime;
                }
            }

            // Update buff's duration and remove if expired
            if (!buffInfo.buffData.isForever) {
                if (buffInfo.durationTimer <= 0)
                {
                    buffList.Remove(node);
                    RemoveBuff(buffInfo);
                }
                else
                {
                    buffInfo.durationTimer -= Time.deltaTime;
                }
            }

            node = nextNode;
        }

    }

    void InsertionSort(LinkedList<BuffInfo> list)
    {
        if (list == null || list.First == null)
        {
            return; 
        }

        LinkedListNode<BuffInfo> current = list.First.Next;

        while (current != null)
        {
            LinkedListNode<BuffInfo> next = current.Next;
            LinkedListNode<BuffInfo> prev = current.Previous;

            while (prev != null && prev.Value.buffData.priority > current.Value.buffData.priority)
            {
                prev = prev.Previous;
            }

            if (prev == null)
            {
                list.Remove(current);
                list.AddFirst(current);
            }
            else
            {
                list.Remove(current);
                list.AddAfter(prev, current);
            }

            current = next;
        }
    }

}
