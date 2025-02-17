using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Quest", menuName = "Type/Quest")]
public class QuestClip : ScriptableObject
{
    public enum QuestType //任務類別
    {
        Kill,
        Search,
    }
    public QuestType type;
    public string Name; //標題
    public Sprite Icon; //圖標
    public int C_value;//完成次數
    public bool IsRepetitive;//是否重複任務
    public bool Doing;//執行中
    public bool Complete;//達成目標
    [TextArea(8,10)]
    public string Description; //敘述
    public QuestGoal[] Goals; // 目標
    public QuestCondtion[] Condition; //前置任務
    public QuestReward[] Reward;//獎勵
    public void GotGoal(EnemyClip Goal)
    {
        for(int goal=0;goal< Goals.Length;goal++)
        {
            if(Goals[goal].Goal==Goal)
            {
                Goals[goal].AddAmount();
            }
            if(!Goals[goal].Complete)//確認所有目標是否達成
            {
                break;
            }
        }
    }
    public void EndQuest()
    {
        for (int goal = 0; goal < Goals.Length; goal++)
        {
            Goals[goal].Complete = false;
            Goals[goal].Amount = 0;
            Doing = false;
            if(!IsRepetitive)
            {
                Complete = true;
            }
            C_value++;
        }
    }
    public void Copy(QuestClip NewQuest)
    {
        NewQuest.Name = Name;
        NewQuest.Icon = Icon;
        NewQuest. Description = Description;
        NewQuest. Reward = Reward;
        NewQuest. Condition = Condition;
        NewQuest.Goals = new QuestGoal[Goals.Length];
        for (int i=0;i<Goals.Length;i++)
        {
            NewQuest.Goals[i] = new QuestGoal();
            Goals[i].New(NewQuest.Goals[i]);
        }

    }
}
[System.Serializable]
public class QuestGoal
{
    public EnemyClip Goal;//目標
    public Sprite Icon;//目標圖標
    public int Amount;//數量
    public int MaxAmount; //最大數量
    public bool Complete;
    public void AddAmount()
    {
        if (!Complete)
        {
            if (Amount < MaxAmount)
            {
                Amount++;
                if(Amount== MaxAmount)
                {
                    Complete = true;
                }
            }
        }
    }
    public void New(QuestGoal Copy)
    {
        Copy.Goal = Goal;
        Copy.Icon = Icon;
        Copy.MaxAmount = MaxAmount;
    }
}
[System.Serializable]
public class QuestReward
{
    public enum Type //任務類別
    {
        Money,
        Item,
    }
    public Type type;
    public Sprite Icon;
    public int Value;
    public string Item;
}
[System.Serializable]
public class QuestCondtion
{
  public QuestClip Quest;
  public bool Complate;
}
