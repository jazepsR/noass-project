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
    public List<Animator> additionalAnimators;
    private List<Animator> tileAnimators = new List<Animator>();
    public int pointsToCompleteGame = 25;
    public int pointsForCorrectDestination = 10;
    public int pointsForHazard= -25;
    public int roundedButtonIncrease = 15;
    int currentScore = 0;
    bool haveTimePressure = false;
    Destination[] possibleDestinations = new Destination[] { Destination.Kalns, Destination.Mebeles, Destination.Sadzives_Atkritumi };
    public Destination activeDestination = Destination.Empty;
    public AnimatedButton[] destinationButtons;
    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>();
    bool gameComplete = false;
    private int activeID = -1;
    private void Awake()
    {
        Instance = this;
        carSpawner = GetComponent<SimpleSpawner>();
        SetupRoundedButtons();
    }


    public void OnGameComplete(int timeleft = 0)
    {
        if (!gameComplete)
        {
            WinScreen.instance.gameObject.SetActive(true);
            WinScreen.instance.SetScore(currentScore + timeleft * Var.timeMultiplier);
            gameComplete = true;
        }
    }
    public void ToggleDestinationButtons(bool isActive)
    {
        foreach (AnimatedButton btn in destinationButtons)
        {
            if (isActive)
            {
                btn.SetState(isActive);
            }
            else
            {
                btn.SetState(isActive, 1);
            }
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
        TopBarController.instance.delegatedTimeUpMethod = OnGameComplete;
        foreach (RoundButtonController btn in roundedButtons)
        {
            btn.Setup(15);
        }
        activeDestination = Destination.Empty;
    }


    private bool CheckRoundedButtons()
    {
        bool buttonsCompleted = true;
        foreach (RoundButtonController roundButton in roundedButtons)
        {
            if (!roundButton.IsFull())
            {
                buttonsCompleted = false;
                break;
            }
        }
        return buttonsCompleted;
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
        foreach (Animator anim in additionalAnimators)
        {
            anim.SetTrigger("move");
        }
    }

    public bool DestinationSelect(Destination destination, int ID, bool setScore= true)
    {
        if (currentGameState == sortingGameState.DestinationSelect)
        {
            List<Destination> destinations = new List<Destination>(ClawScript.instance.GetCurrentDestinations());
            if (destinations.Contains(destination))
            {
                ClawScript.instance.Release(2);
                destinationButtons[ID].SetClickOutcome(true, 1f);
                roundedButtons[ID].UpdateFill(roundedButtonIncrease);
                SortingGameGate.instance.Open();
                tileAnimators.RemoveAt(0);
                AdvanceTiles();
                SetFillState();
                if (setScore)
                {
                    UpdateScore(pointsForCorrectDestination);
                }
                if (CheckRoundedButtons())
                {
                    Invoke("OnGameComplete", 3);
                }
                activeDestination = Destination.Empty;
                return true;
            }
            else
            {
                if (setScore)
                {
                    UpdateScore(-pointsForCorrectDestination);
                }
                destinationButtons[ID].SetClickOutcome(false, 1f);
                activeDestination = Destination.Empty;
                return false;
            }
        }
        else
        {
            List<Destination> destinations = new List<Destination>(ClawScript.instance.GetCurrentDestinations());
            if (destinations.Contains(destination))
            {
                destinationButtons[ID].SetClickOutcome(true, 1f);
                UpdateScore(pointsForCorrectDestination);
                ToggleDestinationButtons(false);
                if (CheckRoundedButtons())
                {
                    Invoke("OnGameComplete", 3);
                }
                activeDestination = destination;
                activeID = ID;
                return true;
            }
            else
            {
                UpdateScore(-pointsForCorrectDestination);
                destinationButtons[ID].SetClickOutcome(false, 1f);
                activeDestination = destination;
                activeID = ID;
                return false;
            }
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
                    Invoke("SetFillState", 1.25f);
                }
                else
                {
                    SetGameState(sortingGameState.GrabItem);
                    ClawScript.instance.StartGrab();
                    Invoke("SetDestinationSelect", 2);
                }
                break;
            case sortingGameState.GrabItem:
                ToggleDestinationButtons(true);


                break;
            case sortingGameState.DestinationSelect:
                if(activeDestination != Destination.Empty)
                {
                    DestinationSelect(activeDestination, activeID,false);
                }
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
