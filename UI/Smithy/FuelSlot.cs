using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class FuelSlot
{
    public FuelClip Fuel;
    float MaxBurnTime;
    float BurnTime;
    int Quantity;
    public bool Add(FuelClip clip)
    {
        if (!Fuel)
        {
            Fuel = clip;
            MaxBurnTime = clip.CanBurnTime;
            Quantity++;
            return true;
        }
        else
        if (Quantity < clip.MaxQuantity)
        {
            Quantity++;
            return true;
        }
        return false;
    }
    public bool IsEnough()
    {
        if(BurnTime<0.1f&&Quantity!=0)
        {
            Quantity--;
            BurnTime = MaxBurnTime;
            Burning();
            return true;
        }
        else if(BurnTime > 0.1f)
        {
            Burning();
            return true;
        }
        else
        {
            Fuel = null;
            return false;
        }
    }
    void Burning()
    {
        BurnTime -= 1;
    }
    public void UpadateUI(Slider Bar,TextMeshProUGUI UI)
    {
        Bar.maxValue = MaxBurnTime;
        Bar.value = BurnTime;
        UI.text = "" + Quantity;
    }
}
