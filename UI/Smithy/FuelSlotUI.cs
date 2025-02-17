using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FuelSlotUI : MonoBehaviour
{
    [SerializeField] FuelSlot Slot;
    [SerializeField] Image Icon;
    [SerializeField] Slider Bar;
    [SerializeField] TextMeshProUGUI QuantityUI;
    public void AddSlot(FuelSlot Date)
    {
        this.Slot = Date;
        Icon.sprite = Slot.Fuel.Item.Icon;
    }
    public void FixedUpdate()
    {
        if(Slot.Fuel)
        {
            Slot.UpadateUI(Bar, QuantityUI);
            Icon.enabled = true;
            QuantityUI.enabled = true;
        }
        else
        {
            Icon.enabled = false;
            QuantityUI.enabled = false;
        }
    }
}
