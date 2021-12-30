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
    public Image nameUnderlay;
    public Image scoreUnderlay;
    public TMP_Text pointsGainText;
    public TMP_Text pointsLoseText;
    [SerializeField] private Animator anim;
    public int secondsRemaining = 10;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerName.color = textColor;
        gameName.color = textColor;
        playerName.text = playerData.firstName.ToUpper()+  " " + playerData.lastName[0] + ".";
        gameName.text = gameNameString;
        nameUnderlay.color = textColor;
        scoreUnderlay.color = textColor;
        InvokeRepeating("UpdateTime", 0.1f,1f);
    }
    private void UpdateTime()
    {
        secondsRemaining--;
        if (secondsRemaining == 0)
            return;
        time.text = secondsRemaining / 60 + ":" + (secondsRemaining % 60).ToString("D2");

    }

    public void UpdateScore(int score, int updateBy)
    {
        
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
