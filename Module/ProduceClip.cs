using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Produce", menuName = "Type/Produce")]
public class ProduceClip : ScriptableObject
{
    public ItemClip item;//成品
    public ProduceType Type;
    [Tooltip("單次所產生的產量")]
    public int PerProudce = 1;
    public bool IsFinish;
    public float MakeTime;
    public float Progress;
    public int Quantity;//數量
    public int MaxQuantity;//最大製造量
    public List<Cost_Resource> Cost;//成本
    protected int IsFinishQuantity;//已完成物品的數量
    public bool IsFull //確認是否可增加
    {
        get
        {
            if(Quantity== MaxQuantity)
            {
                return true;
            }
            return false;
        }
    }
    public void GotItem()
    {
        if (Quantity < MaxQuantity)
        {
            if(Progress >= MakeTime)
            {
                Progress = 0;
            }
            Quantity++;
            IsFinish = false;
        }
    }
    public int QueueUIGotFinishValue
    {
        get
        {
            return IsFinishQuantity;
        }
    }
    public int GotFinishQuantity
    {
        get
        {
            int ReturnQuantity = IsFinishQuantity* PerProudce;
            IsFinishQuantity = 0;
            if (Quantity == 0)
            {
                Destroy(this);
            }
            return ReturnQuantity;
        }
    }
    public bool Cancel(ResourceSystem RS)
    {
        if(Quantity>0)
        {
            Quantity--;
            foreach (Cost_Resource Resource in Cost)
            {
                RS.Add(Resource.Clip, Resource.Value);
            }
            if(Quantity==0)
            {
                FinishMaking();
            }
        }
        if(Quantity<=0&& IsFinishQuantity<=0)
        {
            Destroy(this);
            return true;
        }
        else
        {
            return false;
        }
    }
    void FinishMaking()
    {
        IsFinish = true;
        Progress = MakeTime;
    }
    public virtual bool Making(float Power)//製作強度(取決村民能力)
    {
        if(!IsFinish)
        {
            Progress += Power;
            if (Progress >= MakeTime)
            {
                Progress = 0;
                IsFinishQuantity++;
                Quantity--;
                if (Quantity==0)
                {
                    FinishMaking();
                    return true;
                }
            }
            return false;
        }
        return true;
    }
}
