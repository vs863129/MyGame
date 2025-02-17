using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class QueueProduceUI : MonoBehaviour
{
    public ProduceClip Produce;
    [SerializeField] Button GotFinishedItem;
    [SerializeField] Slider Bar;
    [SerializeField] Image Image;
    [SerializeField] TextMeshProUGUI Quantity;
    [SerializeField] TextMeshProUGUI FinishQuantity;
    [Header("綁定系統")]
    [SerializeField] ResourceSystem RS;
    [SerializeField] ProduceSystem PS;
    public void Generate(ProduceClip Produce,ResourceSystem RS,ProduceSystem PS)
    {
        this.Produce = Produce;
        Produce.GotItem();
        Bar.maxValue = this.Produce.MakeTime;
        Bar.value = Produce.Progress;
        Image.sprite = this.Produce.item.Icon;
        this.PS = PS;
        this.RS = RS;
        UpdateBar();
    }
    public void GenerateUI(ProduceClip Produce, ResourceSystem RS, ProduceSystem PS)
    {
        this.Produce = Produce;
        Bar.maxValue = this.Produce.MakeTime;
        Bar.value = Produce.Progress;
        Image.sprite = this.Produce.item.Icon;
        this.PS = PS;
        this.RS = RS;
        UpdateBar();
    }
    public void UpdateBar() //更新製作狀態
    {
        Bar.value = Produce.Progress;
        Quantity.text = ""+ Produce.Quantity;
        FinishQuantity.text = "" + Produce.QueueUIGotFinishValue;
    }
    public void GotItem()
    {
        if (Produce)
        {
            int GotQuantity = Produce.GotFinishQuantity;
            switch (Produce.item.type)
            {
                case ItemType.Resource:
                    if (Produce.item.type == ItemType.Resource)
                    {
                        if (GotQuantity > 0)
                        {
                            RS.Add(Produce.item, GotQuantity);
                            if (Produce.Quantity == 0)
                            {
                                PS.RmoveQueueUI(this);
                            }
                        }
                        UpdateBar();
                    }
                    break;
                default:
                    break;
            }

        }
    }
    public void Cancel()
    {
        if (Produce)
        {
            if(Produce.Cancel(RS))
            {
                PS.RmoveQueueUI(this);
                Destroy();
            }
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
