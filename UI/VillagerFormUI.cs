using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VillagerFormUI : MonoBehaviour
{
    public VillagerClip Villager;
    public VillagersManger VM;
    #region 設定
    [SerializeField] Image Head;
    [SerializeField] Image Hat;
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI Health;
    [SerializeField] TextMeshProUGUI Food;
    [SerializeField] TextMeshProUGUI Fatigue;
    [SerializeField] Toggle Toggle;
    #endregion
    private void FixedUpdate()
    {
        if(Villager)
        {
            Name.text = Villager.Stats.Name;
            Health.text = ""+(int)Villager.Stats.Health;
            Food.text=""+ (int)Villager.Stats.Food;
            Fatigue.text=""+ (int)Villager.Stats.Fatigue;
        }
    }
    public void Generate(VillagerClip Data,ToggleGroup Group)
    {
        Villager = Data;
        if(Data.GotHat())
        {
            Hat.sprite = Data.GotHat();
        }
        Toggle.group = Group;
    }
    public void Action()
    {
        VM.StateUI.LoadData(Villager);
    }
}
