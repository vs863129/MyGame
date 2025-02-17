using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class AddValue : MonoBehaviour
{
    [SerializeField] private Button One;
    [SerializeField] private Button Ten;
    [SerializeField] private Button Hundred;
    private void Clear()
    {
        One.onClick.RemoveAllListeners();
        Ten.onClick.RemoveAllListeners();
        Hundred.onClick.RemoveAllListeners();
    }
    public void AddResource(ItemClip Clip, ResourceSystem system)
    {
        Clear();
        One.onClick.AddListener(() => { system.Add(Clip, 1); });
        Ten.onClick.AddListener(() => { system.Add(Clip, 10); });
        Hundred.onClick.AddListener(() => { system.Add(Clip, 100); });
    }
    public void AddResearch(ResearchSystem system)
    {
        Clear();
        One.onClick.AddListener(() => { system.ToPoint(1); });
        Ten.onClick.AddListener(() => { system.ToPoint(10); });
        Hundred.onClick.AddListener(() => { system.ToPoint(100); });
    }
    public void DropItem(ItemClip clip,GameManger system,Transform SpawnPoint)
    {
        Clear();
        One.onClick.AddListener(() => { system.DropItem(clip, SpawnPoint.position,1); });
        Ten.onClick.AddListener(() => { system.DropItem(clip, SpawnPoint.position, 10); });
        Hundred.onClick.AddListener(() => { system.DropItem(clip, SpawnPoint.position, 100); });
    }
    public void AddInventoryWeight(InventorySystem system)
    {
        Clear();
        One.onClick.AddListener(() => { system.IncreaseMaxWeight(1); });
        Ten.onClick.AddListener(() => { system.IncreaseMaxWeight(10); });
        Hundred.onClick.AddListener(() => { system.IncreaseMaxWeight(100); });
    }
}
