using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public TileScript tilePrefab;
    public RectTransform tileParent;
    Snapcontroller snapcontroller;
    public static TileGenerator instance;
    private List<TileScript> tiles = new List<TileScript>();
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        snapcontroller = GetComponent<Snapcontroller>();
    }
    void Start()
    {
      //  GenerateTiles();
    }

    public void GenerateTiles()
    {
        ClearTiles();
        for (int i = 0; i < 12; i++)
        {
            TileScript tileObj = Instantiate(tilePrefab, tileParent);
            tileObj.SetupTileScript(ResourceLoader.instance.tileTypes[Random.Range(0, ResourceLoader.instance.tileTypes.Count)]);
            tileObj.GetComponent<Draggable>().dragEndedCallback = snapcontroller.OnDragEnded;
            snapcontroller.TryToAssignTile(tileObj.GetComponent<Draggable>());
            tiles.Add(tileObj);
        }
    }

    public void ClearTiles()
    {
        snapcontroller.ReleaseAllTiles();
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            Destroy(tiles[i].gameObject);
        }
        tiles = new List<TileScript>();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
