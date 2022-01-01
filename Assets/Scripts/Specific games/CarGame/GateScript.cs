using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GateScript : MonoBehaviour
{

    public void ClickGateStartButton()
    {
        CarGameController.Instance.SetGameState(carGameState.TileMatching);
    }
      
}
