using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Inventory",menuName ="System/Inventory")]
public class InventoryClip : ScriptableObject
{
    public List<SlotDate> Slots = new List<SlotDate>();
    [SerializeField] float _CAPACITY; public float Capacity { get {return _CAPACITY; } set { System.Math.Round(_CAPACITY = value, 2); if (_CAPACITY < 0) { _CAPACITY = 0; } } }
    [SerializeField] float _MAX_CAPACITY; public float MaxCapacity { get { return _MAX_CAPACITY; } }
    #region 容量設置
    public void GotCapacityDate(out float Capacity,out float MaxCapacity)
    {
        Capacity = _CAPACITY;
        MaxCapacity = _MAX_CAPACITY;
    }
    public void IncreaseMaxCapacity(int Point)
    {
        _MAX_CAPACITY += Point;
    }
    public void SetMaxCapacity(int Point)
    {
        _MAX_CAPACITY = Point;
    }
#endregion
#region 生成背包欄位
    public void GenerateSlot(int MaxValue)
    {
        while(Slots.Count < MaxValue)
        {
            SlotDate date = new SlotDate();
            date.Item = new Item();
            Slots.Add(date);
        }
    }
    public void GenerateSlot(Slot UI)
    {
        SlotDate date = new SlotDate();
        date.Item = new Item();
        UI.Date = date;
        Slots.Add(date);
    }
#endregion
#region 物品流動
    public bool GotItem(Item item,out int SlotID)
    {
        if (item.Clip.W + Capacity <= MaxCapacity && item.Value != 0)//未撿取完成
        {
            SlotID=AddItem(item);
            item.CostValue(1);
            return false;
        }
        else if (item.Value == 0)//撿取完成
        {
            SlotID = -1;
            return true;
        }
        SlotID = -1;
        Debug.Log("負重過重");
        return false;
    }
    int AddItem(Item item)
    {
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].Item.Clip == null)
            {
                Slots[i].Item.AddValue(item.Clip, 1);
                Capacity += item.Clip.W;
                return i;
            }
            else if (Slots[i].Item.Clip == item.Clip && item.Clip.IsStaceed)
            {
                Slots[i].Item.AddValue(1);
                Capacity += item.Clip.W;
                return i;
            }
        }
        return -1;
    }
    public void RemoveItem(SlotDate slotDate,int Amount)
    {
        Capacity -= slotDate.Item.Clip.W * Amount;
        slotDate.Item.CostValue(Amount);
    }
    public void RemoveItem(Slot slot,int Amount)
    {
        Capacity -= slot.Item.Clip.W * Amount;
        slot.RemoveItem(Amount);
    }
#endregion
}
[System.Serializable]
public class SlotDate
{
    public Item Item;
}