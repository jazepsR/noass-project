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
    public int pointsToCompleteGame = 25;
    public int pointsForCorrectDestination = 10;
    public int pointsForHazard= -25;
    int currentScore = 0;
    bool haveTimePressure = false;
    public Destination[] possibleDestinations = new Destination[] { Destination.Kalns, Destination.Mebeles, Destination.Sadzives_Atkritumi };
    public Destination activeDestination = Destination.Empty;

    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>(); 
    public AnimatedButton[] destinationButtons;
    public AnimatedButton gateStartButton;
    private Destination selectedDestination;
    private void Awake()
    {
        Instance = this;
        carSpawner = GetComponent<SimpleSpawner>();
        roundedButtons.AddRange(FindObjectsOfType<RoundButtonController>());
        SetupRoundedButtons();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetGameState(carGameState.DriveIn);
        UpdateScore(0);
        if(haveTimePressure)
        {
            InvokeRepeating("DecreaseScore", 5, 5);
        }
    }
    public void ToggleDestinationButtons(bool isActive)
    {
        foreach (AnimatedButton btn in destinationButtons)
        {
            btn.SetState(isActive);
        }
    }
    private void DecreaseScore()
    {
        UpdateScore(-5);
    }

    public void UpdateScore(int updateBy)
    {
        Debug.Log("currentScore: " + currentScore + " adding: " + updateBy);
        currentScore += updateBy;
        currentScore = Mathf.Max(0, currentScore);
        TopBarController.instance.UpdateScore(currentScore, updateBy);
    }

    void SetupRoundedButtons()
    {
        foreach(RoundButtonController roundButton in roundedButtons)
        {
            roundButton.Setup(pointsToCompleteGame);
        }
    }
    // Update is called once per frame
    
    public void SetGameState(carGameState carGameState)
    {
        currentGameState = carGameState;
        OnStateStart(carGameState);
    }



    public void SetDestination(Destination destination)
    {
        selectedDestination = destination;
        OnStateStart(carGameState.DestinationSelect);
    }

    public void UpdateRoundedButtons(int increaseBy, Destination destination)
    {
        foreach(RoundButtonController roundButton in roundedButtons)
        {
            if(roundButton.destination == destination)
            {
                roundButton.UpdateFill(increaseBy);
                if(roundButton.IsFull())
                {
                    Debug.LogError("WON BY FILLING " + destination.ToString().ToUpper());
                }
            }
        }
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
                ToggleDestinationButtons(false);
                break;
            case carGameState.GateSelect:
                SpawnCar();
                gateStartButton.SetState(true); 
                break;

            case carGameState.TileMatching:
                gateStartButton.SetState(false);
                activeDestination = possibleDestinations[Random.Range(0, possibleDestinations.Length)];
                receiptAnimator.SetBool("on", true);
                TileGenerator.instance.GenerateTiles(12, Snapcontroller.instance.snapPoints, activeDestination);
                break;
            case carGameState.DestinationSelect:
                ToggleDestinationButtons(true);
                List<Destination> incorrectDestinations = Helpers.CopyList(TileGenerator.instance.possibleDestinations);
                incorrectDestinations.Remove(activeDestination);
                if (DestinationPointCalculator.instance.GetTypeCountInDistribution(incorrectDestinations.ToArray(), Snapcontroller.instance.snapPoints) == 0 &&
                    activeDestination == selectedDestination)
                {
                    receiptAnimator.SetBool("on", false);
                    int count = DestinationPointCalculator.instance.GetTypeCountInDistribution(activeDestination, Snapcontroller.instance.snapPoints);
                    // int hazardCount = DestinationPointCalculator.instance.GetTypeCountInDistribution(Destination.Hazard, Snapcontroller.instance.snapPoints);
                    UpdateRoundedButtons(count, selectedDestination);
                    UpdateScore(count * pointsForCorrectDestination);// + hazardCount * pointsForHazard);
                    currentCar.StartDriveAway();
                }
                else
                {
                    UpdateScore(pointsForHazard);
                }

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
