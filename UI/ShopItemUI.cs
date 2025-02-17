using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Cargo Commodity;
    public Image ItemUI;
    public TextMeshProUGUI Value;//商品數量
    public TextMeshProUGUI Cost;//成本
    public void Buy(GameManger GM)
    {
        //if (GM.InventorySystem.Money >= Cost)
        //{
        //    GM.InventorySystem.Money -= Cost;
        Commodity.item.CostValue(1);
        LoadValue();
        //}
    }
    void LoadValue()
    {
        Value.text = "" + Commodity.item.Value;
    }
    public void UpdateUI()
    {
        ItemUI.sprite = Commodity. item.Clip.Icon;
        Value.text = "" + Commodity. item.Value;
        Cost.text = "" + Commodity. Cost;
    }
}
