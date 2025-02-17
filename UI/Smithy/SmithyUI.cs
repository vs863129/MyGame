using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SmithyUI : GeneralProductionUI
{
    [Header("鍛造屋設定")]
    [SerializeField] GameObject FuelRecipePrefab;
    [SerializeField] GameObject FuelUIPrefab;
    [SerializeField] Transform FuelList;
    [SerializeField] ToggleGroup ActionPage;
    [Header("列表")]
    [SerializeField] List<GameObject> FuelRecipeUI=new List<GameObject>();
    [SerializeField] List<GameObject> FuelSlots=new List<GameObject>();

    public string GotActionToggleName
    {
        get
        {
            return ActionPage.ActiveToggles().FirstOrDefault().name;
        }
    }
    public void GenerateFuelUI(SmithyBuilding Smithy, GameObject CostResourceUI, List<ResourceUI> OriginalCost,ResourceSystem Rs)
    {
        ClearRecipeUI();
        foreach (FuelClip fuelClip in Smithy.FuelRecipeList)
        {
            FuelRecipeUI.Add(GenerateFuelRecipe(fuelClip, Smithy, CostResourceUI, Rs, OriginalCost));
        }
    }
    public void GenerateCostResoruceUI(FuelClip OriginalClip, GameObject ResourceUIPrefab, Transform CostList, List<ResourceUI> Original, List<ResourceUI> Store, bool IsON)
    {
        Clear(Original);
        if (IsON)
        {
            for (int i = 0; i < OriginalClip.Cost.Count; i++)
            {
                GameObject CostUI = Instantiate(ResourceUIPrefab, CostList);
                ResourceUI Script = CostUI.GetComponent<ResourceUI>();
                Script.SpawnUI(OriginalClip.Cost[i].Clip, OriginalClip.Cost[i].Value);
                Script.CheckIsEnough(Store);
                Original.Add(Script);
            }
        }
    }

    GameObject GenerateFuelRecipe(FuelClip clip, SmithyBuilding Smithy, GameObject CostResourceUI,ResourceSystem Rs, List<ResourceUI> OriginalCost)
    {
        GameObject gameObject= Instantiate(FuelRecipePrefab, ProduceList);
        FuelRecipeUI Script = gameObject.GetComponent<FuelRecipeUI>();
        Script.LoadDate(clip,Rs, Smithy, FuelSlots);
        Script.toggle.group = ProduceGroup;
        Script.toggle.onValueChanged.AddListener((value) => GenerateCostResoruceUI(clip, CostResourceUI,CostList, OriginalCost, Rs.Store, value));
        return gameObject;
    }
    public void ClearRecipeUI()
    {
        foreach(GameObject gameObject in FuelRecipeUI)
        {
            Destroy(gameObject);
        }

        FuelRecipeUI.Clear();
    }
    public void GenerateFuelSlotsUI(SmithyBuilding Smithy)
    {
        foreach (GameObject gameObject in FuelSlots)
        {
            Destroy(gameObject);
        }
        FuelSlots.Clear();
        int i = 0;
        while(i< Smithy.GotMaxFuelSlots)
        {
            GameObject NewFuelSlot = Instantiate(FuelUIPrefab, FuelList);
            if(Smithy.FuelSlots.Count>i)
            {
                FuelSlotUI Script = NewFuelSlot.GetComponent<FuelSlotUI>();
                Script.AddSlot(Smithy.FuelSlots[i]);
            }
            FuelSlots.Add(NewFuelSlot);
            i++;
        }
    }
}
