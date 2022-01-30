using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum bunkerGameState {  FillConveyor,ClawReady,  GrabItem,DiscardItem,DeliverItem, DriveToBunker,  DestinationSelect, End}
public class BunkerGameController : MonoBehaviour
{
    public bunkerGameState currentGameState;
    private SimpleSpawner carSpawner;
    public static BunkerGameController Instance;
    public CarScript currentCar;
    public GameObject tileAnimator;
    public Transform tiles;
    [SerializeField] private List<Animator> tileAnimators = new List<Animator>();
    [SerializeField] private List<Animator> additionalAnimators;
    public int pointsToCompleteGame = 15;
    public int pointsForCorrectGrab = 10;
    public int pointsForCorrectDestination= 25;
    int currentScore = 0;
    bool haveTimePressure = false;
    public List<Destination> possibleDestinations; 
    public AnimatedButton deliverButton;
    public AnimatedButton discardButton;
    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>();
    private Destination selectedDestination;
    public BunkerController bunker1;
    public BunkerController bunker2;
    public AnimatedButton[] destinationButtons;
    bool gameComplete = false;
    private bool canClick = false;
    private void Awake()
    {
        Instance = this;
        carSpawner = GetComponent<SimpleSpawner>();
        SetupRoundedButtons();
    }

    
    // Start is called before the first frame update
    void Start()
    {
        deliverButton.SetState(false);
        discardButton.SetState(false);
        SetGameState(bunkerGameState.FillConveyor);
        UpdateScore(0); 
        ToggleDestinationButtons(false);
        if (haveTimePressure)
        {
            InvokeRepeating("DecreaseScore", 5, 5);
        }
        TopBarController.instance.delegatedTimeUpMethod = OnGameComplete;
    }


