using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    public bool isDragged = false;
    public delegate void DragEndedDelegate(Draggable draggable);
    public DragEndedDelegate dragEndedCallback;
    public SnapPoint currentSnapPoint;

    // Update is called once per frame
    void Update()
    {
        if (isDragged)
        {
            transform.position = Vector3.Lerp(transform.position, Input.mousePosition, Time.deltaTime * Var.dragFollowSpeed);
        }
    }



    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragged = true;
        transform.SetAsLastSibling();
    }
    public void SetCurrentSnapPoint(SnapPoint snapPoint)
    {
        currentSnapPoint = snapPoint;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        dragEndedCallback(this);
        isDragged = false;
    }
}
