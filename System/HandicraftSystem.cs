using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandicraftSystem : MonoBehaviour
{
    [Header("可製作項目")]
    [SerializeField] List<ProduceClip> Canbemade=new List<ProduceClip>();
    [SerializeField] List<ProduceClip> QueueList= new List<ProduceClip>();
    [Header("判斷")]
    bool AllFinish;
    [Header("其他系統")]
    [SerializeField] GameManger GM;
    #region 外部取得
    public void GotProduceList(out List<ProduceClip> ProduceList, out List<ProduceClip> QueueList)
    {
        ProduceList = Canbemade;
        QueueList = this.QueueList;
    }
    #endregion
    public void OpenUI()
    {
        GM.ProduceSystem.LoadUI(this);
        GM.UIManger.UI_interface(GM.ProduceSystem.ProduceUITotal);
        if(GM.ProduceSystem.ProduceUITotal.activeSelf)
        {
            if (!IsInvoking(nameof(UpdateQueueBar)))
            {
                InvokeRepeating(nameof(UpdateQueueBar), 1, 1);
            }
        }
        else
        {
            if (IsInvoking(nameof(UpdateQueueBar)))
            {
                CancelInvoke(nameof(UpdateQueueBar));
            }
        
        }
    }
    public void Action(ProduceClip produce)
    {
        QueueList.Add(produce);
        if (!IsInvoking(nameof(Making)))
        {
            InvokeRepeating(nameof(Making),1,1);
        }
    }
    public void UpdateQueueBar()
    {
        List<QueueProduceUI> QueueUI = GM.ProduceSystem.GotAllQueueList;
        if (QueueUI.Count != 0)
        {
            if(!QueueUI[0].Produce)
            {
                if (QueueUI.Count > 1)
                {
                    SwitchUI(ref QueueUI);
                    return;
                }
                QueueUI[0].Destroy();
                QueueUI.Clear();
            }
            else
            {
                QueueUI[0].UpdateBar();
            }
        }
    }
    void SwitchUI(ref List<QueueProduceUI> QueueUI)
    {
        int i = 1;
        while (i < QueueUI.Count)
        {
            if (QueueUI[i].Produce)
            {
                QueueProduceUI IsFinish = QueueUI[0];
                IsFinish.transform.SetAsLastSibling();
                QueueUI[0] = QueueUI[i];
                QueueUI[i] = IsFinish;
                IsFinish.Destroy();
                QueueUI.RemoveAt(i);
                return;
            }
            i++;
        }
    }
    public void Making()
    {
        if(QueueList.Count!=0)
        {
            QueueList[0].Making(1);
            if(!QueueList[0])
            {
                if(QueueList.Count>1)
                {
                    for (int i = 1; i < QueueList.Count; i++)
                    {
                        if (!QueueList[i].IsFinish)
                        {
                            QueueList[0] = QueueList[i];
                            QueueList.RemoveAt(i);
                            return;
                        }
                    }
                }else
                {
                    QueueList.Clear();
                }
            }
            else if(QueueList[0].QueueUIGotFinishValue!=0)
            {
                Item FinishItem = new Item();
                FinishItem.AddValue(QueueList[0].GotFinishQuantity);
                FinishItem.Clip = QueueList[0].item;
                GM.InventorySystem.IgnoreWeight_GotItme(FinishItem.Clip);
            }
        }
        else
        {
            CancelInvoke(nameof(Making));
        }
    }
}