    public void GrabButtonClicked()
    {
        if (tileAnimators.Count == 0 ||!canClick)
            return;
        if (!bunker1.IsEmpty() && !bunker2.IsEmpty())
        {
            tileAnimators[0].SetTrigger("error");
            return;
        }
        if (Helpers.IsTileAcceptable(tileAnimators[0].GetComponentInChildren<TileScript>(), possibleDestinations))
        {
            ClawScript.instance.StartGrab();
            ClawScript.instance.StartDevliver();
            Invoke("AdvanceTiles", 1.5f);
            SetGameState(bunkerGameState.DeliverItem);
            tileAnimators.RemoveAt(0);
           // Debug.LogError("clicked grab! REMOVED. Tile animator count: " + tileAnimators.Count);
            UpdateScore(pointsForCorrectGrab);
            deliverButton.SetClickOutcome(true, 0.5f);
            deliverButton.SetState(false);
            discardButton.SetState(false,1f);
            canClick = false;
        }
        else
        {
          //  Debug.LogError("clicked grab! Incorrect! Tile animator count: " + tileAnimators.Count);
            UpdateScore(-pointsForCorrectGrab);
            deliverButton.SetClickOutcome(false, 0.5f);
        }
    }
    public void DiscardButtonClicked()
    {
        if (tileAnimators.Count == 0 || !canClick)
            return;
        if (!Helpers.IsTileAcceptable(tileAnimators[0].GetComponentInChildren<TileScript>(),possibleDestinations))
        {
            ClawScript.instance.StartGrab();
            ClawScript.instance.StartDiscard();
            Invoke("AdvanceTiles", 1.5f);
            tileAnimators.RemoveAt(0);
          //  Debug.LogError("clicked discard! REMOVED. Tile animator count: " + tileAnimators.Count);
            UpdateScore(pointsForCorrectGrab);
            discardButton.SetClickOutcome(true, 0.5f);
            deliverButton.SetState(false,1f);
            discardButton.SetState(false);
            canClick = false;
        }
        else
        {
          //  Debug.LogError("clicked discard! Incorrect! Tile animator count: " + tileAnimators.Count);
            UpdateScore(-pointsForCorrectGrab);
            discardButton.SetClickOutcome(false, 0.5f);
        }
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

    public void SendButtonClicked(int id)
    {
        if(bunker1.IsTypeFilled(id) || bunker2.IsTypeFilled(id))
        {
            int scoreGained = 0;
            if(bunker1.IsTypeFilled(id))
            {
                roundedButtons[id].UpdateFill(pointsForCorrectGrab);
                bunker1.sliders[id].value = 0;
                scoreGained += pointsForCorrectDestination;
            }

            if(bunker2.IsTypeFilled(id))
            {
                roundedButtons[id].UpdateFill(pointsForCorrectGrab);
                bunker2.sliders[id].value = 0;
                scoreGained += pointsForCorrectDestination;
            }

            if(CheckRoundedButtons())
            {
                OnGameComplete();
            }

            UpdateScore(scoreGained);
            destinationButtons[id].SetClickOutcome(true,0.5f);
        }
        else
        {
            UpdateScore(-pointsForCorrectDestination);
            destinationButtons[id].SetClickOutcome(false, 0.5f);
        }

    }

    private void SetDriveState()
    {
        SetGameState(bunkerGameState.DriveToBunker);
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



    public void UpdateScore(int updateBy)
    {
        Debug.Log("currentScore: " + currentScore + " adding: " + updateBy);
        currentScore += updateBy;
        currentScore = Mathf.Max(0, currentScore);
        TopBarController.instance.UpdateScore(currentScore,  updateBy);
    }
    public void ToggleDestinationButtons(bool isActive)
    {
        foreach(AnimatedButton btn in destinationButtons)
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
    void SetupRoundedButtons()
    {
        foreach(RoundButtonController roundButton in roundedButtons)
        {
            roundButton.Setup(pointsToCompleteGame);
        }
    }
    // Update is called once per frame
    
    public void SetGameState(bunkerGameState bunkerGameState)
    {
        currentGameState = bunkerGameState;
        OnStateStart(bunkerGameState);
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
    private void OnStateStart(bunkerGameState state)
    {
        switch(state)
        {
           
            case bunkerGameState.FillConveyor:
                if (tileAnimators.Count <= 1)
                {
                    canClick = true;
                    GameObject p = Instantiate(tileAnimator, tiles);
                    SnapPoint sp = p.GetComponent<SnapPoint>();
                    TileGenerator.instance.GenerateTile(GetDestination(),sp , p.transform);
                    Snapcontroller.instance.snapPoints.Add(sp);
                    tileAnimators.Add(p.GetComponent<Animator>());
                 //   Debug.LogError("Added! Tile animator count: " + tileAnimators.Count);
                    if (tileAnimators.Count <= 1)
                    {
                        AdvanceTiles();
                    }
                    if (tileAnimators.Count == 2)
                    {
                        SetGameState(bunkerGameState.GrabItem);
                    }
                    else
                    {
                        Invoke("SetFillState", 1f);
                    }
                }
                else
                {
                    SetGameState(bunkerGameState.GrabItem);
                   // SetGameState(bunkerGameState.GrabItem);
                 //   ClawScript.instance.StartGrab();
                }
                break;
            case bunkerGameState.GrabItem:
                deliverButton.SetState(true);
                discardButton.SetState(true);

                break;
            case bunkerGameState.DestinationSelect:
                UpdateScore(pointsForCorrectGrab);

                break;
            case bunkerGameState.DeliverItem:
                BunerGameTractor.instance.StartMove();
                Invoke("SetDriveState", 3);
                break;
            case bunkerGameState.DriveToBunker:
                BunerGameTractor.instance.DriveToDestination(bunker1.IsEmpty());
                BunkerController activeBunker = bunker1.IsEmpty() ? bunker1 : bunker2;
                activeBunker.ToggleBunkerDoor(true);
                break;


            default:
                break;

        }

    }

    public GameObject SpawnCar()
    {
        return carSpawner.SpawnObject();
    }

    public void SetFillState()
    {
        SetGameState(bunkerGameState.FillConveyor);
    }

}
