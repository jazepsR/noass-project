using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GateScript : MonoBehaviour
{
    // Start is called before the first frame update
    public AnimatedButton[] gateLeaveButtons;
    public AnimatedButton gateStartButton;

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
       /*
        gateStartButton.interactable = CarGameController.Instance.currentGameState == carGameState.GateSelect;
        startIndicator.sprite = CarGameController.Instance.currentGameState == carGameState.GateSelect ? startActive : startInactive;
        endIndicator.sprite = CarGameController.Instance.currentGameState == carGameState.TileMatching ? stopActive : stopInactive;*/
    }

}
