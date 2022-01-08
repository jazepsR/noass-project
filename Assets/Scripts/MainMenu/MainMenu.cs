using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public PlayerData playerData;
    public CanvasGroup splashScreen;
    public CanvasGroup profileScreen;
    public CanvasGroup levelScreen;
    public float splashScreenFadeTime = 0.4f;
    public Button submitFormButton;

    public GameObject[] screens;

    // Start is called before the first frame update
    void Start()
    {
        ValidateFormSubmission();
        DisableScreens();
        if (Var.startFromGameSelect)
        {
            GoToLevelSelect();
            screens[0].SetActive(true);
        }
        else
        {
            playerData.ClearPlayerData();
            screens[2].SetActive(true);
            FadeSplashScreen(1);
        }
    }

    public void SelectLatvian()
    {
        FadeSplashScreen(0);
        FadeProfileScreen(1,1);
    }


    public void SelectEnglish()
    {
        FadeSplashScreen(0);
        FadeProfileScreen(1, 1);
    }

     
    void DisableScreens()
    {
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }
        splashScreen.alpha = 0;
        profileScreen.alpha = 0;
        levelScreen.alpha = 0;
    }

    public void FadeSplashScreen(float target)
    {
      StartCoroutine(FadeSplash(target, splashScreen));
    }

    public void FadeProfileScreen(float target, float waitTime =0)
    {
        StartCoroutine(FadeSplash(target, profileScreen, waitTime));
    }

    public void GoToLevelSelect()
    {
        StartCoroutine(FadeSplash(0, profileScreen));
        StartCoroutine(FadeSplash(1, levelScreen,1));
    }

    private IEnumerator FadeSplash(float target, CanvasGroup toFade, float waitTime=0)
    {
      
        float t = 0;
        float start = toFade.alpha;
        if (target != 0)
        {
            toFade.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(waitTime);
        while (t <1)
        {
            toFade.alpha = Mathf.Lerp(start,target,t);
                t += Time.deltaTime / splashScreenFadeTime;
            yield return null;
        }
        toFade.alpha =target;
        toFade.gameObject.SetActive(target!=0);
    }
   

 
    // Button validation




    private void ValidateFormSubmission()
    {
        submitFormButton.interactable = AreAllFieldsFilled();
        submitFormButton.GetComponent<UIHelpers>().useFade = AreAllFieldsFilled();
    }

    private bool AreAllFieldsFilled()
    {
        if (playerData.username == "")
            return false;

        return true;
    }


    //text input
    public void SetName(string text)
    {
        playerData.firstName = text;
        SetString(text);
    }
    public void SetLastName(string text)
    {
        playerData.lastName = text;
        SetString(text);
    }
    public void SetSchool(string text)
    {
        playerData.school = text;
        SetString(text);
    }
    public void SetGrade(string text)
    {
        playerData.grade = text;
        SetString(text);
    }
    public void SetUsename(string text)
    {
        playerData.username = text;
        SetString(text);
    }

    public void SetString(string receivedText)
    {
        Debug.Log("received: " + receivedText);
        ValidateFormSubmission();
    }

    public void Quit()
    {
        Debug.LogError("quitting!");
        Application.Quit();   
    }

}
