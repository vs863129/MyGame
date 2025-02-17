using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchUIManger : MonoBehaviour
{
    public GameObject[] ResearchList;
    public void Check(List<ResearchClip> IsDoneList)
    {
        for(int i=0;i< ResearchList.Length; i++)
        {
            ResearchUI Scrip = ResearchList[i].GetComponent<ResearchUI>();
            Scrip.Check(IsDoneList);
        }
    }
    public void UpdateLine()
    {
        for (int Start = 0; Start < ResearchList.Length; Start++)
        {
            ResearchUI UI = ResearchList[Start].GetComponent<ResearchUI>();
            if (UI.Clip.CheckCondition)
            {
                for (int Times = 0; Times < UI.Clip.GotConditionLength; Times++)
                {
                    ResearchClip clip = UI.Clip.GotCondition(Times).R_clip;
                    Transform[] Points = { ResearchList[Start].transform, EndPoint(clip) };
                    ResearchList[Start].GetComponent<ResearchUI>().DrawLine(Times,Points);
                }
            }
            else
            {
                continue;
            }
        }
    }
    private Transform EndPoint(ResearchClip clip)
    {
        for(int i=0;i< ResearchList.Length;i++)
        {
            ResearchClip EndClip = ResearchList[i].GetComponent<ResearchUI>().Clip;
            if(clip == EndClip)
            {
                return ResearchList[i].transform;
            }
        }
        return null;
    }
}

