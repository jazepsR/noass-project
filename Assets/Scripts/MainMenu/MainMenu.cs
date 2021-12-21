using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public PlayerData playerData;
    public CanvasGroup splashScreen;
    public CanvasGroup profileScreen;
    public float splashScreenFadeTime = 0.4f;
    public Button submitFormButton;

    public GameObject[] screens;

    // Start is called before the first frame update
    void Start()
    {
        playerData.ClearPlayerData();
        ValidateFormSubmission();
        EnableScreens();
    }

    void EnableScreens()
    {
        foreach (GameObject screen in screens)
        {
            screen.SetActive(true);
        }
    }

    public void FadeSplashScreen(float target)
    {
      StartCoroutine(FadeSplash(target, splashScreen));
    }

    public void FadeProfileScreen(float target)
    {
        StartCoroutine(FadeSplash(target, profileScreen));
    }

    private IEnumerator FadeSplash(float target, CanvasGroup toFade)
    {
      
        float t = 0;
        float start = toFade.alpha;
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
        if (playerData.firstName == "")
            return false;
        if (playerData.lastName == "")
            return false;
        if (playerData.school == "")
            return false;
        if (playerData.grade == "")
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

    
}
