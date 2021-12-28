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
    public int pointsToCompleteGame = 25;
    public int pointsForCorrectDestination = 10;
    public int pointsForHazard= -25;
    int currentScore = 0;
    bool haveTimePressure = true;
    Destination[] possibleDestinations = new Destination[] { Destination.Kalns, Destination.Mebeles, Destination.Sadzives_Atkritumi };
    public Destination activeDestination = Destination.Empty;
    public Button deliverButton;
    public Button discardButton;
    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>();
    private Destination selectedDestination;
    public BunkerController bunker1;
    public BunkerController bunker2;
    private void Awake()
    {
        Instance = this;
        carSpawner = GetComponent<SimpleSpawner>();
        roundedButtons.AddRange(FindObjectsOfType<RoundButtonController>());
        SetupRoundedButtons();
    }

    private void Update()
    {
        discardButton.interactable = currentGameState == bunkerGameState.GrabItem;
        deliverButton.interactable = currentGameState == bunkerGameState.GrabItem;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetGameState(bunkerGameState.FillConveyor);
       // Debug.LogError("AS");
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

    public void GrabButtonClicked()
    {
        ClawScript.instance.StartGrab();
        ClawScript.instance.StartDevliver();
        Invoke("AdvanceTiles", 2);
        SetGameState(bunkerGameState.DeliverItem);
        tileAnimators.RemoveAt(0);
    }

    private void SetDriveState()
    {
        SetGameState(bunkerGameState.DriveToBunker);
    }

    public void DiscardButtonClicked()
    {
        ClawScript.instance.StartGrab();
        ClawScript.instance.StartDiscard();
        Invoke("AdvanceTiles", 2);
        Invoke("SetFillState", 4);
        tileAnimators.RemoveAt(0);
    }

    public void UpdateScore(int updateBy)
    {
        Debug.Log("currentScore: " + currentScore + " adding: " + updateBy);
        currentScore += updateBy;
        currentScore = Mathf.Max(0, currentScore);
       // TopBarController.instance.UpdateScore(currentScore);
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

    public bool DestinationSelect(Destination destination)
    {
        List<Destination> destinations = new List<Destination>( ClawScript.instance.GetCurrentDestinations());
        if(destinations.Contains(destination))
        {
            ClawScript.instance.Release(2);
            Debug.LogError("successful release!");
            SortingGameGate.instance.Open();
            tileAnimators.RemoveAt(0);
            AdvanceTiles();
            Invoke("SetFillState", 2);
            return true;
        }
        else
        {
            Debug.LogError("bad release!");
            return false;
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


                break;
            case bunkerGameState.DestinationSelect:
                UpdateScore(pointsForCorrectDestination);

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
