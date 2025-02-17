using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GMTool : MonoBehaviour
{
    [SerializeField] GameManger Gm;
    [SerializeField] GameObject Main;
    [SerializeField] GameObject Type;
    [SerializeField] GameObject Value;
    [SerializeField] List<GameObject> List = new List<GameObject>();
    [Header("更動項目")]
    [SerializeField] Button B_Type;
    [SerializeField] Button B_Value;
    [SerializeField] AddValue AddValueUI;
    [Header("預製物")]
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] Transform SpawnPoint;
    void BackMain()
    {
        Main.SetActive(true);
        Type.SetActive(false);
        Value.SetActive(false);
    }
    public void AddResearchPoint() //增加研究進度
    {
        AddValueUI.AddResearch(Gm.Research);
        SetBackButton(B_Value, Main, Value);
    }
    public void IncreaseMaxWeight()
    {
        AddValueUI.AddInventoryWeight(Gm.InventorySystem);
        SetBackButton(B_Value, Main, Value);
    }
    public void GenerateResourceButton()
    {
        Clear();
        for (int i =0;i<Gm.DateBase.A_Resource.Count;i++)
        {
            ItemClip Clip = Gm.DateBase.A_Resource[i];
            GameObject New = NewItemButton(Clip.Name);
            Button button = New.GetComponent<Button>();
            button.onClick.AddListener(() => AddValueUI.AddResource(Clip, Gm.ResourceSystem));
            GotPreset(button);
            List.Add(New);
        }
        SetBackButton(B_Value, Value, Type);
    }
    public void GenerateDropItemButton()
    {
        Clear();
        for (int i=0;i<Gm.DateBase.A_Items.Count;i++)
        {
            ItemClip Clip = Gm.DateBase.A_Items[i];
            GameObject New = NewItemButton(Clip.Name);
            Button button = New.GetComponent<Button>();
            button.onClick.AddListener(() => AddValueUI.DropItem(Clip, Gm, SpawnPoint));
            GotPreset(button);
            List.Add(New);
        }
        SetBackButton(B_Value, Value, Type);
    }
    private void Clear()
    {
        for(int i=0;i<List.Count;i++)
        {
            Destroy(List[i]);
        }
        List.Clear();
    }
    private GameObject NewItemButton(string Name)
    {
        GameObject New = Instantiate(ButtonPrefab, Type.transform);
        New.transform.SetAsFirstSibling();
        UnityEngine.Localization.Components.LocalizeStringEvent Event = New.GetComponent<UnityEngine.Localization.Components.LocalizeStringEvent>();
        LanguageSystem.SetText(Event, "Items," + Name);
        return New;
    }
    private void SetBackButton(Button button,GameObject objA,GameObject objB)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => UI_interface(objA));
        button.onClick.AddListener(() => UI_interface(objB));
    }
    private void GotPreset(Button button)
    {
        button.onClick.AddListener(() => UI_interface(Type));
        button.onClick.AddListener(() => UI_interface(Value));
    }
    public void UI_interface(GameObject obj) //開關UI介面
    {
        obj.SetActive(!obj.activeSelf);
        if (!obj.activeSelf && obj == gameObject)
        {
            BackMain();
        }
    }
}
