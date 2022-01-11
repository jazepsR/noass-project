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
    public float followSpeed = 30;
    public bool canDragTiles= true;
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
            snap.disposeInstantly = data.disposeInstantly;
            snap.generateInstantly = data.generateInstantly;
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
            if(!snapPoint.occupied && !snapPoint.isTrash && snapPoint.enabled && snapPoint.CanAssign(draggable))
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
        if (canDragTiles)
        {
            float closestDistance = float.PositiveInfinity;
            SnapPoint closestSnapPoint = null;
            foreach (SnapPoint snapPoint in snapPoints)
            {
                float dist = Vector3.Distance(snapPoint.transform.position, Input.mousePosition);
                if (dist < closestDistance && snapPoint.enabled && snapPoint.CanAssign(draggable))
                {
                    closestDistance = dist;
                    closestSnapPoint = snapPoint;
                }
            }
            if (closestDistance < snapDistance && closestSnapPoint != null)
            {
                SetSnapPointForDraggable(draggable, closestSnapPoint);
            }
        }

    }

    public void SetSnapPointForDraggable(Draggable draggable, SnapPoint snapPoint)
    {       
        if (!snapPoint.occupied)
        {
            //regular release
            ReleaseTile(draggable);
            snapPoint.AssignDraggable(draggable);
        }
        else
        {
            //swap
            Draggable tempDrag = snapPoint.content;
            SnapPoint tempSnap = draggable.currentSnapPoint;
            snapPoint.AssignDraggable(draggable);
            tempSnap.AssignDraggable(tempDrag);
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
            snapPoint.ReleaseDraggable(false);
        }
    }
}

[System.Serializable]
public class SnapPointSpawnData
{
    public Transform spawnPoint;
    public int pointCount;
    public bool isTrash;
    public bool disposeInstantly;
    public bool generateInstantly;

}
