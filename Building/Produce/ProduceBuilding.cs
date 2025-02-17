using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ProduceBuilding : Building
{
    [SerializeField] OrderClip WorkOrde;
    public ProduceBuildingType Type;
    [SerializeField]protected List<ProduceClip> QueueList = new List<ProduceClip>();
    [SerializeField] List<ProduceClip> Finshed = new List<ProduceClip>();
    public List<ProduceClip> GotQueueList { get { return QueueList; } }
    public int MaxQueue;
    bool AllFinish;
    [SerializeField]float _NowPower;
    public float NowPower
    {
        get
        {
            int i = 0;
            _NowPower = 0;
            while (i< clip.WorkingPeple.Count)
            {
                _NowPower += clip.WorkingPeple[i].Stats.ProducePower;
                i++;
            }
            return _NowPower;
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        GotWorkOrder(WorkOrde);
    }
    protected override void OpenUI()
    {
        GM.ProduceSystem.OpenUI(!GM.ProduceSystem.ProduceUITotal.activeSelf);
        GM.UIManger.UI_interface(GM.ProduceSystem.ProduceUITotal);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GM.ProduceSystem.NowBuilding = this;
            InteractiveUI.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GM.ProduceSystem.NowBuilding = null;
            InteractiveUI.SetActive(false);
        }
    }
    public virtual void AddProduce(ProduceClip produce)//新增製作項目
    {
        QueueList.Add(produce);
    }
    public virtual void Action()
    {
        if (QueueList.Count != 0)
        {
            if (!IsInvoking(nameof(Makeing)))
            {
                InvokeRepeating(nameof(Makeing), 1, 1);
            }
        }
    }
    protected void Makeing()//製作中
    {
        if(VillagerIsWorking)
        {
            if (QueueList.Count != 0)
            {
                bool IsFinished = QueueList[0].Making(NowPower);
                if (IsFinished)
                {
                    if (SwitchProduce())
                    {
                        AM_Villager.ActionAnmtion("Action", false);
                        //Debug.Log("已經完成");
                    }
                }
                else
                {
                    AM_Villager.ActionAnmtion("Action", true);
                }
            }
            else
            {
                AM_Villager.ActionAnmtion("Action", false);
                CancelInvoke(nameof(Makeing));
            }
        }
        else
        {
            AM_Villager.ActionAnmtion("Action", false);
            CancelInvoke(nameof(Makeing));
        }
    }
    protected  bool SwitchProduce()
    {
        int i = 1;
        while (i < QueueList.Count)
        {
            if (!QueueList[i].IsFinish)
            {
                ProduceClip IsFinish = QueueList[0];
                QueueList[0] = QueueList[i];
                QueueList[i] = IsFinish;
                AllFinish = false;
                return false;
            }
            else if (i == QueueList.Count)
            {
                AllFinish = true;
                return true;
            }
            i++;
        }
        return true;
    }
    #region 外部取得
    public virtual void GotAllList(out List<ProduceClip> ProduceList,out List<ProduceClip> QueueList)
    {
        ProduceList = null;
        QueueList = this.QueueList;
    }
    public void UpdateQueueBar(ref List<QueueProduceUI> QueueUI)
    {
        if (QueueUI.Count!=0)
        {
            QueueUI[0].UpdateBar();
            if (!AllFinish && QueueUI[0].Produce.IsFinish && QueueUI.Count > 1)
            {
                SwitchUI(ref QueueUI);
            }
        }
    }
    void SwitchUI(ref List<QueueProduceUI> QueueUI)
    {
        int i = 1;
        while(i< QueueUI.Count)
        {
            if(!QueueUI[i].Produce.IsFinish)
            {
                QueueProduceUI IsFinish = QueueUI[0];
                IsFinish.transform.SetAsLastSibling();
                QueueUI[0] = QueueUI[i];
                QueueUI[i] = IsFinish;
            }
            i++;
        }
    }
    #endregion
}
