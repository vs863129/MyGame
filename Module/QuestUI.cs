using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public QuestClip Quest; //組建
    public TextMeshProUGUI[] TEXT;// 1標題 2 內文 
    public MoreReward Rewards; //獎勵
    public MoreGoals Goals; //目標
    public MoreCondition Condition; //條件
    public GameObject[] NullObj;//空物件
    public void Select(QuestClip _Quest)
    {
        Quest = _Quest;
        ClearUI();
        TEXT[0].text = Quest.Name;
        TEXT[1].text = Quest.Description;
       for(int i =0;i<Quest.Reward.Length;i++)
        {
            Rewards.Add(Instantiate(Rewards.RewardPrefab, Rewards.RewardParent), Quest.Reward[i].Icon, Quest.Reward[i].Value);
        }
        for (int i = 0; i < Quest.Goals.Length; i++)
        {
            Goals.Add(Instantiate(Goals.GoalsPrefab, Goals.GoalsParent), Quest.Goals[i].Icon, Quest.Goals[i].Goal.Name, Quest.Goals[i].Amount, Quest.Goals[i].MaxAmount);
        }
        for (int i = 0; i < Quest.Condition.Length; i++)
        {
            Condition.Add(Instantiate(Condition.ConditionPrefab, Condition.ConditionParent), Quest.Condition[i].Quest.Name);
        }
        for (int i = 0; i < NullObj.Length; i++)
        {
            switch (i)
            {
                case 0:
                    if(Quest.Condition.Length==0)
                    {
                        NullObj[i].SetActive(true);
                    }
                    else
                    {
                        NullObj[i].SetActive(false);
                    }
                    break;
                case 1:
                    if (Quest.Goals.Length == 0)
                    {
                        NullObj[i].SetActive(true);
                    }
                    else
                    {
                        NullObj[i].SetActive(false);
                    }
                    break;
                case 2:
                    if (Quest.Reward.Length == 0)
                    {
                        NullObj[i].SetActive(true);
                    }
                    else
                    {
                        NullObj[i].SetActive(false);
                    }
                    break;
            }

        }
    }
    public void Update()
    {
        if(Quest)
        {
            UpdateUI();
        }
    }
    public void UpdateUI()
    {
        Rewards.Update(Quest);
        Goals.Update(Quest);
        Condition.Update(Quest);
    }
    public void ClearUI()
    {
        Rewards.Clear();
        Goals.Clear();
        Condition.Clear();
        for(int i=0;i< NullObj.Length;i++)
        {
            NullObj[i].SetActive(true);
       }
        for(int i=0;i< TEXT.Length;i++)
        {
            TEXT[i].text = "";
        }
    }
}
[System.Serializable]
public class MoreReward
{
    public GameObject RewardPrefab;//產生用預置物
    public Transform RewardParent;//生成的位置
    [SerializeField]
    private List<GameObject>obj=new List<GameObject>();//已生成物件
    public void Add(GameObject Prefab, Sprite _Icon,int _value)
    {
        obj.Add(Prefab);
        Prefab.transform.localScale = new Vector3(1, 1, 1);
        Prefab.GetComponentInChildren<Image>().sprite = _Icon;
        Prefab.GetComponentInChildren<TextMeshProUGUI>().text = "X" + _value;
    }
    public void Update(QuestClip quest)
    {
        for(int i=0;i< obj.Count;i++)
        {
            obj[i].GetComponentInChildren<TextMeshProUGUI>().text = "X" + quest.Reward[i].Value;
        }
    }
    public void Clear()
    {
        for (int i = 0; i < obj.Count; i++)
        {
            Object.Destroy(obj[i]);
        }

        obj.Clear();
    }
}
[System.Serializable]
public class MoreGoals
{
    public GameObject GoalsPrefab;//產生用預置物
    public Transform GoalsParent;//生成的位置
    [SerializeField]
    public List<GameObject> obj = new List<GameObject>();//已生成物件
    public void Add(GameObject Prefab, Sprite _Icon,string Name ,int Min,int Max)
    {
        obj.Add(Prefab);
        Prefab.transform.localScale = new Vector3(1, 1, 1);
        Prefab.GetComponentInChildren<Image>().sprite = _Icon;
        Prefab.GetComponentInChildren<TextMeshProUGUI>().text = Name +" "+ Min+" / "+Max;
    }
    public void Update(QuestClip quest)
    {
        for (int i = 0; i < obj.Count; i++)
        {
            obj[i].GetComponentInChildren<TextMeshProUGUI>().text = quest.Goals[i].Goal.Name + " " + quest.Goals[i].Amount + "/" + quest.Goals[i].MaxAmount;
        }
    }
    public void Clear()
    {
        for(int i=0;i<obj.Count;i++)
        {
            Object.Destroy(obj[i]);
        }
        obj.Clear();
    }
}
[System.Serializable]
public class MoreCondition
{
    public GameObject ConditionPrefab;//產生用預置物
    public Transform ConditionParent;//生成的位置
    [SerializeField]
    public List<GameObject> obj = new List<GameObject>();//已生成物件
    public void Add(GameObject Prefab, string quest)
    {
        obj.Add(Prefab);
        Prefab.transform.localScale = new Vector3(1, 1, 1);
        Prefab.GetComponentInChildren<TextMeshProUGUI>().text = quest;
    }
    public void Update(QuestClip quest)
    {
        for (int i = 0; i < obj.Count; i++)
        {
            obj[i].GetComponentInChildren<TextMeshProUGUI>().text = ""+ quest.Condition[i].Quest.Name;
        }
    }
    public void Clear()
    {
        for (int i = 0; i < obj.Count; i++)
        {
            Object.Destroy(obj[i]);
        }
        obj.Clear();
    }
}