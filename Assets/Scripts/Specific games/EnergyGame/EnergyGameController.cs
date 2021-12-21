using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum energyGameState {  TileMatching, DestinationSelect, End }

public class EnergyGameController : MonoBehaviour
{
   
        public energyGameState currentGameState;
        private SimpleSpawner carSpawner;
        public static EnergyGameController Instance;
        public Animator receiptAnimator;
        public int pointsToCompleteGame = 25;
        public int pointsForCorrectDestination = 10;
        public int pointsForHazard = -25;
        int currentScore = 0;
        bool haveTimePressure = false;
        public  Destination[] possibleDestinations = new Destination[] { Destination.Kalns, Destination.Mebeles, Destination.Sadzives_Atkritumi };
        public Destination activeDestination = Destination.Empty;
       public SnapPoint[] heatingPoints;
    public Button[] heatingButtons;
        public List<RoundButtonController> roundedButtons = new List<RoundButtonController>();
        private Destination selectedDestination;
    public Transform heatingPointParent;
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
            SetGameState(energyGameState.TileMatching);
            UpdateScore(0);
            if (haveTimePressure)
            {
                InvokeRepeating("DecreaseScore", 5, 5);
            }
            heatingPoints = heatingPointParent.GetComponentsInChildren<SnapPoint>();
        SetupGreenhouses();
        }

    public void Update()
    {
        if (currentGameState == energyGameState.TileMatching) ;
        CheckHeater();
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

        foreach(Button heatingButton in heatingButtons)
        {
            heatingButton.interactable = heaterReady;
        }

    }
    public void HeatGreenhouse(int ID)
    {
        roundedButtons[ID].UpdateFill(5);
        foreach(SnapPoint snapPoint in heatingPoints)
        {
            snapPoint.content.BeginDestroy();
        }
    }

    public void SetupGreenhouses()
    {
        foreach(RoundButtonController btn in roundedButtons)
        {
            btn.Setup(15);
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
                TopBarController.instance.UpdateScore(currentScore);
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

        public void EnableGateButtons()
        {


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
                case energyGameState.TileMatching:
                    activeDestination = possibleDestinations[Random.Range(0, possibleDestinations.Length)];
                    receiptAnimator.SetBool("on", true);
                    TileGenerator.instance.GenerateTiles(12, Snapcontroller.instance.snapPoints, activeDestination);
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
