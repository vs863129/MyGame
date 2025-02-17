using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;
public class Tooltip : MonoBehaviour
{
   public Camera UiCamera;
   [SerializeField] TextMeshProUGUI TextUI;
   [SerializeField] LocalizeStringEvent TooltipText;
   [SerializeField] RectTransform BackGroundScale;
    Vector2 CheckQuadrant
    {
        get
        {
            Vector2 mousePos = Input.mousePosition;
            float pivotX = mousePos.x / Screen.width;
            float pivotY = mousePos.y / Screen.height;
            return new Vector2(pivotX, pivotY);
        }

    }
    void Update()
    {
        SetPoint();
    }
    public void ShowTooltip(string ToolText)
    {
        SetPoint();
        BackGroundScale.pivot = CheckQuadrant;
        LanguageSystem.SetText(TooltipText, ToolText);
        gameObject.SetActive(true);
        Vector2 SetScale = new Vector2(TextUI.preferredWidth + 16f, TextUI.preferredHeight + 8f);
        BackGroundScale.sizeDelta = SetScale;
    }
    public void SetPoint()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), Input.mousePosition, UiCamera, out localPoint);
        transform.localPosition = localPoint;

    }
}

