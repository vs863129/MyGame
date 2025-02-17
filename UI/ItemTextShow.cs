using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
public class ItemTextShow : MonoBehaviour
{
    [Header("標題替換")]
    public ItemClip item;
    [SerializeField] int Value;
    [SerializeField] Image TypeIcon;
    [SerializeField] private Image ItemImage;
    [SerializeField] LocalizeStringEvent Type;
    [SerializeField] LocalizeStringEvent ItemName;
    [SerializeField] LocalizeStringEvent Description;
    List<GameObject> Single = new List<GameObject>();
    List<GameObject> Double = new List<GameObject>();
    [Header("預置物")]
    public GameObject P_BuffTextUI;
    [Header("顯示用")]
    [SerializeField] private GameObject Top;
    [SerializeField] private GameObject AttributesList;
    [SerializeField] TextMeshProUGUI WeightText;
    [Tooltip("根據EquipmentType顯示\n0 武器\n1 頭盔\n2 盔甲\n3 鞋子\n4 手套\n5 飾品 ")]
    [SerializeField] Sprite[] AllEquipmentTypeIcon;
    [SerializeField] Sprite[] AllItemTypeIcon;
    [SerializeField] Sprite[] AllItemBuffIcon;
    [Header("欄位")]
    [SerializeField] Transform AloneUI;
    [SerializeField] Transform DoubleUI;
    public void Switch(ItemClip clip,int value)
    {
        switch (clip.type)
        {
            case ItemType.Tool:
                TypeIcon.sprite = AllItemTypeIcon[2];
                break;
            case ItemType.Equipment:
                TypeIcon.sprite = AllEquipmentTypeIcon[(int)clip.EquipmentType];
                break;
            default:
                TypeIcon.sprite = AllItemTypeIcon[(int)clip.type];
                break;
        }
        LanguageSystem.SetText(Type, ITEN_TYPE(clip));
        ItemImage.sprite = clip.Icon;
        LanguageSystem.SetText(ItemName, "Items,"+ clip.Name);
        Top.SetActive(true);
        AttributesList.SetActive(true);
        Single = ShowBuff(Single, clip);
        LanguageSystem.SetText(Description, "Items_Tootip," + clip.Description);
        WeightText.text = System.Math.Round(value * clip.W,2) + "(" + clip.W + ")";
    }
    string ITEN_TYPE(ItemClip clip)
    {
        switch (clip.type)
        {
            case ItemType.Equipment:
                return "ITEM_TYPE," + clip.EquipmentType;
            case ItemType.Tool:
                ToolClip toolClip = (ToolClip)clip;
                return "ITEM_TYPE," + toolClip.Type;
            default:
                return "ITEM_TYPE,"+ clip.type;
        }
    }
    public void Press(ItemClip clip,int value)
    {
        item = clip;
        Value = value;
    }
    public void Check()
    {
        if(item)
        {
            Switch(item,Value);
        }
        else
        {
            Recovery();
        }
    }
    private void Recovery()
    {
        Top.SetActive(false);
        AttributesList.SetActive(false);
        RemoveList(Single);
        RemoveList(Double);
    }
    List<GameObject> ShowBuff(List<GameObject> list, ItemClip clip)
    {
        RemoveList(list); 
        for(int i=0;i< clip.Buffs.Count;i++)
        {
            GameObject NewUI=  Instantiate(P_BuffTextUI, DoubleUI);
            NewUI.GetComponentInChildren<TextMeshProUGUI>().text = "+" + clip.Buffs[i].Value;
            NewUI.GetComponentInChildren<Image>().sprite = AllItemBuffIcon[(int)clip.Buffs[i].Type];
            list.Add(NewUI);
        }
        return list;
    }
    List<GameObject> RemoveList(List<GameObject> list)
    {
        if (list!=null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Destroy(list[i]);
            }
            list.Clear();
        }
        return list;
    }
}


