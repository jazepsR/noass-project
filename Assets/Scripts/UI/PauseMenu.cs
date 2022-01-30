using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public LevelSelect levelSelect;
    // Start is called before the first frame update
    void Start()
    {
        levelSelect.StartElementStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleLanguage()
    {
        if(Helpers.isLatvian())
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        }
        else
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
        TopBarController.instance.SetupHeading();
        levelSelect.StartElementStage();
        WinScreen.instance.SetupWinScreen();
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
