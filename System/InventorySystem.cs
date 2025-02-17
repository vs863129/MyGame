
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] GameManger GM;
    public int Money;//持有金錢
    [Header("生成欄位")]
    [SerializeField] private GameObject P_Slot;
    [SerializeField] private ItemTextShow ItemText;
    [SerializeField] private Transform List;
    [SerializeField] public List<Slot> SlotsUI = new List<Slot>();
    [SerializeField] ToggleGroup ToggleGroup;
    [Header("裝備")]
    public EquipmentSystem Equipment;
    [Header("其他系統")]
    public StatsUI StatsUI;
    public InventoryClip _Date;
    private void Start()
    {
        Equipment.Start();
    }
    public void IncreaseMaxWeight(int Point)
    {
        _Date.IncreaseMaxCapacity(Point);
    }
    public void AddSlot()
    {
        Equipment.UpdateWeight();
        int Value = 30;//生成量 (一次性)
        _Date.SetMaxCapacity(100);
        for (int i = 0; i < Value; i++)
        {
            GameObject NewSlot = Instantiate(P_Slot, List);
            Slot Script = NewSlot.GetComponent<Slot>();
            Script.UpdateID(i);
            Script.ItemText = ItemText;
            Script.Select.onValueChanged.AddListener((value) => Slot_UseItem(Script));
            Script.Select.group = ToggleGroup;
            _Date.GenerateSlot(Script);
            SlotsUI.Add(Script);
        }
    }
    public void Slot_UseItem(Slot Slot)
    {
        if (Slot.Item.Clip != null)
        {
            switch (Slot.Item.Clip.type)
            {
                case ItemType.Equipment:
                    Equipment.Equip(Slot);
                    break;
                case ItemType.Tool:
                    Equipment.Equip(Slot);
                    break;
                case ItemType.Resource:
                    StoreRsource(Slot);
                    break;
                case ItemType.Sundries:
                    break;
            }
        }
    }
    void StoreRsource(Slot Slot)
    {
        if (Slot.DoublePress())
        {
            Slot.Select.isOn = false;
            Item item = new Item()
            {
                Clip = Slot.Item.Clip
            }
            ;
            item.AddValue(Slot.Item.Value);
            GM.ResourceSystem.Add(item.Clip, item.Value);
            RemoveItem(Slot, Slot.Item.Value);
        }
    }
    public void Unequipped(Slot EquiptSlot)
    {
        Equipment.UnEquip(EquiptSlot, this);
    }
    public void IgnoreWeight_GotItme(ItemClip Clip) //GM用
    {
        Item itme = new Item();
        itme.Clip = Clip;
        itme.AddValue(1);
        GotItem(itme);
        Equipment.UpdateWeight(); ;
    }
    public bool GotItem(Item item)
    {
        int SlotID=0;
        bool GOT=_Date.GotItem(item,out SlotID);
        if(SlotID>=0)
        {
            SlotsUI[SlotID].UpdateUI();
            Equipment.UpdateWeight();
        }
        return GOT;
    }

    public Slot GetSelectSlot
    {
        get
        {
            foreach(Toggle toggle in ToggleGroup.ActiveToggles())
            {
                if(toggle.isOn)
                {
                    return toggle.GetComponent<Slot>();
                }
            }
            return null;
        }
    }
    public void RemoveItem(Slot slot, int Amount)
    {
        _Date.RemoveItem(slot, Amount);
        Equipment.UpdateWeight();
    }
    #region 輸入數量
    public void Rmove(TMP_InputField inputField)
    {
        if (GetSelectSlot && inputField.text != "")
        {
            int Amount = int.Parse(inputField.text);
            Slot slot = GetSelectSlot;
            RemoveItem(slot,Amount);
            Check(slot, inputField);
        }

    }
    public void Drop(TMP_InputField inputField)
    {
        if (GetSelectSlot&& inputField.text!="")
        {
            int Amount = int.Parse(inputField.text);
            Slot slot = GetSelectSlot;
            GM.DropItem(slot.Item.Clip, GM.player.GetComponent<Player>().DropPos.position, Amount);
            RemoveItem(slot, Amount);
            Check(slot,inputField);
        }
    }
    void Check(Slot slot,TMP_InputField inputField)
    {
        if(slot)
        {
            int Amount = int.Parse(inputField.text);
            if(Amount>slot.Item.Value)
            {
                inputField.text = "" +slot.Item.Value;
            }
        }
        else
        {
            inputField.text = "";
        }
    }
    public void InputEnd(TMP_InputField Text)
    {
        if (GetSelectSlot)
        {
            if (int.Parse(Text.text) > GetSelectSlot.Item.Value)
            {
                Text.text = "" + GetSelectSlot.Item.Value;
            }
            else if (int.Parse(Text.text) < 0)
            {
                Text.text = "" + 0;
            }
        }
        return;
    }
    #endregion

}

