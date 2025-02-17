using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class StatsUI : MonoBehaviour
{
    [SerializeField] GameManger GM;
    [SerializeField] private TextMeshProUGUI Health;
    [SerializeField] private TextMeshProUGUI Damage;
    [SerializeField] private TextMeshProUGUI Defense;
    [Header("負重")]
    [SerializeField] private TextMeshProUGUI Value;
    [SerializeField] private Slider Bar;
    private void Start()
    {
        UpdateStats();
        UpdateWeight();
    }
    public void UpdateStats()
    {
        Health.text = "" + GM.player.GetComponent<Player>().Health;
        Damage.text = "" + GM.player.GetComponent<Player>().Damage;
        Defense.text = "" + GM.player.GetComponent<Player>().Defense;
    }
    public void UpdateWeight()
    {
        GM.InventorySystem._Date.GotCapacityDate(out float Value, out float MaxValue);
        Bar.value = Value;
        Bar.maxValue = MaxValue;
        this.Value.text = Value + "/" + MaxValue;
    }
}
