using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TopBarController : MonoBehaviour
{
    public TMP_Text score;
    public TMP_Text playerName;
    public TMP_Text time;
    public TMP_Text gameName;
    public Color textColor;
    public static TopBarController instance;
    public PlayerData playerData;
    public string gameNameString;
    public string gameNameStringEN;
    public Image nameUnderlay;
    public Image helpBtnImage;
    public Image scoreUnderlay;
    public TMP_Text pointsGainText;
    public TMP_Text pointsLoseText;
    [SerializeField] private Animator anim;
    public int secondsRemaining = 10;
    public delegate void DelegateMethod(int timeLeft);
    public DelegateMethod delegatedTimeUpMethod = null;
    public GameObject pauseBG;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerName.color = textColor;
        gameName.color = textColor;
        playerName.text = playerData.username.ToUpper();// playerData.firstName.ToUpper()+  " " + playerData.lastName[0] + ".";
        nameUnderlay.color = textColor;
        scoreUnderlay.color = textColor;
        helpBtnImage.color = textColor;
        SetupHeading();
        UpdateTime();
    }

    public void SetupHeading()
    {
        gameName.text = Helpers.isLatvian() ? gameNameString : gameNameStringEN;
    }
    private void UpdateTime()
    {
        if (secondsRemaining == 0)
        {
            if(delegatedTimeUpMethod!= null)
            {
                delegatedTimeUpMethod(0);
            }
            return;
        }
        if (!pauseBG.activeSelf)
        {
            secondsRemaining--;
        }
        time.text = secondsRemaining / 60 + ":" + (secondsRemaining % 60).ToString("D2");
        Invoke("UpdateTime",1);
    }

    public void UpdateScore(int score, int updateBy)
    {
        if (score > 2500)
        {
            score = 2500;
        }
        this.score.text = score.ToString();
        if (updateBy == 0)
            return;
       // score.
       if(updateBy > 0)
        {
            anim.SetTrigger("gain");
            pointsGainText.text = "+" + updateBy.ToString();
        }
        else
        {
            anim.SetTrigger("lose");
            pointsLoseText.text = updateBy.ToString();
        }
    }
}
