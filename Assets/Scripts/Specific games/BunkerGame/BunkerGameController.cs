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
    public int gameLength = 180;
    public AnimatedButton[] destinationButtons;
    private void Awake()
    {
        Instance = this;
        carSpawner = GetComponent<SimpleSpawner>();
        SetupRoundedButtons();
    }

    private void Update()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        deliverButton.SetState(false);
        discardButton.SetState(false);
        TopBarController.instance.secondsRemaining = gameLength;
        SetGameState(bunkerGameState.FillConveyor);
        UpdateScore(0); 
        ToggleDestinationButtons(false);
        if (haveTimePressure)
        {
            InvokeRepeating("DecreaseScore", 5, 5);
        }
    }


    public void GrabButtonClicked()
    {
        if (Helpers.IsTileAcceptable(tileAnimators[0].GetComponentInChildren<TileScript>(), possibleDestinations))
        {
            ClawScript.instance.StartGrab();
            ClawScript.instance.StartDevliver();
            Invoke("AdvanceTiles", 2);
            SetGameState(bunkerGameState.DeliverItem);
            tileAnimators.RemoveAt(0);
            UpdateScore(pointsForCorrectGrab);
            deliverButton.SetClickOutcome(true, 0.5f);
            deliverButton.SetState(false);
            discardButton.SetState(false);
        }
        else
        {
            UpdateScore(-pointsForCorrectGrab);
            deliverButton.SetClickOutcome(false, 0.5f);
        }
    }
    public void DiscardButtonClicked()
    {
        if (!Helpers.IsTileAcceptable(tileAnimators[0].GetComponentInChildren<TileScript>(),possibleDestinations))
        {
            ClawScript.instance.StartGrab();
            ClawScript.instance.StartDiscard();
            Invoke("AdvanceTiles", 2);
            Invoke("SetFillState", 4);
            tileAnimators.RemoveAt(0);
            UpdateScore(pointsForCorrectGrab);
            discardButton.SetClickOutcome(true, 0.5f);
            deliverButton.SetState(false);
            discardButton.SetState(false);
        }
        else
        {
            UpdateScore(-pointsForCorrectGrab);
            discardButton.SetClickOutcome(false, 0.5f);
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
            btn.SetState(isActive);
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
                    GameObject p = Instantiate(tileAnimator, tiles);
                    SnapPoint sp = p.GetComponent<SnapPoint>();
                    TileGenerator.instance.GenerateTile(GetDestination(),sp , p.transform);
                    Snapcontroller.instance.snapPoints.Add(sp);
                    tileAnimators.Add(p.GetComponent<Animator>());
                    if(tileAnimators.Count <= 1)
                    {
                        AdvanceTiles();
                    }
                    Invoke("SetFillState", 2);
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
                Invoke("SetDriveState", 4);
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
