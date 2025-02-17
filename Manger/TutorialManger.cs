using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManger : MonoBehaviour
{
    [SerializeField] GameManger GM;
    [SerializeField] Animator animator;
    [SerializeField] GameObject[] AllTutorial;
    [SerializeField] GameObject[] AllHint;
    [SerializeField] GameObject[] AllHideUI;
    [SerializeField] GameObject TutorialUI;
    [SerializeField] Button B_Next, B_Back;
    public Tutorial_Hint Hint;
    [Tooltip("True = Next,false = Back")]
    bool LastClick;
    public bool Ignore;
    #region 移除教學
    public void IgnoreSetting()
    {
        Debug.Log("Test");
        foreach(GameObject TutorialObj in AllTutorial)
        {
            Destroy(TutorialObj);
        }
        foreach (GameObject TutorialObj in AllHint)
        {
            Destroy(TutorialObj);
        }
        foreach (GameObject UI in AllHideUI)
        {
            UI.SetActive(true);
        }
        Destroy(TutorialUI);
        Destroy(gameObject);
    }

    #endregion
    private void Update()
    {
        if(Hint)
        {
            Hint.ActionHint();
        }
    }
    public void OpenUI()
    {
        if(Hint)
        {
            Hint.OpenUI();
        }
    }
    void CheckCollectDone()
    {
        if (Hint.CollectDone(GM))
        {
            B_Next.interactable = true;
            CancelInvoke(nameof(CheckCollectDone));
        }
    }
    public void Next()
    {
        if(!HintBranch(true))
        {
            LastClick = true;
            float NowNumber = animator.GetFloat("TutorialMain");
            animator.SetFloat("TutorialMain", NowNumber + 1);
            CheckMore();
        }
    }
    public void Back()
    {
        if (!HintBranch(false))
        {
            LastClick = false;
            float NowNumber = animator.GetFloat("TutorialMain");
            animator.SetFloat("TutorialMain", NowNumber - 1);
            CheckMore();
        }
    }
    public void Repeat(bool Yes)
    {
        if(Yes)
        {
            Hint.IsDone = false;
            return;
        }
        if (LastClick)
        {
            Next();
        }
        else
        {
            Back();
        }

    }
    bool HintBranch(bool IsNext)//判斷當前狀態
    {
        if(Hint)
        {
            if(Hint.HaveBranch(IsNext,this))
            {
                B_Next.interactable = !HintGoalCheck;
                return true;
            }
        }
        return false;
    }
    bool HintGoalCheck
    {
        get
        {
            if (Hint.HaveGoal&&Hint.CollectBranch)
            {
                if (!IsInvoking(nameof(CheckCollectDone)))
                {
                    InvokeRepeating(nameof(CheckCollectDone), 1, 1);
                  
                }
                return true;
            }
            else
            {
                if (IsInvoking(nameof(CheckCollectDone)))
                {
                    CancelInvoke(nameof(CheckCollectDone));
                }
                return false;
            }
        }
    }
    void CheckMore()//判斷下一個
    {
        //上個提示處裡
        if(Hint)
        {
            Hint.ClearHint();
        }
        //如果有額外提示
        float NowNumber = animator.GetFloat("TutorialMain");
        if (AllTutorial[(int)NowNumber].GetComponent<Tutorial_Hint>())
        {
            Hint = AllTutorial[(int)NowNumber].GetComponent<Tutorial_Hint>();
            B_Next.interactable = !HintGoalCheck;
            Hint.Action = true;
        }
        else
        {
            if(Hint)
            {
                Hint.ClearHint();
                Hint = null;
            }
            switch (NowNumber)
            {
                default:
                    B_Next.interactable = true;
                    B_Back.interactable = true;
                    break;
                case 0:
                    B_Back.interactable = false;
                    break;
                case float x when x == AllTutorial.Length - 1:
                    B_Next.interactable = false;
                    break;
            }
        }

    }
}
