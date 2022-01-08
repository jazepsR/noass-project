using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum hillGameState { Start, TileMatching, DestinationSelect, End }

public class HillGameController : MonoBehaviour
{
   
    public hillGameState currentGameState;
    private SimpleSpawner carSpawner;
    public static HillGameController Instance;
    public Animator bubbleAnimator;
    public int pointsToCompleteGame = 25;
    public int pointsForCorrectDestination = 10;
    public int pointsForCorrectLine = 25;
    public int pointsForCompletePiece = 50;
    int currentScore = 0;
    bool haveTimePressure = false;
    public List<Destination> possibleDestinations;
    public Destination activeDestination = Destination.Empty;
    public List<RoundButtonController> roundedButtons = new List<RoundButtonController>();
    private Destination selectedDestination;
    public Slider factoryBar;
    public Transform heatingPointParent;
    public float sliderFillSpeed = 3;
    public Sprite sliderEmptySprite;
    public Sprite sliderFullSprite;
    public float piecesToCompleteLine = 5;
    public AnimatedButton completeButton;
    bool gameComplete = false;
    private void Awake()
    {
        Instance = this;
        carSpawner = GetComponent<SimpleSpawner>();
        roundedButtons.AddRange(FindObjectsOfType<RoundButtonController>());
    }
    // Start is called before the first frame update
    void Start()
    {
        SetGameState(hillGameState.Start);
        UpdateScore(0);
        if (haveTimePressure)
        {
            InvokeRepeating("DecreaseScore", 5, 5);
        }
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
    // Update is called once per frame

    public void SetGameState(hillGameState hillGameState)
    {
        currentGameState = hillGameState;
        OnStateStart(hillGameState);
    }
      
      
    public void OnStateStart(hillGameState state)
    {
        switch (state)
        {
            case hillGameState.Start:
                activeDestination = TileGenerator.instance.possibleDestinations[Random.Range(0, TileGenerator.instance.possibleDestinations.Count)];
                TileGenerator.instance.GenerateTiles(12, Snapcontroller.instance.snapPoints, activeDestination);
                SetGameState(hillGameState.TileMatching);

                break;

            case hillGameState.TileMatching:
                completeButton.SetState(false);
                break;

            case hillGameState.DestinationSelect:
                List<Destination> incorrectDestinations = Helpers.CopyList(TileGenerator.instance.possibleDestinations);
                incorrectDestinations.Remove(activeDestination);
                if (DestinationPointCalculator.instance.GetTypeCountInDistribution(incorrectDestinations.ToArray(), Snapcontroller.instance.snapPoints) == 0 &&
                    activeDestination == selectedDestination)
                {
                    int count = DestinationPointCalculator.instance.GetTypeCountInDistribution(activeDestination, Snapcontroller.instance.snapPoints);
                    // UpdateScore(count * pointsForCorrectDestination);
                }
                else
                {
                    // UpdateScore(-pointsForCorrectDestination);
                }

                break;
            default:
                break;

        }

    }

}
