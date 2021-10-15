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
        icon.sprite = data.image;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
