using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GeneralProductionUI : MonoBehaviour
{
    [Header("介面")]
    public GameObject UITotal;
    [Header("通用設定")]
    public GameObject ProduceUIPrefab;
    public Transform ProduceList;
    public Transform QueueList;
    public Transform CostList;
    public Button ActionButton;
    public ToggleGroup ProduceGroup;
    public void GenerateCostResoruceUI(GameManger Gm, ref ProduceClip OriginalClip, GameObject ResourceUIPrefab, Transform CostList, List<ResourceUI> Original, bool IsON)
    {
        Clear(Original);
        if (IsON)
        {
            for (int i = 0; i < OriginalClip.Cost.Count; i++)
            {
                GameObject CostUI = Instantiate(ResourceUIPrefab, CostList);
                ResourceUI Script = CostUI.GetComponent<ResourceUI>();
                Script.SpawnUI(OriginalClip.Cost[i].Clip, OriginalClip.Cost[i].Value);
                Gm.CheckResource(Script);
                Original.Add(Script);
            }
        }
        else
        {
            OriginalClip = null;
        }
    }
    protected void Clear(List<ResourceUI> Original)
    {
        for (int i = 0; i < Original.Count; i++)
        {
            Original[i].Destroy();
        }
        Original.Clear();
    }
}
