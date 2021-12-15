using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public bool occupied;
    public Draggable content;
    public bool isTrash = false;
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
    }


    public void ReleaseDraggable()
    {
        occupied = false;
        content = null;

    }

    public Draggable SwapDraggable(Draggable draggable)
    {
        Draggable tempDraggable = content;
        content = draggable;
        return tempDraggable;
    }
}
