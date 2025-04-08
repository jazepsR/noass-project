using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum TutorialStage { gaita, elementi, kopsavilkums};
public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    public LevelDataScriptable[] levelData;
    public Button[] gameButtons;
    private LevelDataScriptable selectedGame;
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
    public int elementsPerPage = 8;
    public GameObject elementScreen;
    public Sprite summarySprite;
    public Sprite summarySpriteEN;
    public GameObject summaryParent;
    public Color activeColor;
    public Color passiveColor;
    public TMP_Text gaitaText;
    public TMP_Text elementiText;
    public TMP_Text kopsavilkumsText;
    public Toggle easyToggle;
    public Toggle difficultToggle;
    public Animator easyToggleAnim;
    public Animator difficultToggleAnim;
    bool toggledDiffifulty = false;
    public bool OnPauseScreen = false;
    // Start is called before the first frame update
    void Start()
    {
        SelectGame(0);
    }

    // Update is called once per frame
    void Update()
    {
        toggledDiffifulty = false;
    }


    public void SetupDifficultyToggles()
    {
        string diffState = Var.isEasy ? "toggleOff" : "toggleOn";
        string easyState = Var.isEasy ? "toggleOn" : "toggleOff";

        easyToggleAnim.Play(easyState);
        difficultToggleAnim.Play(diffState);
        easyToggle.isOn = Var.isEasy;
        difficultToggle.isOn = !Var.isEasy;
        easyToggleAnim.SetBool("isOn", Var.isEasy);
        difficultToggleAnim.SetBool("isOn", !Var.isEasy);
    }

    public void IsDifficultToggle(bool isDifficult)
    {
        if(!toggledDiffifulty)
        {
            toggledDiffifulty = true;
            Var.isEasy = !isDifficult;
            easyToggle.isOn = !isDifficult;
            difficultToggle.isOn = isDifficult;
            difficultToggleAnim.SetBool("isOn", isDifficult);
            easyToggleAnim.SetBool("isOn", !isDifficult);
            currentElementGroup = 0;
            currentElementSheet = 0;
            StartElementStage();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(selectedGame.levelScene);
    }

    public void SetupGame(int currentSlide= 0)
    {
        if (!OnPauseScreen)
        {
            currentStage = TutorialStage.gaita;
            currentTutorialSlide = currentSlide;
           // startGameButton.interactable = selectedGame.available;
            selectionImage.sprite = selectedGame.levelSelection;
            mainImage.gameObject.SetActive(true);
            elementScreen.SetActive(false);
            SetCurrentTutorialSlide();
            summaryParent.SetActive(false);
            if (currentSlide == 0)
            {
                prevSlideBtn.interactable = false;
                startGameButton.gameObject.SetActive(true);
            }
            nextSlideBtn.interactable = true;
            RefreshTexts();
        }
        heading.text = Helpers.isLatvian()? selectedGame.levelName.ToUpper() :selectedGame.levelNameEn.ToUpper();
    }
    public void StartElementStage()
    {
        currentStage = TutorialStage.elementi;
        if (!OnPauseScreen)
        {
            mainImage.gameObject.SetActive(false);
            summaryParent.SetActive(false);
            elementScreen.SetActive(true);
            currentElementGroup = 0;
            currentElementSheet = 0;
            RefreshTexts();
            SetupDifficultyToggles();
        }
        heading.text = Helpers.isLatvian() ? selectedGame.levelName.ToUpper() : selectedGame.levelNameEn.ToUpper();
        selectedGame.SetupElementList();
        SetupElementPage();
    }



    private void StartSummaryStage()
    {
        summaryParent.SetActive(true);
        currentStage = TutorialStage.kopsavilkums;
        mainImage.gameObject.SetActive(true);
        elementScreen.SetActive(false);
        mainImage.sprite = Helpers.isLatvian() ? summarySprite : summarySpriteEN;
        RefreshTexts();
    }

    public void SetupElementPage()
    {
        SetupElementPage(selectedGame.tileGroups[currentElementGroup], currentElementSheet);
    }


    private void RefreshTexts()
    {
        gaitaText.color = currentStage == TutorialStage.gaita ? activeColor : passiveColor;
        elementiText.color = currentStage == TutorialStage.elementi ? activeColor : passiveColor;
        kopsavilkumsText.color = currentStage == TutorialStage.kopsavilkums ? activeColor : passiveColor;
    }
    public void SetupElementPage(TileGroup group, int sheet)
    {
        elementsHeading.text = Helpers.isLatvian()? group.groupName : group.groupNameEn;
        int startingIndex = sheet * elementsPerPage;
        for(int i =0;i<elementsPerPage;i++)
        {
            int currentIndex = startingIndex + i;
            if(group.currentTiles.Count>currentIndex)
            {
                tutorialElements[i].gameObject.SetActive(true);
                tutorialElements[i].Setup(group.currentTiles[currentIndex]);
            }
            else
            {
                tutorialElements[i].gameObject.SetActive(false);
            }
        }
    }



    public void NextTutorialSlide()
    {
        prevSlideBtn.interactable = true;
        startGameButton.gameObject.SetActive(false);
        switch (currentStage)
        {
            case TutorialStage.gaita:
                Sprite[] activeSprites = Helpers.isLatvian()? selectedGame.tutorialSprites :selectedGame.tutorialSpritesEn;
                if (currentTutorialSlide ==activeSprites.Length - 1)
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
                    if (!OnPauseScreen)
                    {
                        StartSummaryStage();
                    }
                    nextSlideBtn.interactable = false;
                }
                break;

            case TutorialStage.kopsavilkums:
                break;

            default:
                break;

        }
    }


    public void PrevTutorialSlide()
    {
        nextSlideBtn.interactable = true;
        startGameButton.gameObject.SetActive(false);
        switch (currentStage)
        {
            case TutorialStage.gaita:
                if (currentTutorialSlide > 0)
                {
                    currentTutorialSlide--;
                    SetCurrentTutorialSlide();
                }
                if(currentTutorialSlide == 0)
                {
                    prevSlideBtn.interactable = false;
                    startGameButton.gameObject.SetActive(true);
                }
                break;
            case TutorialStage.elementi:

              //  Debug.LogError("Back pressed on elements! group: "+ currentElementGroup + " page: " + currentElementSheet);
                if (currentElementSheet==0)
                {
                    //first page in group
                    if(currentElementGroup ==0)
                    {
                        //  Debug.LogError("FIRST GROUP, FIRST PAGE");
                        if (!OnPauseScreen)
                        {
                            SetupGame(currentTutorialSlide);
                        }
                        else
                        {
                            prevSlideBtn.interactable = false;
                        }
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

            case TutorialStage.kopsavilkums:
                StartElementStage();
                currentElementGroup = selectedGame.tileGroups.Length-1;
                currentElementSheet = GetSheetCount();
               // Debug.LogError("GOING BACK FROM SUMMARY group: " + currentElementGroup+ " sheet: " + currentElementSheet);
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
        Sprite[] activeSprites = Helpers.isLatvian() ? selectedGame.tutorialSprites : selectedGame.tutorialSpritesEn;
        mainImage.sprite = activeSprites[currentTutorialSlide];
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
    public string levelNameEn;
    public string levelScene;
    public Sprite levelIcon;
    public Sprite levelSelection;
    public Sprite[] tutorialSprites;
    public Sprite[] tutorialSpritesEn;
    public bool available = true;
    public TileGroup[] tileGroups;

    public bool isLastPage(int group, int page,int elementCount)
    {
        return (tileGroups[group].currentTiles.Count-1) / elementCount == page;
    }
    public bool isLastGroup(int group)
    {
       // Debug.LogError("lenght: " + tileGroups.Length + " received: " + group + " result: " + (tileGroups.Length == group + 1));
        return tileGroups.Length == group + 1;
    }
    public int GetSheetCount(int group, int elementCount)
    {
        int c= (tileGroups[group].currentTiles.Count-1) / elementCount;
        return c;
    }

    public void SetupElementList()
    {
        foreach (TileGroup group in tileGroups)
        {
            group.currentTiles = new List<TileScriptable>();
            foreach (TileScriptable tile in group.tiles)
            {
                if (tile.isHard)
                {
                    if(!Var.isEasy)
                    {
                        group.currentTiles.Add(tile);
                    }
                }
                else
                {
                    group.currentTiles.Add(tile);
                }
            }
        }
    }
}

[System.Serializable]
public class TileGroup
{
    public string groupName;
    public string groupNameEn;
    public TileScriptable[] tiles;
   [HideInInspector] public List<TileScriptable> currentTiles;
}