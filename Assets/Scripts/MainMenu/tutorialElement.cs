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
        string text = Helpers.isLatvian() ? tileScriptable.DisplayName : tileScriptable.DisplayNameEng;
        try
        {
            objName.text = text[0].ToString().ToUpper() + text.Remove(0, 1);
        }
        catch
        {
            objName.text = text;
        }
        image.sprite = (!Var.isEasy && tileScriptable.imageHard != null) ? tileScriptable.imageHard : (tileScriptable.imageEN != null && !Helpers.isLatvian()) ? tileScriptable.imageEN : tileScriptable.image;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
