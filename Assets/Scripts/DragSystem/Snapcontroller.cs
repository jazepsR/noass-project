using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snapcontroller : MonoBehaviour
{
    public SnapPoint snapPointPrefab;
    public SnapPointSpawnData[] spawnPoints;
    [HideInInspector]  public List<SnapPoint> snapPoints;
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
        SpawnAllSnapPoints();
    }
    private void SpawnAllSnapPoints()
    {
        snapPoints = new List<SnapPoint>();
        foreach(SnapPointSpawnData data in spawnPoints)
        {
            SpawnSnapPoints(data, snapPoints);
        }
    }


    private void SpawnSnapPoints(SnapPointSpawnData data, List<SnapPoint> pointlist = null)
    {
        for(int i=0; i<data.pointCount;i++)
        {
            SnapPoint snap = Instantiate(snapPointPrefab, data.spawnPoint);
            snap.isTrash = data.isTrash;
            if (pointlist != null)
            {
                pointlist.Add(snap);
            }
        }

    }

    public void TryToAssignTile(Draggable draggable, List<SnapPoint> possiblePoints)
    {
        foreach(SnapPoint snapPoint in possiblePoints)
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

[System.Serializable]
public class SnapPointSpawnData
{
    public Transform spawnPoint;
    public int pointCount;
    public bool isTrash;

}
