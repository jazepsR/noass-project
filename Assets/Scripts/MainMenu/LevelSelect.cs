using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum TutorialStage { gaita, elementi, kopsavlikums};
public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    public LevelData[] levelData;
    public Button[] gameButtons;
    private LevelData selectedGame;
    public Image mainImage;
    public TMP_Text heading;
    public Button startGameButton;
    public Image selectionImage;
    public TutorialStage currentStage = TutorialStage.gaita;
    int currentTutorialSlide = 0;
    int currentElementGroup = 0;
    int currentElementSheet= 0;
    public Button nextSlideBtn;
    public Button prevSlideBtn;
    [Header("Elements")]
    public List<tutorialElement> tutorialElements;
    public TMP_Text elementsHeading;
    private int elementsPerPage = 8;
    public GameObject elementScreen;
    public Sprite summarySprite;
    public GameObject summaryParent;
    // Start is called before the first frame update
    void Start()
    {
        SelectGame(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(selectedGame.levelScene);
    }

    public void SetupGame(int currentSlide= 0)
    {
        currentStage = TutorialStage.gaita;
        currentTutorialSlide = currentSlide;
        heading.text = selectedGame.levelName.ToUpper();
        startGameButton.interactable = selectedGame.available;
        selectionImage.sprite = selectedGame.levelSelection;
        mainImage.gameObject.SetActive(true);
        elementScreen.SetActive(false);
        SetCurrentTutorialSlide();
        summaryParent.SetActive(false);
    }
    private void StartElementStage()
    {
        mainImage.gameObject.SetActive(false);
        summaryParent.SetActive(false);
        currentStage = TutorialStage.elementi;
        elementScreen.SetActive(true);
        currentElementGroup = 0;
        currentElementSheet = 0;
        SetupElementPage();
    }

    private void StartSummaryStage()
    {
        summaryParent.SetActive(true);
        currentStage = TutorialStage.kopsavlikums;
        mainImage.gameObject.SetActive(true);
        elementScreen.SetActive(false);
        mainImage.sprite = summarySprite;
    }

    public void SetupElementPage()
    {
        SetupElementPage(selectedGame.tileGroups[currentElementGroup], currentElementSheet);
    }

    public void SetupElementPage(TileGroup group, int sheet)
    {
        elementsHeading.text = group.groupName;
        int startingIndex = sheet * elementsPerPage;
        for(int i =0;i<elementsPerPage;i++)
        {
            int currentIndex = startingIndex + i;
            if(group.tiles.Length>currentIndex)
            {
                tutorialElements[i].gameObject.SetActive(true);
                tutorialElements[i].Setup(group.tiles[currentIndex]);
            }
            else
            {
                tutorialElements[i].gameObject.SetActive(false);
            }
        }
    }



    public void NextTutorialSlide()
    {
        switch (currentStage)
        {
            case TutorialStage.gaita:
                if (currentTutorialSlide == selectedGame.tutorialSprites.Length - 1)
                {
                    StartElementStage();
                }
                else
                {
                    currentTutorialSlide++;
                    SetCurrentTutorialSlide();
                }
                break;
            case TutorialStage.elementi:
                if(!IsLastPage())
                {
                    //not last page, turn page
                    currentElementSheet++;
                    SetupElementPage();
                   // Debug.LogError("NOT LAST PAGE");
                }else if(!IsLastGroup())
                {
                    // not last group, go to next group
                    currentElementSheet = 0;
                    currentElementGroup++;
                    SetupElementPage();
                   // Debug.LogError("LAST PAGE, NOT LAST GROUP");
                }
                else
                {
                    // last group, go to next stage
                    //Debug.LogError(" LAST PAGE, LAST GROUP");
                    StartSummaryStage();
                }
                break;

            case TutorialStage.kopsavlikums:
                StartGame();
                break;

            default:
                break;

        }
    }


    public void PrevTutorialSlide()
    {
        switch (currentStage)
        {
            case TutorialStage.gaita:
                currentTutorialSlide--;
                SetCurrentTutorialSlide();
                break;
            case TutorialStage.elementi:

              //  Debug.LogError("Back pressed on elements! group: "+ currentElementGroup + " page: " + currentElementSheet);
                if (currentElementSheet==0)
                {
                    //first page in group
                    if(currentElementGroup ==0)
                    {
                      //  Debug.LogError("FIRST GROUP, FIRST PAGE");
                        SetupGame(currentTutorialSlide);
                    }
                    else
                    {
                      //  Debug.LogError("NOT FIRST GROUP, FIRST PAGE");
                        currentElementGroup--;
                        currentElementSheet = GetSheetCount();
                        SetupElementPage();
                    }
                }
                else
                {
                   // Debug.LogError("NOT FIRST GROUP,NOT FIRST PAGE");
                    currentElementSheet--;
                    SetupElementPage();
                }

                break;

            case TutorialStage.kopsavlikums:
                StartElementStage();
                currentElementGroup = selectedGame.tileGroups.Length-1;
                currentElementSheet = GetSheetCount();
                Debug.LogError("GOING BACK FROM SUMMARY group: " + currentElementGroup+ " sheet: " + currentElementSheet);
                SetupElementPage();
                break;

            default:
                break;

        }
    }

    public bool IsLastGroup()
    {
        return selectedGame.isLastGroup(currentElementGroup);
    }

    public bool IsLastPage()
    {
        return selectedGame.isLastPage(currentElementGroup, currentElementSheet, elementsPerPage);
    }

    public int GetSheetCount()
    {
        return selectedGame.GetSheetCount(currentElementGroup, elementsPerPage);
    }

    public void SetCurrentTutorialSlide()
    {
        ValidateButtons();
        mainImage.sprite = selectedGame.tutorialSprites[currentTutorialSlide];
    }
    public void ValidateButtons()
    {
      //  prevSlideBtn.interactable = currentTutorialSlide > 0;
     //   nextSlideBtn.interactable = currentTutorialSlide < selectedGame.tutorialSprites.Length-1;
    }

    public void SelectGame(int id)
    {
        selectedGame = levelData[id];
        SetupGame();
    }

}

[System.Serializable]
public class LevelData
{
    public string levelName;
    public string levelScene;
    public Sprite levelIcon;
    public Sprite levelSelection;
    public Sprite[] tutorialSprites;
    public Sprite[] tutorialSpritesEn;
    public bool available = true;
    public TileGroup[] tileGroups;

    public bool isLastPage(int group, int page,int elementCount)
    {
        return (tileGroups[group].tiles.Length-1) / elementCount == page;
    }
    public bool isLastGroup(int group)
    {
       // Debug.LogError("lenght: " + tileGroups.Length + " received: " + group + " result: " + (tileGroups.Length == group + 1));
        return tileGroups.Length == group + 1;
    }
    public int GetSheetCount(int group, int elementCount)
    {
        int c= (tileGroups[group].tiles.Length-1) / elementCount;
        return c;
    }
}

[System.Serializable]
public class TileGroup
{
    public string groupName;
    public TileScriptable[] tiles;
}