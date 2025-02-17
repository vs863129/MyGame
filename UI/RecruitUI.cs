using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RecruitUI : MonoBehaviour
{
    public Button Recruit_Button;
    public TextMeshProUGUI[] StatsUI; //0 STR 1 AGI 2 INT 3 LUC
    public void UpdateUI(int ID,float value)
    {
        StatsUI[ID].text = "" + value;
    }
    public void RecruitFinish()
    {
        gameObject.SetActive(false);
        Destroy(gameObject,0.1f);
    }
}
