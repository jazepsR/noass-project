using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum carGameState { DriveIn, GateSelect, TileMatching, DestinationSelect, End}
public class CarGameController : MonoBehaviour
{
    public carGameState currentGameState;
    private SimpleSpawner carSpawner;
    public static CarGameController Instance;
    public Animator receiptAnimator;
    public CarScript currentCar;

    private void Awake()
    {
        Instance = this;
        carSpawner = GetComponent<SimpleSpawner>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetGameState(carGameState.DriveIn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGameState(carGameState carGameState)
    {
        currentGameState = carGameState;
        OnStateStart(carGameState);
    }

    public void EnableGateButtons()
    {


    }
    public void OnStateStart(carGameState state)
    {
        switch(state)
        {
            case carGameState.DriveIn:
                if (currentCar == null)
                {
                    currentCar = SpawnCar().GetComponent<CarScript>();
                }
                else
                {
                    currentCar = FindObjectOfType<CarScript>();
                    currentCar.OnDriveIn();
                }
                break;
            case carGameState.GateSelect:
                SpawnCar();
                EnableGateButtons();
                break;
            case carGameState.TileMatching:
                receiptAnimator.SetBool("on", true);
                TileGenerator.instance.GenerateTiles();
                break;
            case carGameState.DestinationSelect:
                receiptAnimator.SetBool("on", false);
                currentCar.StartDriveAway();
                break;
            default:
                break;

        }

    }

    public GameObject SpawnCar()
    {
        return carSpawner.SpawnObject();
    }

}
