using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    [SerializeField]
    public LevelData[] levelData;
    public Button[] gameButtons;
    private LevelData selectedGame;
    public Image mainImage;
    public TMP_Text heading;
    public Button startGameButton;
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

    public void SetupGame()
    {
        heading.text = selectedGame.levelName.ToUpper();
        mainImage.sprite = selectedGame.levelPreview;
        startGameButton.interactable = selectedGame.available;
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
    public Sprite levelPreview;
    public Sprite levelIcon;
    public bool available = true;

}