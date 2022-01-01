using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum carGameState { DriveIn, GateSelect, TileMatching, DestinationSelected, End}
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
    public List<Destination> skirosanaDestinations;
    public List<Destination> glabasanaDestinations;
    public List<Destination> bioDestinations;
    public Destination activeDestination = Destination.Empty;
    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>(); 
    public AnimatedButton[] destinationButtons;
    public AnimatedButton gateStartButton;
    private Destination selectedDestination;
    public Animator gateAnimator;
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

    private List<Destination> GetDestinationByID(int ID)
    {
        switch(ID)
        {
            case 0:
                return skirosanaDestinations;
            case 1:
                return glabasanaDestinations;
            case 2:
                return bioDestinations;
            default:
                return null;
        }
            
    }

    public void SetDestination(int destination)
    {
        var destList = GetDestinationByID(destination);
        int incorrectTiles = 0;
        int count = 0;
      foreach (SnapPoint snap in Snapcontroller.instance.snapPoints)
       {
            if (snap.occupied)
            {
                count++;
                TileScript ts = snap.content.GetComponent<TileScript>();
                if (!Helpers.IsTileAcceptable(ts, destList))
                {
                    snap.content.GetComponent<Animator>().SetTrigger("error");
                    incorrectTiles++;
                }
            }
       }


        if (incorrectTiles==0)
        {
            receiptAnimator.SetBool("on", false);
            UpdateRoundedButtons(count, selectedDestination);
            UpdateScore(count * pointsForCorrectDestination);
            OnStateStart(carGameState.DestinationSelected);
            destinationButtons[destination].SetClickOutcome(true, 1);
        }
        else
        {
            destinationButtons[destination].SetClickOutcome(false, 1);
            UpdateScore(pointsForHazard*incorrectTiles);
        }


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
                gateAnimator.SetBool("open", false);
                gateStartButton.SetState(true); 
                break;

            case carGameState.TileMatching:
                gateStartButton.SetClickOutcome(true,0.5f);
                gateStartButton.SetState(false);
                ToggleDestinationButtons(true);
                var list = GetDestinationByID(Random.Range(0, 3));
                activeDestination = list[Random.Range(0, list.Count)];// possibleDestinations[0,Random.Range(0, possibleDestinations.Length)];
                receiptAnimator.SetBool("on", true);
                TileGenerator.instance.GenerateTiles(12, Snapcontroller.instance.snapPoints, activeDestination);
                break;
            case carGameState.DestinationSelected:
                gateAnimator.SetBool("open", true);
                Invoke("DriveAway", 1);
                break;
            default:
                break;
        }

    }

    private void DriveAway()
    {
        currentCar.StartDriveAway();
    }

    public GameObject SpawnCar()
    {
        return carSpawner.SpawnObject();
    }

}
