using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilleGenerator : MonoBehaviour
{
    public TileScript tilePrefab;
    public RectTransform tileParent;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
           TileScript tileObj= Instantiate(tilePrefab, tileParent);
            tileObj.transform.position += new Vector3(Random.Range(-100, 100), Random.Range(-100,100));
            tileObj.SetupTileScript(ResourceLoader.instance.tileTypes[Random.Range(0, ResourceLoader.instance.tileTypes.Count)]);
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
