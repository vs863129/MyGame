using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
public class MainSettings : MonoBehaviour
{
    public static MainSettings Settings;
    bool ChangeLang;
    private void Awake()
    {   
        if (Settings==null)
        {
            Settings = this;
            DontDestroyOnLoad(this);
        }
        else if(Settings!=this)
        {
            Destroy(gameObject);
        }
 
    }
    public void ChangeLocle()
    {
        if (ChangeLang)
            return;
        StartCoroutine(SetLocale());
    }
    IEnumerator SetLocale()
    {
        ChangeLang = true;
        yield return LocalizationSettings.InitializationOperation;
        int i = 0;
        var Locales = LocalizationSettings.AvailableLocales.Locales;
        while (i < Locales.Count)
        {
            if (LocalizationSettings.SelectedLocale == Locales[i])
            {
                if (i == Locales.Count - 1)
                {
                    LocalizationSettings.SelectedLocale = Locales[0];
                    break;
                }
                else
                {
                    LocalizationSettings.SelectedLocale = Locales[i + 1];
                    break;
                }
            }
            i++;
        }
        ChangeLang = false;
    }
    public bool IgnoreTutorial;
    public void Ignore(bool Isgnore)
    {
        IgnoreTutorial = Isgnore;
    }
}
