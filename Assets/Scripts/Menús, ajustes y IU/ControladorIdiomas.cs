using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ControladorIdiomas : MonoBehaviour
{
    private bool _active = false;

    void Start()
    {
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        ChangeLocale(ID);
    }

    public void ChangeLocale(int localID)
    {
        if (_active)
            return;

        StartCoroutine(SetLocale(localID));
    }

    private IEnumerator SetLocale(int localID)
    {
        _active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localID];
        PlayerPrefs.SetInt("LocaleKey", localID);
        _active = false;
    }
}