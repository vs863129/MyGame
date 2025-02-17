using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManger : MonoBehaviour
{
    [SerializeField]private GameObject UIList;
    public GameObject NowUI;
    public BuilndingManger InBuilding;
    public bool MousePointInUI;
    public bool IsTutorialUI;
    [SerializeField] GameObject NowTooltip;
    [Header("UI選單")]
    [SerializeField] Animator UI_Menu;
    [SerializeField] GameObject GameMenuPrafab;
    GameObject Game_Menu;
    [Header("提示工具")]
    MouseState NowWokring;
    [SerializeField] GameObject TooltipPrefab;
    [SerializeField] Camera Camera;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Cursor.visible = true;
            RemoveTip();
        }
        if (Input.GetKeyDown(KeyCode.Escape))  
        {
            if(NowUI&& !Game_Menu&&!IsTutorialUI)
            {
                Close(NowUI);
            }
            else
            {
                Pause();
            }
        }
    }
    public void MenuShow(string BoolName)
    {
        UI_Menu.SetBool(BoolName,!UI_Menu.GetBool(BoolName));
    }
    #region 提示工具
    public void ShowTip(string TipText,MouseState UI)
    {
        if(UI!= NowWokring)
        {
            RemoveTip();
            NowTooltip = Instantiate(TooltipPrefab, transform);
            Tooltip tooltip = NowTooltip.GetComponent<Tooltip>();
            tooltip.UiCamera = Camera;
            tooltip.ShowTooltip(TipText);
        }

    }
    public void RemoveTip()
    {
        if (NowTooltip != null)
        {
            Destroy(NowTooltip);
            Cursor.visible = true;
        }
    }
    #endregion
    void Pause()
    {
        if(!Game_Menu)
        {
            Game_Menu = Instantiate(GameMenuPrafab,transform);
            Time.timeScale = 0;
            Debug.Log("PAUSE");
        }
        else
        {
            Game_Menu.GetComponent<GameMenu>().Continue();
        }
    }
    public void Exit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
    public void UI_interface(GameObject obj) //按鈕取用
    {
        if (obj.activeSelf)
        {
            Close(obj);
        }
        else
        {
            Open(obj);
        }
    }
    private void Close(GameObject obj)
    {
        if (InBuilding)
        {
            InBuilding.Cancel(true);
            InBuilding = null;
        }
        if(obj.GetComponent<ToggleGroup>())
        {
            obj.GetComponent<ToggleGroup>().SetAllTogglesOff();
        }
        UIList.SetActive(true);
        obj.SetActive(false);
        NowUI = null;     
    }
    private void Open(GameObject obj)
    {
        if(NowUI)
        {
            NowUI.SetActive(false);
        }
        NowUI = obj;
        if(obj!= Game_Menu)
        {
            UIList.SetActive(false);
        }
        NowUI.SetActive(true);
    }
}
