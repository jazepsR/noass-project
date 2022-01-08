using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class tutorialElement : MonoBehaviour
{
    public TMP_Text objName;
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup(TileScriptable tileScriptable)
    {
        objName.text = tileScriptable.DisplayName;
        image.sprite = tileScriptable.image;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
