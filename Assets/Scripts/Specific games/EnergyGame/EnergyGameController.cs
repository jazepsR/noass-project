using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum energyGameState { Start, TileMatching, DestinationSelect, GeneratorHeating, GeneratorHeated, End }

public class EnergyGameController : MonoBehaviour
{
   
    public energyGameState currentGameState;
    private SimpleSpawner carSpawner;
    public static EnergyGameController Instance;
    public Animator receiptAnimator;
    public Animator bubbleAnimator;
    public int pointsToCompleteGame = 25;
    public int pointsForCorrectDestination = 10;
    public int pointsForCorrectDiscard = 25;
    public int pointsForCorrectBurn = 25;
    int currentScore = 0;
    bool haveTimePressure = false;
    public List<Destination> possibleDestinations;
    public Destination activeDestination = Destination.Empty;
    public SnapPoint[] heatingPoints;
    public AnimatedButton generatorButton;
    public AnimatedButton[] heatingButtons;
    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>();
    private Destination selectedDestination;
    public Slider factoryBar;
    public Sprite fullBar;
    public Sprite emptyBar;
    public Image factoryBarFill;
    public Transform heatingPointParent;
    public GameObject[] fillIcons;
    private float generatorTime = 1.5f;
    bool prevHeat = false;
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
    SetGameState(energyGameState.Start);
    UpdateScore(0);
    if (haveTimePressure)
    {
        InvokeRepeating("DecreaseScore", 5, 5);
    }
    heatingPoints = heatingPointParent.GetComponentsInChildren<SnapPoint>();
    SetupGreenhouses();
        TopBarController.instance.delegatedTimeUpMethod = OnGameComplete;
    }

    public void Update()
    {
       // if (currentGameState == energyGameState.TileMatching)
            CheckHeater();
        UpdateFillIcons();
       

    }


    private void UpdateFillIcons()
    {
        for(int i=0;i<fillIcons.Length;i++)
        {
            fillIcons[i].SetActive(roundedButtons[i].IsFull());
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

    private void CheckHeater()
    {
        bool heaterReady = true;
        for(int i=0; i<heatingPoints.Length;i++)
        {
            if(!heatingPoints[i].content)
            {
                heaterReady = false;
                break;
            }
        }
        if (prevHeat != heaterReady)
        {
            generatorButton.SetState(heaterReady);
        }
        prevHeat = heaterReady;

    }

    public void HeatGenerator()
    {
        //Check if can be burned
        bool canBurn =true;
        int scoreChange = 0;
        foreach (SnapPoint snapPoint in heatingPoints)
        {
            if(!Helpers.IsTileAcceptable(snapPoint.content.GetComponent<TileScript>(),possibleDestinations))
            {
                canBurn = false;
                scoreChange -=pointsForCorrectBurn;
                snapPoint.content.GetComponent<Animator>().SetTrigger("error");
            }

        }

        if (canBurn)
        {
            foreach (SnapPoint snapPoint in heatingPoints)
            {
                snapPoint.content.BeginDestroy();
            }
            StartCoroutine(HeatGeneratorCoroutine(1));
            scoreChange += pointsForCorrectBurn;
            bubbleAnimator.SetTrigger("move");
        }
        UpdateScore(scoreChange);
        generatorButton.SetClickOutcome(canBurn, 0.5f);
    }


    private IEnumerator HeatGeneratorCoroutine(float target)
    {
        generatorButton.SetState(false);
        SetGameState(energyGameState.GeneratorHeating);
        factoryBarFill.sprite = emptyBar;
        while(Mathf.Abs(target-factoryBar.value)>0.02f)
        {
            factoryBar.value = Mathf.Lerp(factoryBar.value, target, Time.deltaTime / generatorTime);
            yield return null;
        }
        factoryBarFill.sprite = fullBar;
        factoryBar.value = target;
        SetGameState(energyGameState.GeneratorHeated);
    }
    public void HeatGreenhouse(int ID)
    {
        roundedButtons[ID].UpdateFill(5);
        heatingButtons[ID].SetClickOutcome(true, 0.5f);
        SetGameState(energyGameState.TileMatching);
        factoryBar.value = 0;
        UpdateScore(pointsForCorrectDestination);
        if(CheckRoundedButtons())
        {
            OnGameComplete(TopBarController.instance.secondsRemaining);
        }
    }

    private bool CheckRoundedButtons()
    {
        bool buttonsCompleted = true;
        foreach(RoundButtonController roundButton in roundedButtons)
        {
            if(!roundButton.IsFull())
            {
                buttonsCompleted = false;
                break;
            }
        }
        return buttonsCompleted;
    }

    public void SetupGreenhouses()
    {
        foreach(RoundButtonController btn in roundedButtons)
        {
            btn.Setup(25);
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
            if (TopBarController.instance)
            {
                TopBarController.instance.UpdateScore(currentScore, updateBy);
            }
        }

        void SetupRoundedButtons()
        {
            foreach (RoundButtonController roundButton in roundedButtons)
            {
                roundButton.Setup(pointsToCompleteGame);
            }
        }
        // Update is called once per frame

        public void SetGameState(energyGameState carGameState)
        {
            currentGameState = carGameState;
            OnStateStart(carGameState);
        }

        public void SetDestination(Destination destination)
        {
            selectedDestination = destination;
            OnStateStart(energyGameState.DestinationSelect);
        }

        public void UpdateRoundedButtons(int increaseBy, Destination destination)
        {
            foreach (RoundButtonController roundButton in roundedButtons)
            {
                if (roundButton.destination == destination)
                {
                    roundButton.UpdateFill(increaseBy);
                    if (roundButton.IsFull())
                    {
                        Debug.LogError("WON BY FILLING " + destination.ToString().ToUpper());
                    }
                }
            }
        }
        public void OnStateStart(energyGameState state)
        {
            switch (state)
            {
            case energyGameState.Start:
                activeDestination = TileGenerator.instance.possibleDestinations[Random.Range(0, TileGenerator.instance.possibleDestinations.Count)];
                receiptAnimator.SetBool("on", true);
                TileGenerator.instance.GenerateTiles(12, Snapcontroller.instance.snapPoints, activeDestination);
                SetGameState(energyGameState.TileMatching);
                break;

            case energyGameState.TileMatching:
                ToggleDestinationButtons(false);
                break;
            case energyGameState.GeneratorHeating:

                break;
            case energyGameState.GeneratorHeated:
                ToggleDestinationButtons(true);
                break;

            case energyGameState.DestinationSelect:
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
                    //  currentCar.StartDriveAway();
                }
                else
                {
                    UpdateScore(pointsForCorrectDiscard);
                }

                break;
            default:
                break;

            }

        }
    public void ToggleDestinationButtons(bool isActive)
    {
        foreach (AnimatedButton btn in heatingButtons)
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
    public GameObject SpawnCar()
        {
            return carSpawner.SpawnObject();
        }

    }
