using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GateScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Button[] gateLeaveButtons;
    public Button gateStartButton;
    public Image startIndicator;
    public Image endIndicator;
    public Sprite startActive;
    public Sprite startInactive;
    public Sprite stopActive;
    public Sprite stopInactive;

    public void ClickGateStartButton()
    {
        CarGameController.Instance.SetGameState(carGameState.TileMatching);
    }

    public void ClickGateEndButton(int destination)
    {
        CarGameController.Instance.SetDestination((Destination)destination);
       // CarGameController.Instance.SetGameState(carGameState.DestinationSelect);
    }

    private void Update()
    {
        foreach(Button button in gateLeaveButtons)
        {
            button.interactable = (CarGameController.Instance.currentGameState == carGameState.TileMatching);
        }
        gateStartButton.interactable = CarGameController.Instance.currentGameState == carGameState.GateSelect;
        startIndicator.sprite = CarGameController.Instance.currentGameState == carGameState.GateSelect ? startActive : startInactive;
        endIndicator.sprite = CarGameController.Instance.currentGameState == carGameState.TileMatching ? stopActive : stopInactive;
    }

}
