using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum sortingGameState { DriveIn, FillConveyor,ClawReady,  GrabItem,DiscardItem,  DestinationSelect, End}
public class SortingGameController : MonoBehaviour
{
    public sortingGameState currentGameState;
    private SimpleSpawner carSpawner;
    public static SortingGameController Instance;
    public CarScript currentCar;
    public GameObject tileAnimator;
    public Transform tiles;
    private List<Animator> tileAnimators = new List<Animator>();
    public int pointsToCompleteGame = 25;
    public int pointsForCorrectDestination = 10;
    public int pointsForHazard= -25;
    int currentScore = 0;
    bool haveTimePressure = false;
    Destination[] possibleDestinations = new Destination[] { Destination.Kalns, Destination.Mebeles, Destination.Sadzives_Atkritumi };
    public Destination activeDestination = Destination.Empty;
    public AnimatedButton[] destinationButtons;
    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>();
    private Destination selectedDestination;
    private void Awake()
    {
        Instance = this;
        carSpawner = GetComponent<SimpleSpawner>();
        roundedButtons.AddRange(FindObjectsOfType<RoundButtonController>());
        SetupRoundedButtons();
    }

    public void ToggleDestinationButtons(bool isActive)
    {
        foreach (AnimatedButton btn in destinationButtons)
        {
            btn.SetState(isActive);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        SetGameState(sortingGameState.DriveIn);
        UpdateScore(0);
        if(haveTimePressure)
        {
            InvokeRepeating("DecreaseScore", 5, 5);
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
        TopBarController.instance.UpdateScore(currentScore,updateBy);
    }

    void SetupRoundedButtons()
    {
        foreach(RoundButtonController roundButton in roundedButtons)
        {
            roundButton.Setup(pointsToCompleteGame);
        }
    }
    // Update is called once per frame
    
    public void SetGameState(sortingGameState saortingGameState)
    {
        currentGameState = saortingGameState;
        OnStateStart(saortingGameState);
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
    public void AdvanceTiles()
    {
        foreach(Animator anim in tileAnimators)
        {
            anim.SetTrigger("move");
        }
    }

    public bool DestinationSelect(Destination destination, int ID)
    {
        List<Destination> destinations = new List<Destination>( ClawScript.instance.GetCurrentDestinations());
        if(destinations.Contains(destination))
        {
            ClawScript.instance.Release(2);
            destinationButtons[ID].SetClickOutcome(true, 1f);
            SortingGameGate.instance.Open();
            tileAnimators.RemoveAt(0);
            AdvanceTiles();
            Invoke("SetFillState", 2);
            UpdateScore(pointsForCorrectDestination);
            ToggleDestinationButtons(false);
            return true;
        }
        else
        {
            UpdateScore(-pointsForCorrectDestination);
            destinationButtons[ID].SetClickOutcome(false, 1f);
            return false;
        }


    }

    private Destination GetDestination()
    {
        return TileGenerator.instance.possibleDestinations[Random.Range(0, TileGenerator.instance.possibleDestinations.Count)];
    }
    private void OnStateStart(sortingGameState state)
    {
        switch(state)
        {
            case sortingGameState.DriveIn:               
                currentCar = SpawnCar().GetComponent<CarScript>();
                ToggleDestinationButtons(false);
                Invoke("SetFillState", 2);
                break;
            case sortingGameState.FillConveyor:
                if (tileAnimators.Count <= 3)
                {
                    GameObject p = Instantiate(tileAnimator, tiles);
                    SnapPoint sp = p.GetComponent<SnapPoint>();
                    TileGenerator.instance.GenerateTile(GetDestination(),sp , p.transform);
                    Snapcontroller.instance.snapPoints.Add(sp);
                    tileAnimators.Add(p.GetComponent<Animator>());
                    Invoke("SetFillState", 2);
                }
                else
                {
                    SetGameState(sortingGameState.GrabItem);
                    ClawScript.instance.StartGrab();
                    Invoke("SetDestinationSelect", 3);
                }
                break;
            case sortingGameState.GrabItem:


                break;
            case sortingGameState.DestinationSelect:
                ToggleDestinationButtons(true);

                break;
            default:
                break;

        }

    }
    private void SetDestinationSelect()
    {
        SetGameState(sortingGameState.DestinationSelect);
    }

    public GameObject SpawnCar()
    {
        return carSpawner.SpawnObject();
    }

    public void SetFillState()
    {
        SetGameState(sortingGameState.FillConveyor);
        if (tileAnimators.Count <= 3)
        {
            Invoke("AdvanceTiles", 1);
        }
    }

}
