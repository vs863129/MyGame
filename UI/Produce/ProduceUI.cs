using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

public class ProduceUI : MonoBehaviour
{
    [SerializeField] ProduceClip Produce;
    [SerializeField] Image Image;
    [SerializeField] LocalizeStringEvent Name;
    float DoublePressTime;
    bool DoublePress;
    public void Generate(ProduceClip Clip)
    {
        Produce = Clip;
        Image.sprite = Clip.item.Icon;
        LanguageSystem.SetText(Name,"Items,"+Clip.item.Name);
    }
    public void GenerateCostResoruceUI(GameManger GM, ref LocalizeStringEvent[] SwitchText,ref ProduceClip OriginalClip, GameObject ResourceUIPrefab,Transform CostList,List<ResourceUI> Original,bool IsON)
    {
        Clear(Original);
        if(IsON)
        {
            for(int x=0;x< SwitchText.Length;x++)
            {
                LanguageSystem.SetText(SwitchText[0],"Items,"+ Produce.item.Name);
                LanguageSystem.SetText(SwitchText[1], "Items_Tootip," + Produce.item.Description);
            }
            for (int i = 0; i < Produce.Cost.Count; i++)
            {
                GameObject CostUI = Instantiate(ResourceUIPrefab, CostList);
                ResourceUI Script = CostUI.GetComponent<ResourceUI>();
                Script.SpawnUI( Produce.Cost[i].Clip,  Produce.Cost[i].Value);
                GM.CheckResource(Script);
                OriginalClip = Produce;
                Original.Add(Script);
            }
        }
        else
        {
            OriginalClip = null;
            for (int x = 0; x < SwitchText.Length; x++)
            {
                LanguageSystem.SetText(SwitchText[x], "Global,Null");
            }
        }
    }
     void FixedUpdate()
    {
        if(DoublePressTime>0.1f)
        {
            DoublePressTime -= Time.fixedDeltaTime;
        }
        else
        {
            DoublePress = false;
        }
    }
    public bool IsDoublePress()
    {
        if(GetComponent<Toggle>().isOn)
        {
            DoublePressTime = 0.5f;
        }
        else
        {
            DoublePressTime = 0;
            DoublePress = false;
            return false;
        }
        if(DoublePress)
        {
            DoublePressTime = 0;
            DoublePress = false;
            return true;
        }
        DoublePress = true;
        return false;
    }
    void Clear(List<ResourceUI> Original)
    {
        for(int i=0;i<Original.Count;i++)
        {
            Original[i].Destroy();
        }
        Original.Clear();
    }
}
