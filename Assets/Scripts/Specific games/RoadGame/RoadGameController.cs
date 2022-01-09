using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum roadGameState { DriveIn, FillConveyor, ClawReady, GrabItem, DiscardItem, DestinationSelect, Crunch, End}
public class RoadGameController : MonoBehaviour
{
    public roadGameState currentGameState;
    private SimpleSpawner carSpawner;
    public static RoadGameController Instance;
    public CarScript currentCar;
    public GameObject tileAnimator;
    public Transform tiles;
    public List<Animator> tileAnimators = new List<Animator>();
    public int pointsToCompleteGame = 25;
    public int pointsForCorrectDestination = 10;
    public int pointsForCorrectGrab= -25;
    int currentScore = 0;
    bool haveTimePressure = false;
    public List<Destination> possibleDestinations;
    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>();
    public AnimatedButton[] destinationButtons;
    public AnimatedButton grabButton;
    public AnimatedButton discardButton;
    public Animator receiveTruckAnim;
    public List<Animator> additionalAnimators;
    bool gameComplete = false;
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
        SetGameState(roadGameState.DriveIn);
        UpdateScore(0);
        if(haveTimePressure)
        {
            InvokeRepeating("DecreaseScore", 5, 5);
        }
        ToggleDestinationButtons(false);
        grabButton.SetState(false);
        discardButton.SetState(false);
        TopBarController.instance.delegatedTimeUpMethod = OnGameComplete;
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
    private void DecreaseScore()
    {
        UpdateScore(-5);
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
    public void UpdateScore(int updateBy)
    {
        Debug.Log("currentScore: " + currentScore + " adding: " + updateBy);
        currentScore += updateBy;
        currentScore = Mathf.Max(0, currentScore);
        TopBarController.instance.UpdateScore(currentScore,updateBy);
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
    public void GrabButtonClicked()
    {
        if (Helpers.IsTileAcceptable(tileAnimators[0].GetComponentInChildren<TileScript>(), possibleDestinations))
        {
            SetGameState(roadGameState.GrabItem);
            UpdateScore(pointsForCorrectGrab);
            grabButton.SetClickOutcome(true, 0.5f);
            grabButton.SetState(false);
            discardButton.SetState(false,1f);
        }
        else
        {
            UpdateScore(-pointsForCorrectGrab);
            grabButton.SetClickOutcome(false, 0.5f);
        }
    }
  

    public void DiscardButtonClicked()
    {
        if (!Helpers.IsTileAcceptable(tileAnimators[0].GetComponentInChildren<TileScript>(), possibleDestinations))
        {
            SetGameState(roadGameState.DiscardItem);
            UpdateScore(pointsForCorrectGrab);
            discardButton.SetClickOutcome(true, 0.5f);
            grabButton.SetState(false,1f);
            discardButton.SetState(false);
        }
        else
        {
            UpdateScore(-pointsForCorrectGrab);
            discardButton.SetClickOutcome(false, 0.5f);
        }
    }

    void SetupRoundedButtons()
    {
        foreach(RoundButtonController roundButton in roundedButtons)
        {
            roundButton.Setup(pointsToCompleteGame);
        }
    }
    // Update is called once per frame
    
    public void SetGameState(roadGameState roadGameState)
    {
        currentGameState = roadGameState;
        OnStateStart(roadGameState);
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
     

    private Destination GetDestination()
    {
        return TileGenerator.instance.possibleDestinations[Random.Range(0, TileGenerator.instance.possibleDestinations.Count)];
    }
    private void OnStateStart(roadGameState state)
    {
        switch(state)
        {
            case roadGameState.DriveIn:               
                currentCar = SpawnCar().GetComponent<CarScript>();
                Invoke("SetFillState", 2);
                break;
            case roadGameState.FillConveyor:
                if (tileAnimators.Count <= 3)
                {
                    GameObject p = Instantiate(tileAnimator, tiles);
                    SnapPoint sp = p.GetComponent<SnapPoint>();
                    TileGenerator.instance.GenerateTile(GetDestination(),sp , p.transform);
                    Snapcontroller.instance.snapPoints.Add(sp);
                    tileAnimators.Add(p.GetComponent<Animator>());
                    Invoke("SetFillState", 2);
                    receiveTruckAnim.SetTrigger("leave");
                }
                else
                {
                    SetGameState(roadGameState.ClawReady);
                }
                break;
            case roadGameState.ClawReady:
                grabButton.SetState(true);
                discardButton.SetState(true);
                break;
            case roadGameState.DiscardItem:
                ClawScript.instance.StartDiscard();
                tileAnimators.RemoveAt(0);
                Invoke("SetFillState", 2.5f);
                Invoke("AdvanceTiles", 2);
                break;
            case roadGameState.GrabItem:
                ClawScript.instance.StartGrab();
                Invoke("SetDestinationState", 2.5f);
                receiveTruckAnim.ResetTrigger("leave");
                receiveTruckAnim.SetTrigger("toGate");
                break;
            case roadGameState.DestinationSelect:
                ToggleDestinationButtons(true);
                ///UpdateScore(pointsForCorrectDestination);
                break;
            case roadGameState.Crunch:
                ToggleDestinationButtons(false);
                UpdateScore(pointsForCorrectDestination);
                RoadCrusher.instance.StartCrush();
                ClawScript.instance.Release(1);
                tileAnimators.RemoveAt(0);
                Invoke("AdvanceTiles", 1);
                Invoke("SetFillState", 1.5f);
                break;
            default:
                break;

        }

    }

    private void SetDestinationState()
    {
        SetGameState(roadGameState.DestinationSelect);
    }

    public GameObject SpawnCar()
    {
        return carSpawner.SpawnObject();
    }

    public void SetFillState()
    {
        SetGameState(roadGameState.FillConveyor);
        if (tileAnimators.Count <= 3)
        {
            Invoke("AdvanceTiles", 1);
        }
    }

}
