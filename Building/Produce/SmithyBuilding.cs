using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SmithyBuilding : ProduceBuilding
{
    [Header("配方")]
    [SerializeField] List<ProduceClip> Melt = new List<ProduceClip>();
    public List<ProduceClip> MeltList => Melt;
    [SerializeField] List<ProduceClip> Forge = new List<ProduceClip>();
    public List<ProduceClip> ForgeList => Forge;
    [SerializeField] List<FuelClip> Fuel = new List<FuelClip>();
    public List<FuelClip> FuelRecipeList => Fuel;
    [SerializeField] List<FuelSlot> FuelSlot = new List<FuelSlot>();
    public List<FuelSlot> FuelSlots => FuelSlot;
    [SerializeField] int MaxFuelSlots;
    public int GotMaxFuelSlots => MaxFuelSlots;
    public bool FuelSlotFull
    {
        get
        {
            if(FuelSlots.Count!= MaxFuelSlots)
            {
                return true;
            }
            else
            {
                Debug.Log("沒有足夠的空間");
                return false;
            }
        }
    }

    protected override void OpenUI()
    {
        if (!GM.ProduceSystem.ProduceUITotal.activeSelf)
        {
            base.OpenUI();
            GM.ProduceSystem.SwitchPage(true);
        }
        else
        {
            base.OpenUI();
        }
    }
    public override void GotAllList(out List<ProduceClip> ProduceList, out List<ProduceClip> QueueList)
    {
        ProduceList = Forge;
        QueueList = GotQueueList;
    }
    public override void Action()
    {
        if (QueueList.Count != 0)
        {
            switch (QueueList[0].Type)
            {
                case ProduceType.Normal:
                    if (!IsInvoking(nameof(Makeing)))
                    {
                        InvokeRepeating(nameof(Makeing), 1, 1);
                    }
                    break;
                case ProduceType.Melt:
                    if (!IsInvoking(nameof(CastingMinerals)))
                    {
                        InvokeRepeating(nameof(CastingMinerals), 1, 1);
                    }
                    break;
            }
        }
        else
        {
            CancelInvoke(nameof(CastingMinerals));
        }
    }
    void CastingMinerals()
    {
        if(!QueueList[0].IsFinish)
        {
            if (!CheckFuel())
            {
                Debug.Log("需要更多燃料");
            }
            else
            {
                bool IsFinishProduce = QueueList[0].Making(NowPower);
                if (IsFinishProduce)
                {
                    if (SwitchProduce() && QueueList.Count > 1)
                    {
                        CancelInvoke(nameof(CastingMinerals));
                    }
                }
            }
        }
        else
        {
            if(QueueList[0])
            {
                Debug.Log("等待完成品取走");
            }
        }
    }
    bool CheckFuel()
    {
        int i = 0;
        while(i< FuelSlots.Count)
        {
            if(FuelSlots[i].Fuel)
            {

                return FuelSlots[i].IsEnough();
            }
            i++;
        }
        return false;
    }

}
