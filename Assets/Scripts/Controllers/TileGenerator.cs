using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    public TileScript tilePrefab;
    public RectTransform tileParent;
    Snapcontroller snapcontroller;
    public static TileGenerator instance;
    [HideInInspector] public List<TileScript> tiles = new List<TileScript>();
    [HideInInspector] public List<Destination> possibleDestinations;
    public List<TileScriptable> tileTypes;
    // Start is called before the first frame update

    private void Awake()
    {
        instance = this;
        snapcontroller = GetComponent<Snapcontroller>();
    }
    void Start()
    {
        GeneratePossibleDestinationList();
    }

    private void GeneratePossibleDestinationList()
    {
        possibleDestinations = new List<Destination>();
        foreach (TileScriptable tileData in tileTypes)
        {
            foreach(Destination destination in tileData.possibleDestinations)
            {
                if(!possibleDestinations.Contains(destination))
                {
                    possibleDestinations.Add(destination);
                }
            }
        }
    }

    public void GenerateTiles(int tileCount, List<SnapPoint> possibleTargetPoints = null, Destination mainDestination = Destination.Empty)
    {
        ClearTiles();
        var list = GenerateDestinationList(possibleDestinations, tileCount, mainDestination);
        for (int i = 0; i < tileCount; i++)
        {           
            GenerateTile(list[i],possibleTargetPoints);
        }
    }


    public List<Destination> GenerateDestinationList(List<Destination> destinations, int tileCount, Destination mainDestination = Destination.Empty)
    {
        List<Destination> destinationList = new List<Destination>();
        if(mainDestination == Destination.Empty)
        {
            for(int i =0;i<tileCount;i++)
            {
                destinationList.Add(destinations[Random.Range(0, destinations.Count)]);
            }
        }
        else
        {
            int mainTileCount = (int)((float)tileCount * 0.6f);
            for (int i = 0; i < tileCount; i++)
            {
                if (i < mainTileCount)
                {
                    destinationList.Add(mainDestination);
                }
                else
                {
                    destinationList.Add(destinations[Random.Range(0, destinations.Count)]);
                }
            }
        }
        destinationList = Helpers.ShuffleList(destinationList);
        return destinationList;
    }

    public void GenerateTile(Destination destination, List<SnapPoint> possibleTargetPoints= null , Transform parent = null)
    {
        TileScript tileObj;
        if (parent == null)
        {
            tileObj = Instantiate(tilePrefab, tileParent);
        }
        else
        {
            tileObj = Instantiate(tilePrefab, parent);
        }
        tileObj.SetupTileScript(GetTileDataByDestination(destination));
        if (snapcontroller)
        {
            tileObj.GetComponent<Draggable>().dragEndedCallback = snapcontroller.OnDragEnded;
            if (possibleTargetPoints != null)
            {
                snapcontroller.TryToAssignTile(tileObj.GetComponent<Draggable>(), possibleTargetPoints);
            }
        }
        tiles.Add(tileObj);
    }

    public void GenerateTile(Destination destination, SnapPoint targetPoint, Transform parent = null)
    {
        TileScript tileObj;
        if (parent == null)
        {
            tileObj = Instantiate(tilePrefab, tileParent);
        }
        else
        {
            tileObj = Instantiate(tilePrefab, parent);
        }
        tileObj.SetupTileScript(GetTileDataByDestination(destination));
        tileObj.GetComponent<Draggable>().dragEndedCallback = snapcontroller.OnDragEnded;
        targetPoint.AssignDraggable(tileObj.GetComponent<Draggable>());
        tiles.Add(tileObj);
    }

    private TileScriptable GetTileDataByDestination(Destination destination)
    {
        tileTypes = Helpers.ShuffleList(tileTypes);
        foreach(TileScriptable tileScript in tileTypes)
        {
            foreach(Destination dest in tileScript.possibleDestinations)
            {
                if (dest == destination)
                {
                    return tileScript;
                }
            }
        }
        return null;
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
       
}
