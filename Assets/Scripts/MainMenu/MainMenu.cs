using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public PlayerData playerData;
    public CanvasGroup splashScreen;
    public float splashScreenFadeTime = 0.4f;
    public Button submitFormButton;

    // Start is called before the first frame update
    void Start()
    {
        playerData.ClearPlayerData();
        ValidateFormSubmission();
    }

    public void FadeSplashScreen(float target)
    {
      StartCoroutine(FadeSplash(target));
    }

    private IEnumerator FadeSplash(float target)
    {
      
        float t = 0;
        float start = splashScreen.alpha;
        while (t <1)
        {
            splashScreen.alpha = Mathf.Lerp(start,target,t);
                t += Time.deltaTime / splashScreenFadeTime;
            yield return null;
        }
        splashScreen.alpha =target;
        splashScreen.gameObject.SetActive(target!=0);
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
