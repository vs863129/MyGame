using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResearchUI : MonoBehaviour
{
    public ResearchClip Clip;
    public Image ICON;
    [SerializeField] TextMeshProUGUI Text;
    private  List<LineRenderer> Line = new List<LineRenderer>();
    public void Awake()
    {
        ICON.sprite = Clip.ICON;
    }
    public void Check(List<ResearchClip> IsDone)
    {
        Button button = GetComponent<Button>();
        if(IsDone.Count!=0)
        {
            for (int i = 0; i < Clip.GotConditionLength; i++)
            {
                for (int x = 0; x < IsDone.Count; x++)
                {
                    if (Clip.GotCondition(i).R_clip == IsDone[x])
                    {
                        if(Clip.GotCondition(i).level <= IsDone[x].level)
                        {
                            button.interactable = true;
                            break;
                        }
                        else
                        {
                            button.interactable = false;
                        }
                    }
                    else 
                    {
                        button.interactable = false;
                    }
                }
            }
        }
        else if (Clip.GotConditionLength == 0)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }
    public void DrawLine(int ID,Transform[] point)
    {
        if (Line.Count != Clip.GotConditionLength)
        {
            for(int i=0;i< Clip.GotConditionLength;i++)
            {
                GameObject NewPrefab= Instantiate(new GameObject(),this.transform);
                SpawnLineRenderer(point, NewPrefab);
            }
        }
        else
        {
            for (int i = 0; i < Line.Count; i++)
            {
                Vector2 Start = point[0].position;
                Vector2 End = point[1].position;
                Vector3[] PointArray = { Start, End };
                Line[ID].SetPositions(PointArray);
            }
        }
    }
    public void SpawnLineRenderer(Transform[] point,GameObject gameObject)
    {
            LineRenderer NewLineRenderer = gameObject.AddComponent<LineRenderer>();
            NewLineRenderer.startWidth = 0.1f;
            Vector2 Start = point[0].position;
            Vector2 End = point[1].position;
            Vector3[] PointArray = { Start, End };
            NewLineRenderer.SetPositions(PointArray);
            Line.Add(NewLineRenderer);
    }
    public void SetLevel()
    {
     
        if(Clip.level==Clip.MaxLevel)
        {
            Text.text = "MAX";
        }
        else
        {
            Text.text = "" + Clip.level;
        }
    }
}
