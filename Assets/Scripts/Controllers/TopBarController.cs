using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopBarController : MonoBehaviour
{
    public TMP_Text score;
    public static TopBarController instance;


    private void Awake()
    {
        instance = this;
    }

    public void UpdateScore(int score)
    {
        this.score.text = score.ToString();
       // score.
    }
}
