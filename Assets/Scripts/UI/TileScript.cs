using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TileScript : MonoBehaviour
{
    public Image icon;
    public Image bg;
    public TileScriptable data;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupTileScript(TileScriptable data)
    {
        this.data = data;
        bg.color = data.possibleBgColors[Random.Range(0, data.possibleBgColors.Length)];
        icon.sprite = (!Var.isEasy && data.imageHard != null) ? data.imageHard : (data.imageEN != null && !Helpers.isLatvian())? data.imageEN: data.image;
    }

  
}
