using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class FuelRecipeUI : MonoBehaviour
{
    [SerializeField] FuelClip FuelClip;
    [SerializeField] Image Icon;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI BurnTime;
    [SerializeField] Button ActionBurn;
    public Toggle toggle;
    public void LoadDate(FuelClip fuelClip,ResourceSystem Rs, SmithyBuilding Smithy, List<GameObject> SlotsUI)
    {
        FuelClip = fuelClip;
        Icon.sprite = FuelClip.Item.Icon;
        Name.text = FuelClip.Item.Name;
        BurnTime.text = "" + FuelClip.CanBurnTime;
        ActionBurn.onClick.AddListener(()=> ActionBrunButton(Rs, Smithy, SlotsUI));
    }
    public void ActionBrunButton(ResourceSystem Rs, SmithyBuilding Smithy, List<GameObject> SlotsUI)
    {
        if (Rs.CheckResource(FuelClip.Cost))
        {
            Rs.Pay(FuelClip.Cost);
            GenerateFuelDate(ref Smithy, SlotsUI);
        }
    }
    void GenerateFuelDate(ref SmithyBuilding Smithy, List<GameObject> SlotsUI)
    {
        if(Smithy.FuelSlots.Count!=0)
        {
            int i = 0;
            while(i< Smithy.FuelSlots.Count)
            {
                if(Smithy.FuelSlots[i].Fuel==FuelClip)
                {
                    if (Smithy.FuelSlots[i].Add(FuelClip))
                    {
                        break;
                    }
                    else if(i== Smithy.FuelSlots.Count-1&& Smithy.FuelSlotFull)
                    {
                        FuelSlot NewFuelSlot = new FuelSlot();
                        NewFuelSlot.Add(FuelClip);
                        Smithy.FuelSlots.Add(NewFuelSlot);
                        FuelSlotUI SlotUI = SlotsUI[i+1].GetComponent<FuelSlotUI>();
                        SlotUI.AddSlot(NewFuelSlot);
                        break;
                    }
                }
                else if(i== Smithy.FuelSlots.Count-1&& Smithy.FuelSlots[i].Fuel != FuelClip&& Smithy.FuelSlotFull)
                {
                    FuelSlot NewFuelSlot = new FuelSlot();
                    NewFuelSlot.Add(FuelClip);
                    Smithy.FuelSlots.Add(NewFuelSlot);
                    FuelSlotUI SlotUI = SlotsUI[i+1].GetComponent<FuelSlotUI>();
                    SlotUI.AddSlot(NewFuelSlot);
                    break;
                }
                i++;
            }
        }
        else
        {
            FuelSlot NewFuelSlot = new FuelSlot();
            NewFuelSlot.Add(FuelClip);
            Smithy.FuelSlots.Add(NewFuelSlot);
            FuelSlotUI SlotUI = SlotsUI[0].GetComponent<FuelSlotUI>();
            SlotUI.AddSlot(NewFuelSlot);
        }

    }
}
