using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VillagerListUI_States : MonoBehaviour
{
    [SerializeField]VillagersManger VM;
    [SerializeField] GameObject StatesUI;
    [Header("狀態設定")]
    [SerializeField] TextMeshProUGUI VillagerName;
    [SerializeField] TextMeshProUGUI Home;
    [SerializeField] TextMeshProUGUI WorkPlace;
    [SerializeField] TextMeshProUGUI Job;
    [SerializeField] Image JobIcon;
    [Header("村民服裝")]
    [SerializeField] Image Clothe;
    [SerializeField] Image LSleeve;
    [SerializeField] Image RSleeve;
    [Header("屬性")]
    [SerializeField] TextMeshProUGUI[] BaseAttribute;//0 STR 1 AGI 2 INT 3LUC
    [SerializeField] TextMeshProUGUI[] AnotherAttribute;//0 CollectPower 1 CollectSpeed 2 ProducePower
    void FixedUpdate()
    {
        if(!VM.UIGroup.AnyTogglesOn())
        {
            StatesUI.SetActive(false);
        }
    }
    public void LoadData(VillagerClip data)
    {
        StatesUI.SetActive(true);
        #region 衣服
        data.LoadClothing(out Sprite Clothe, out Sprite HeadWear, out Sprite Sleeve, out Sprite Shoe);
        if(Clothe == null)
        {
            this.Clothe.enabled = false;
        }
        else
        {
            this.Clothe.enabled = true;
            this.Clothe.sprite = Clothe;
        }
        if (Sleeve == null)
        {
            LSleeve.enabled = false;
            RSleeve.enabled = false;
        }
        else
        {
            LSleeve.enabled = true;
            RSleeve.enabled = true;
            LSleeve.sprite = Sleeve;
            RSleeve.sprite = Sleeve;
        }
        #endregion
        VillagerName.text = data.Stats.Name;
        if(data.Home!=null)
        {
            Home.text = data.Working.GetComponent<Building>().clip.Name;
        }
        else
        {
            Home.text = "None";
        }
        if (data.Working != null)
        {
            WorkPlace.text = data.Working.GetComponent<Building>().clip.Name;
        }
        else
        {
            WorkPlace.text = "None";
        }
        Job.text = "" + JobName(data);
        BaseAttribute[0].text = "" + data.Stats.Strength;
        BaseAttribute[1].text = "" + data.Stats.Agile;
        BaseAttribute[2].text = "" + data.Stats.Intelligence;
        BaseAttribute[3].text = "" + data.Stats.Lucky;
        AnotherAttribute[0].text = "" + data.Stats.CollectPower;
        AnotherAttribute[1].text = "" + data.Stats.CollectSpeed;
        AnotherAttribute[2].text = "" + data.Stats.ProducePower;
    }
    string JobName(VillagerClip data)
    {
        if(data.Working!=null)
        {
            WorkPosition workposition = data.Working.GetComponent<Building>().clip.m_WorkPosition;
            switch (workposition)
            {
                case WorkPosition.none:
                    return "None";
                case WorkPosition.Lumberjack:
                    return "Lumberjack";
                case WorkPosition.Miner:
                    return "Miner";
                case WorkPosition.Maker:
                    return "Maker";
                case WorkPosition.Researcher:
                    return "Researcher";
                case WorkPosition.Blacksmith:
                    return "Blacksmith";
                default:
                    return "None";
            }
        }
        else
        {
            return "None";
        }
    }
}
