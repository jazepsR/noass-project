using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadSelectButtonController : MonoBehaviour
{
    public Button grabButton;
    public Button discradButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        grabButton.interactable = RoadGameController.Instance.currentGameState == roadGameState.ClawReady;
        discradButton.interactable = RoadGameController.Instance.currentGameState == roadGameState.ClawReady;
    }

    public void GrabButtonClicked()
    {
        RoadGameController.Instance.SetGameState(roadGameState.GrabItem);
    }

    public void DiscardButtonClicked()
    {
        RoadGameController.Instance.SetGameState(roadGameState.DiscardItem);
    }
}
