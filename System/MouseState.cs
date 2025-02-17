using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
public class MouseState : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] string TipText;
    private GameManger GM;
    float ShowTip;
    void Start()
    {
        GM = GameObject.Find("GameManger").GetComponent<GameManger>();
    }
    private void Update()
    {
        if(IsInvoking(nameof(ActionToolTip))&&Input.GetKeyDown(KeyCode.Mouse0))
        {
            CancelInvoke(nameof(ActionToolTip));
        }
    }
    public string SetTipText
    {
        set { TipText = value; }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(TipText!="")
        {
            Invoke(nameof(ActionToolTip),0.5f);
        }
        GM.UIManger.MousePointInUI = true;
    }
    public void ActionToolTip()
    {
        GM.UIManger.ShowTip(TipText, this);
        Cursor.visible = false;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke(nameof(ActionToolTip));
        GM.UIManger.MousePointInUI = false;
        GM.UIManger.RemoveTip();
    }
}
