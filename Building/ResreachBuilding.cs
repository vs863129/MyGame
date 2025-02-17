using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResreachBuilding : Building
{
    [SerializeField] OrderClip WorkOrder;
    [SerializeField]float MaxTime;
    float _time;
    public float NowTime
    {
        get
        {
            return _time;
        }
        set
        {
            _time = value;
            if (_time >= MaxTime)
            {
                TimeCallBack();
                _time = 0;
            }
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        GotWorkOrder(WorkOrder);
    }
    public void Action()
    {
        if (CanResearch())
        {
            if(VillagerIsWorking)
            {
                AM_Villager.ActionAnmtion("Action", true);
                if (!IsInvoking(nameof(ActionResearch)))
                {
                    InvokeRepeating(nameof(ActionResearch), 1, 1);
                }
            }
            else
            {
                AM_Villager.ActionAnmtion("Action", false);
                Debug.Log("沒有村民正在研發科技");
            }

        }
        else
        {
            if (IsInvoking(nameof(ActionResearch)))
            {
                AM_Villager.ActionAnmtion("Action", false);
                CancelInvoke(nameof(ActionResearch));
            }
        }
    }
    void ActionResearch()
    {
        NowTime++;
    }
    bool CanResearch()
    {
        if(GM.Research.NowProject)
        {
         return IsOpen();
        }
        return false;
    }
    private void TimeCallBack()
    {
        float villagerSates = 0;
        for (int i=0;i<clip.WorkingPeple.Count;i++)
        {
            villagerSates += clip.WorkingPeple[i].Stats.Intelligence;
        }
        GM.Research.ToPoint(villagerSates);
    }
}
