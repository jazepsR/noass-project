using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : MonoBehaviour
{
    public static ResourceLoader instance;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
       // tileTypes = new List<TileScriptable>(Resources.LoadAll<TileScriptable>("ScriptableObjects"));
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
