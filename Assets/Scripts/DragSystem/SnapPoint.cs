using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool occupied;
    public Draggable content;
    public bool isTrash = false;
    public bool disposeInstantly = false;
    public bool generateInstantly = false;
    public delegate void DelegateMethod(SnapPoint snap);
    public DelegateMethod delegatedAssingMethod = null;
    // Start is called before 


    void Update()
    {
        if (content && !content.isDragged)
        {
            content.transform.position = Vector3.Lerp(content.transform.position, transform.position, Time.deltaTime * Snapcontroller.instance.followSpeed);
        }
    }

    public void AssignDraggable(Draggable draggable)
    {
        content = draggable;
        occupied = true;
        draggable.currentSnapPoint = this;
        if(isTrash && disposeInstantly)
        {
            content.BeginDestroy();
        }
        if(delegatedAssingMethod != null)
        {
            delegatedAssingMethod(this);
        }
    }


   

    public void ReleaseDraggable(bool generateOnRelease = true)
    {
        occupied = false;
        content = null;
        if(!isTrash && generateInstantly && generateOnRelease)
        {
            TileGenerator.instance.GenerateTile(TileGenerator.instance.possibleDestinations[Random.Range(0, TileGenerator.instance.possibleDestinations.Count)], this);
          //  Debug.LogError("Generating on empty tile!");
        }
    }


}
