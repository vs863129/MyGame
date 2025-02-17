using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;
public static class LanguageSystem 
{

    ///<summary>
    ///localizeStringEvent �n�Q�ܧ󪺤�r��,Type_Key �d�� "Type,KeyName"
    ///</summary>
    public static void SetText(LocalizeStringEvent localizeStringEvent, string Type_Key)
    {
        try
        {
            string[] Split = Type_Key.Split(",");
            string Type = Split[0];
            string Name = Split[1];
            localizeStringEvent.StringReference.SetReference(Type, Name);
        }
        catch
        {
            Debug.LogError(Type_Key);
        }
    }

}