[System.Serializable]
public class EquipmentSystem
{
    [SerializeField] private Player Player;
    [SerializeField] private StatsUI UI;
    [Header("武器")]
    public Slot LeftWeapon;
    public Slot RightWeapon;
    [Header("護甲")]
    [SerializeField] private Slot Head;
    [SerializeField] private Slot Chest;
    [SerializeField] private Slot Foots;
    [SerializeField] private Slot[] Accessories;
    [SerializeField] private List<AttributeBuff> BuffList = new List<AttributeBuff>();
    [Header("工具")]
    [Tooltip("0 斧頭\n1 鎬子\n2 槌子\n3 手工")]
    [SerializeField] Slot[] Tools_Slot;
    [Header("顯示")]
    [SerializeField] public GameObject Obj_LeftWeapon;
    [SerializeField] public GameObject Obj_RightWeapon;
    public void Start()
    {
        UpdateStats();
        Head.Date = new SlotDate() {Item=new Item() };
        Chest.Date = new SlotDate() { Item = new Item() };
        Foots.Date = new SlotDate() { Item = new Item() };
        int _Accessories=0;
        int _Tools=0;
        while(_Accessories< Accessories.Length)
        {
            Accessories[_Accessories].Date = new SlotDate() { Item = new Item() };
            _Accessories++;
        }
        while (_Tools < Accessories.Length)
        {
            Tools_Slot[_Tools].Date = new SlotDate() {Item=new Item() };
            _Tools++;
        }
    }
    public void Equip(Slot InvSlot)
    {
        switch (InvSlot.Item.Clip.type)
        {
            case ItemType.Equipment:
                EquipEquipment(InvSlot);
                break;
            case ItemType.Tool:
                EquipTool(InvSlot);
                break;
        }
    }
    public bool CheckHaveTool(int ToolType)
    {
        if (Tools_Slot[ToolType].Item.Clip)
        {
            return true;
        }
        return false;
    }
    public Sprite GotToolSprite(int ToolType)
    {
        ToolClip clip = (ToolClip)Tools_Slot[ToolType].Item.Clip;
        return clip.ToolImage;
    }
    void EquipTool(Slot InvSlot)
    {
        ToolClip clip = (ToolClip)InvSlot.Item.Clip;
        int ToolType = (int)clip.Type;
        if (!Tools_Slot[ToolType].Item.Clip)
        {
            if (InvSlot.DoublePress())
            {
                Tools_Slot[ToolType].AddItem(InvSlot.Item.Clip, 1);
                InvSlot.RemoveItem();
            }
        }
    }
    void EquipEquipment(Slot LastSlot)
    {
        if (LastSlot.DoublePress())
        {
            #region 裝上

            switch (LastSlot.Item.Clip.EquipmentType)
            {
                case Equipment_Type.Weapon:
                    if (Check(LeftWeapon))
                    {
                        LeftWeapon.AddItem(LastSlot.Item.Clip, 1);
                        CheckBuffs(LastSlot.Item.Clip.Buffs, true);
                        LastSlot.RemoveItem();
                        break;
                    }
                    else if (Check(RightWeapon))
                    {
                        RightWeapon.AddItem(LastSlot.Item.Clip, 1);
                        CheckBuffs(LastSlot.Item.Clip.Buffs, true);
                        LastSlot.RemoveItem();
                        break;
                    }
                    break;
                case Equipment_Type.Head:
                    if (Check(Head))
                    {
                        Head.AddItem(LastSlot.Item.Clip, 1);
                        CheckBuffs(LastSlot.Item.Clip.Buffs, true);
                        LastSlot.RemoveItem();
                        break;
                    }
                    break;
                case Equipment_Type.Chest:
                    if (Check(Chest))
                    {
                        Chest.AddItem(LastSlot.Item.Clip, 1);
                        CheckBuffs(LastSlot.Item.Clip.Buffs, true);
                        LastSlot.RemoveItem();
                        break;
                    }
                    break;
                case Equipment_Type.Foots:
                    if (Check(Foots))
                    {
                        Foots.AddItem(LastSlot.Item.Clip, 1);
                        CheckBuffs(LastSlot.Item.Clip.Buffs, true);
                        LastSlot.RemoveItem();
                        break;
                    }
                    break;
                case Equipment_Type.Accessories:
                    for (int i = 0; i < Accessories.Length; i++)
                    {
                        if (Check(Accessories[i]))
                        {
                            Accessories[i].AddItem(LastSlot.Item.Clip, 1);
                            CheckBuffs(LastSlot.Item.Clip.Buffs, true);
                            LastSlot.RemoveItem();
                            break;
                        }
                    }
                    break;
            }

            #endregion
        }
    }
    public void UnEquip(Slot EquipSlot, InventorySystem inventorySlot)
    {
        if (EquipSlot.DoublePress())
        {
            #region 脫下
            if (EquipSlot.Item.Clip != null)
            {
                for (int i = 0; i < inventorySlot.SlotsUI.Count; i++)
                {
                    if (Check(inventorySlot.SlotsUI[i]))
                    {
                        CheckBuffs(EquipSlot.Item.Clip.Buffs, false);
                        inventorySlot.SlotsUI[i].AddItem(EquipSlot.Item.Clip, 1);
                        EquipSlot.RemoveItem();
                        break;
                    }
                }
            }
            #endregion
        }
        else
        {
            EquipSlot.Press();
        }
    }
    private void CheckBuffs(List<AttributeBuff> buffs, bool Equip)
    {
        if (buffs.Count != 0)
        {
            if (Equip)
            {
                for (int i = 0; i < buffs.Count; i++)
                {
                    AddModfile(buffs[i].Type, buffs[i].Value);
                }
            }
            else
            {
                for (int i = 0; i < buffs.Count; i++)
                {
                    AddModfile(buffs[i].Type, -buffs[i].Value);
                }
            }
        }
    }
    private void AddModfile(Attribute Type, int Value)
    {
        if (BuffList.Count == 0)
        {
            AttributeBuff Buff = new AttributeBuff
            {
                Type = Type,
                Value = Value
            };
            BuffList.Add(Buff);
            Player.AddModflie(Buff.Type, Value, true);
            UpdateStats();
        }
        else
        {
            for (int i = 0; i < BuffList.Count; i++)
            {
                if (BuffList[i].Type == Type)
                {
                    BuffList[i].Value += Value;
                    Player.AddModflie(BuffList[i].Type, Value, true);
                    UpdateStats();
                    break;
                }
                else if (BuffList[i].Type != Type && i == BuffList.Count - 1)
                {
                    AttributeBuff Buff = new AttributeBuff
                    {
                        Type = Type,
                        Value = Value
                    };
                    BuffList.Add(Buff);
                    Player.AddModflie(Buff.Type, Value, true);
                    UpdateStats();
                    break;
                }
            }
        }
    }
    public void UpdateStats()
    {
        UI.UpdateStats();
    }
    public void UpdateWeight()
    {
        UI.UpdateWeight();
    }
    public bool Check(Slot slot)//判斷是否空
    {
        if (slot.Item.Clip == null)
        {
            return true;
        }
        return false;
    }
    public void AllClear()
    {
        LeftWeapon.Clear();
        RightWeapon.Clear();
        Head.Clear();
        Chest.Clear();
        Foots.Clear();
        for (int i = 0; i < Accessories.Length; i++)
        {
            Accessories[i].Clear();
        }
    }
    #region 存取
    public void Save(out string Head, out string Chest, out string Foots, out string LeftWeapon, out string RightWeapon, out string[] Accessories)
    {
        Head = this.Head.ClipName;
        Chest = this.Chest.ClipName;
        Foots = this.Foots.ClipName;
        LeftWeapon = this.LeftWeapon.ClipName;
        RightWeapon = this.RightWeapon.ClipName;
        Accessories = new string[this.Accessories.Length];
        for (int i = 0; i < this.Accessories.Length; i++)
        {
            Accessories[i] = this.Accessories[i].ClipName;
        }
    }
    public void Load(ref ItemClip Head, ref ItemClip Chest, ref ItemClip Foots, ref ItemClip LeftWeapon, ref ItemClip RightWeapon, ref ItemClip[] Accessories)
    {
        this.Head.AddItem(Head, 1);
        this.Chest.AddItem(Chest, 1);
        this.Foots.AddItem(Foots, 1);
        this.LeftWeapon.AddItem(LeftWeapon, 1);
        this.RightWeapon.AddItem(RightWeapon, 1);
        for (int i = 0; i < this.Accessories.Length; i++)
        {
            this.Accessories[i].AddItem(Accessories[i], 1);
        }
        UpdateStats();
    }
    #endregion
}
