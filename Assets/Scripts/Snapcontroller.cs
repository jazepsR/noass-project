using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapcontroller : MonoBehaviour
{
    public SnapPoint snapPointPrefab;
    public Transform regularSnapPointParent;
    public int regularPointCount;
    public Transform trashSnapPointParent;
    public int trashPointCount;
    private List<SnapPoint> snapPoints;
    public float snapDistance = 50f;
    public static Snapcontroller instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        Initialize();
    }
    private void Initialize()
    {
        snapPoints = new List<SnapPoint>();
        for (int i = 0; i < regularPointCount; i++)
        {
           SnapPoint snap= Instantiate(snapPointPrefab, regularSnapPointParent);
            snap.isTrash = false;
            snapPoints.Add(snap);
        }
        for(int i=0; i<trashPointCount;i++)
        {
            SnapPoint snap = Instantiate(snapPointPrefab, trashSnapPointParent);
            snap.isTrash = true;
            snapPoints.Add(snap);

        }
    }

    public void TryToAssignTile(Draggable draggable)
    {
        foreach(SnapPoint snapPoint in snapPoints)
        {
            if(!snapPoint.occupied && !snapPoint.isTrash)
            {
                snapPoint.AssignDraggable(draggable);
                break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void CalculateTypeDistribution()
    {
        Dictionary<Destination, int> currentDistribution = new Dictionary<Destination, int>();
        foreach (SnapPoint snapPoint in snapPoints)
        {
            if(!snapPoint.isTrash && snapPoint.occupied)
            {
               Destination[] destinations=  snapPoint.content.GetComponent<TileScript>().data.possibleDestinations;
                currentDistribution = AddToDestinationList(currentDistribution, destinations);
            }
        }
        Debug.LogError("HALT");
    }

    public Dictionary<Destination, int> AddToDestinationList(Dictionary<Destination, int> destinationList, Destination[] destinationsToAdd)
    {
        if (destinationsToAdd.Length > 0)
        {
            foreach (Destination dest in destinationsToAdd)
            {
                if (destinationList.ContainsKey(dest))
                {
                    destinationList[dest] = destinationList[dest] + 1;
                }
                else
                {
                    destinationList.Add(dest, 1);
                }
            }
        }
        return destinationList;
    }


    public void OnDragEnded(Draggable draggable)
    {
        float closestDistance = float.PositiveInfinity;
        SnapPoint closestSnapPoint = null;
        foreach(SnapPoint snapPoint in snapPoints)
        {
            float dist = Vector3.Distance(snapPoint.transform.position, Input.mousePosition);
            if (dist<closestDistance)
            {
                closestDistance = dist;
                closestSnapPoint = snapPoint;
            }
        }

        if(closestDistance< snapDistance && closestSnapPoint!= null)
        {
            if (!closestSnapPoint.occupied)
            {
                //regular release
                ReleaseTile(draggable);
                closestSnapPoint.AssignDraggable(draggable);
            }
            else
            {
                //swap
                Draggable tempDrag = closestSnapPoint.content;
                SnapPoint tempSnap = draggable.currentSnapPoint;
                closestSnapPoint.AssignDraggable(draggable);
                tempSnap.AssignDraggable(tempDrag);
            }
        }

    }

    public void ReleaseTile(Draggable draggable)
    {

        foreach (SnapPoint snapPoint in snapPoints)
        {
            if (snapPoint.content != null && snapPoint.content == draggable)
            {
                snapPoint.ReleaseDraggable();
            }
        }
    }

    public void ReleaseAllTiles()
    {

        foreach (SnapPoint snapPoint in snapPoints)
        {
            snapPoint.ReleaseDraggable();
        }
    }
}
