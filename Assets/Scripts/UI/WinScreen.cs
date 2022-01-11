using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class WinScreen : MonoBehaviour
{
    public TMP_Text score;
    public TMP_InputField email;
    public TMP_InputField playerName;
    public TMP_InputField playerLastName;
    public Toggle gdprConsentYes;
    public Toggle gdprConsentNo;
    public static WinScreen instance;
    private int scoreVal;
    public Button secondScreenButton;
    public GameObject gdprPopup;
    private bool toggledThisFrame = false;
    private bool selectionMade = false;
    public PlayerData playerData;
    private bool GDPRSeen = true;
    public Sprite gdprLV;
    public Sprite gdprEN;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
    void Start()
    {
        gdprPopup.SetActive(true);
        secondScreenButton.interactable = false;
        EnableInputFields(false);
        gdprPopup.GetComponent<Image>().sprite = Helpers.isLatvian() ? gdprLV : gdprEN;
    }

    private void EnableInputFields(bool enable)
    {
        playerLastName.interactable = enable;
        playerName.interactable = enable;
        email.interactable = enable;
    }

    public void YesToggled(bool selection)
    {
        if (!toggledThisFrame)
        {
            toggledThisFrame = true;
            if (!GDPRSeen)
            {
                gdprPopup.SetActive(true);
                GDPRSeen = true;
                gdprConsentYes.isOn = false;
            }
            else 
            {
                selectionMade = true;
                gdprConsentNo.isOn = !selection;
               // Debug.LogError("yes toggle changed to: " + selection);
                gdprPopup.SetActive(false);
            }
            ValidateFormSubmission();
        }

    }

    public void NoToggled(bool selection)
    {
        if (!toggledThisFrame)
        {
            toggledThisFrame = true;
            if (GDPRSeen)
            {
                gdprPopup.SetActive(false);
                selectionMade = true;
                gdprConsentYes.isOn = !selection;
            }
            else
            {
                selectionMade = false;
            }
          //  Debug.LogError("no toggle changed to: " + selection);
            ValidateFormSubmission();
        }

    }

    public void SetScore(int score)
    {
        this.score.text = score.ToString();
        scoreVal = score;
    }

    // Update is called once per frame
    void Update()
    {
        toggledThisFrame = false;
        if (Input.GetKeyDown(KeyCode.Y))
        {
            gdprConsentYes.isOn = true;

        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            gdprConsentNo.isOn = true;
        }
    }

    public void SendEmail()
    {
        Leaderboard.instance.Load();
        Leaderboard.instance.AddScoreToLeaderboard(TopBarController.instance.playerData.username, scoreVal);
        Leaderboard.instance.SetupLeaderboardVisualisation();
        if (gdprConsentYes.isOn)
        {
           sendEmail.instance.SendMail(playerData.email, playerData.firstName, playerData.lastName, scoreVal);
        }
    }

    public void GoToMainMenu(bool goToLevelSelect)
    {
        Var.startFromGameSelect = goToLevelSelect;
        SceneManager.LoadScene("MainMenu");
    }
    public void SetName(string text)
    {
        playerData.firstName = text;
        ValidateFormSubmission();

    }
    public void SetLastName(string text)
    {
        playerData.lastName = text;
        ValidateFormSubmission();
    }
    public void SetEmail(string text)
    {
        playerData.email = text;
        ValidateFormSubmission();
    }
    private void ValidateFormSubmission()
    {
        if(selectionMade==false)
        {
           // Debug.LogError("no selection made");
            secondScreenButton.interactable = false; 
            EnableInputFields(false);
        }

        if(gdprConsentNo.isOn)
        {
           // Debug.LogError("GDPR no selected");
            secondScreenButton.interactable = true;
            EnableInputFields(false);
        }
        else
        {
            if (playerData.firstName == "" || playerData.lastName == "" || playerData.email=="")
            {
               // Debug.LogError("GDPR yes, missing data");
                secondScreenButton.interactable = false;
                EnableInputFields(true);
            }
            else
            {
               // Debug.LogError("GDPR yes, all data");
                secondScreenButton.interactable = true;
                EnableInputFields(true);

            }
        }

    }

}
