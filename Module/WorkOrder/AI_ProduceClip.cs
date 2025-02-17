using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ProduceOredr", menuName = "WorkOrder/Produce")]
public class AI_ProduceClip : OrderClip
{
    public override void Wokring()
    {
        if(villager.Place!=WokrBuilding)
        {
            EnterWorkPlace();
        }
        else
        {
            WokrBuilding.GetComponent<ProduceBuilding>().Action();
        }
    }
}