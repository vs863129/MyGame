using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResearchOredr", menuName = "WorkOrder/Research")]
public class AI_Research_Clip : OrderClip
{
    public override void Wokring()
    {
        base.Wokring();
        if(villager.Place==WokrBuilding)
        {
            WokrBuilding.GetComponent<ResreachBuilding>().Action();
        }
        else
        {
            EnterWorkPlace();
        }
    }
}
