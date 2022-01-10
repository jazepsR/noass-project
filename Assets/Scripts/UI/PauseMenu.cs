using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public LevelSelect levelSelect;
    public GameObject bg;
    // Start is called before the first frame update
    void Start()
    {
        levelSelect.StartElementStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
