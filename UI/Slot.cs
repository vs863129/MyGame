using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Slot : MonoBehaviour
{
    private int ID;
    [SerializeField] private bool IsEquipmentSlot;
    [SerializeField] private Image Icon;
    [SerializeField] TextMeshProUGUI ItemValue;
    public Toggle Select{get { return GetComponent<Toggle>();}}
    [HideInInspector] private Equipment_Type EquipmentType;
    public SlotDate Date;
    public Item Item
    {
        get { 
             if(Date==null)
            {
                Debug.LogError("Date未建立完成");
                return null;
            }
            return Date.Item; }
        set { Date.Item = value; }
    }
    public ItemTextShow ItemText;
    float ShowTime = 0;
    #region 外部獲取
    public float GotAllWeight
    {
        get
        {
            float _AllWeight = 0;
            if(Item.Clip)
            {
                _AllWeight = Item.Clip.W * Item.Value;
            }
            return _AllWeight;
        }
    }
    public Equipment_Type GotEquipmentType
    {
        get { return EquipmentType; }
    }
    public bool GotEquipmentSlot
    {
        get { return IsEquipmentSlot; }
    }
    public int GotID
    {
        get { return ID; }
    }
    public string ClipName
    {
        get
        {
            if(Item.Clip!=null)
            {
                return Item.Clip.name;
            }
            return "";
        }
    }
    public bool DoublePress()
    {
        if(Item.Clip==null)
        {
            Select.isOn = false;
        }
        if (ShowTime >= 0.1f)
        {
            ShowTime = 0;
            return true;
        }
        else
        {
            ShowTime = 0.5f;
            return false;
        }
    }
    #endregion
    private void FixedUpdate()
    {
        ShowItmeText();
    }
    void ShowItmeText()
    {
        if (ShowTime > 0.1)
        {
            ShowTime -= Time.fixedDeltaTime;
        }
    }
    public void AddItem(ItemClip item,int Value)
    {
        if (item != null)
        {
            if(GetComponent<MouseState>())
            {
                GetComponent<MouseState>().SetTipText = item.Tip;
            }
            Item.Clip = item;
            Item.AddValue(Value);
            UpdateUI(item,Value);
        }
    } 
    public void AddItem(int value)
    {
        Item.AddValue(value);
        if(!IsEquipmentSlot)
        {
            ItemValue.text = "" + Item.Value;
        }
    }
    public void RemoveItem()
    {
        Item.CostValue(1);
        Check();
    }
    public void RemoveItem(int value)
    {
        Item.CostValue(value);
        Check();
    }
    void Check()
    {
        if(!IsEquipmentSlot)
        {
            ItemValue.text = "" + Item.Value;
        }
        if (Item.Value <= 0)
        {
            Clear();
        }
    }
    public void UpdateUI()
    {
        Icon.sprite = Item.Clip.Icon;
        Icon.color = Color.white;
        if (!IsEquipmentSlot)
        {
            ItemValue.text = "" + Item.Value;
        }
    }
    void UpdateUI(ItemClip item, int Value)
    {
        Icon.sprite = item.Icon;
        Icon.color = Color.white;
        if (!IsEquipmentSlot)
        {
            ItemValue.text = "" + Value;
        }
    }
    public void Clear()
    {
        if (!IsEquipmentSlot)
        {
            ItemValue.text = "";
        }
        Select.isOn = false;
        ItemText.item = null;
        ItemText.Check();
        Icon.sprite = null;
        Icon.color = new Color(1, 1, 1, 0);
        Item.Clear();
    }
    public void UpdateID(int NewID)
    {
        ID = NewID;
    }
    public void Press()
    {
        if(Item.Clip)
        {
            ItemText.Press(Item.Clip,Item.Value);
        }
        else
        {
            Select.isOn = false; 
        }
        if(!Select.isOn)
        {
            ItemText.item = null;
        }
    }
    public void OnMouseEnter()
    {
        if (Item.Clip != null)
        {
            ItemText.Switch(Item.Clip,Item.Value);
        }
    }
    public void OnMouseExit()
    {
        ItemText.Check();
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(Slot))]
    public class SlotEdit : Editor
    {
        Slot _Slot;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _Slot = (Slot)target;
            if (_Slot.IsEquipmentSlot)
            {
                _Slot.EquipmentType = (Equipment_Type)EditorGUILayout.EnumPopup("SlotType", _Slot.EquipmentType);
            }
            else
            {
                _Slot.ID = EditorGUILayout.IntField("ID", _Slot.ID);
            }
        }
    }
#endif
}
