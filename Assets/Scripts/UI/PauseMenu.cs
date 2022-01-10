using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public LevelSelect levelSelect;
    // Start is called before the first frame update
    void Start()
    {
        levelSelect.StartElementStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
