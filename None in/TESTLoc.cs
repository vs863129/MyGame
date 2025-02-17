using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Tables;

public class TESTLoc : MonoBehaviour
{
    [SerializeField]string Text;
    public LocalizeStringEvent localizeStringEvent;
    
     void Start()
    {
        string[] TextDouble = Text.Split(",");
        string Type = TextDouble[0];
        string Name= TextDouble[1];
        localizeStringEvent.StringReference.SetReference(Type, Name);
    }
}
