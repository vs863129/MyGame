using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Hint : MonoBehaviour
{
    public bool Action;
    [SerializeField] Animator Main;
    [SerializeField] Animator Hint;
    string AnimationName = "TutorialHint";
    [SerializeField] string TutorialName;
    public bool IsDone
    {
        get 
        {
            if(TutorialName=="")
            {
                return true;
            }
            if(Main.GetFloat(TutorialName) ==1)
            {
                return true;
            }
            return false;
        }
        set 
        { 
            if(value)
            {
                Main.SetFloat(TutorialName, 1);
            }
            else
            {
                Main.SetFloat(TutorialName, 0);
            }
        }
    }
    [Header("分支")]
    [SerializeField] GameObject NeedOpenUI;
    [SerializeField] GameObject SelectButton;
    [SerializeField] float HintNumber;
    [SerializeField] int AllBranchs;
    [SerializeField] int QuestBranchNumber;
    public bool CollectBranch
    {
        get
        {
            if (QuestBranchNumber == Main.GetFloat("TutorialBranch"))
            {
                return true;
            }
            return false;
        }
    }
    [SerializeField] Tutorial_Quest[] Goal;
    public bool HaveGoal { get { if (Goal.Length != 0) { return true; } return false; } }
    [Header("外部系統")]
    [SerializeField] UIManger UIManger;
    [SerializeField] TutorialManger TutorialManger;
    public bool CollectDone(GameManger GM)
    {
        if (Goal.Length != 0 && CollectBranch)
        {
            int i = 0;
            while (i < Goal.Length)
            {
                if (!Goal[i].IsDone(GM))
                {
                    return false;
                }
                i++;
            }
            Goal = new Tutorial_Quest[0];
        }
        return true;

    }
    public void Update()
    {
        if (AllBranchs != 0)
        {
            if (Main.GetFloat("TutorialBranch") == AllBranchs)
            {
                UIManger.IsTutorialUI = false;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseUI();
                }
            }
        }
    }
    public void CloseUI()
    {
        IsDone = true;
        Main.SetFloat("TutorialBranch", 0);
        TutorialManger.Next();
        if(NeedOpenUI)
        {
            NeedOpenUI.transform.SetAsLastSibling();
        }
    }
    public void OpenUI()
    {
        if(!NeedOpenUI.activeSelf)
        {
            NeedOpenUI.transform.SetAsFirstSibling();
            UIManger.IsTutorialUI=true;
            Main.SetFloat("TutorialBranch", 1);
        }
        else
        {
            Main.SetFloat("TutorialBranch", 0);
        }
    }
    public bool HaveBranch(bool IsNext, TutorialManger tutorialManger)
    {
        if(IsDone)
        {
            return false;
        }
        if (AllBranchs!=0)
        {
            TutorialManger = tutorialManger;
            ActionBranch(IsNext);
            if (Main.GetFloat("TutorialBranch") != AllBranchs)
            {
                if(!IsNext&& Main.GetFloat("TutorialBranch")==0)
                {
                    return false;
                }
            }
            else
            {
                return true;
            }

        }
        return true;
    }
    void ActionBranch(bool IsNext)
    {
        float NowNumber = Main.GetFloat("TutorialBranch");
        if (IsNext)
        {
            if(NowNumber!= AllBranchs)
            {
                Main.SetFloat("TutorialBranch", NowNumber + 1);
            }
        }
        else
        {
            if (NowNumber != 0)
            {
                Main.SetFloat("TutorialBranch", NowNumber - 1);
            }
        }
    }
    public void ActionHint()
    {
        if (NeedOpenUI && Action)
        {
            if(!NeedOpenUI.activeSelf&&Main.GetFloat("TutorialBranch")!=AllBranchs)
            {
                if (Main.GetFloat("TutorialBranch") != 0)
                {
                    Main.SetFloat("TutorialBranch", 0);
                }
                SelectButton.SetActive(true);
                SelectButton.transform.SetAsFirstSibling();
                if(Main.GetFloat(TutorialName)==0)
                {
                    Hint.SetFloat(AnimationName, HintNumber);
                }
            }
        }
    }
    public void ClearHint()
    {
        Hint.SetFloat(AnimationName, 0);
        Action = false;
    }

    [System.Serializable]
    public  class Tutorial_Quest
    {
        [SerializeField] Tutorial_Quest_Type Type;
        [SerializeField]Item NeedItem;
        [SerializeField] BuildingClip NeedBuidlings;
        [SerializeField]int HaveAmount;
        public bool IsDone(GameManger GM)
        {
            switch (Type)
            {
                #region 蒐集
                case Tutorial_Quest_Type.CollectItem:
                    List<Slot> Slot = GM.InventorySystem.SlotsUI;
                    int i = 0;
                    while (i < Slot.Count)
                    {
                        if (Slot[i].Item.Clip != null)
                        {
                            if (Slot[i].Item.Clip == NeedItem.Clip)
                            {
                                HaveAmount = Slot[i].Item.Value;
                                if (HaveAmount >= NeedItem.Value)
                                {
                                    return true;
                                }
                                return false;
                            }
                        }
                        i++;
                    }
                    return false;
                #endregion
                #region 建築
                case Tutorial_Quest_Type.Buidling:
                    List<GameObject> ComplateBuldings = GM.builderSystem.Complate;
                    if(ComplateBuldings.Count!=0)
                    {
                        int x = 0;
                        while(x<ComplateBuldings.Count)
                        {
                            BuildingClip building = ComplateBuldings[x].GetComponent<Building>().clip;
                            if(building== NeedBuidlings)
                            {
                                return true;
                            }
                            x++;
                        }
                    }
                    return false;
                #endregion
                default:
                    return false;
            }

        }

    }
}

