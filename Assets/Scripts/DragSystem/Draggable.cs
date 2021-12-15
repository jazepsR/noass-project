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


    private void Start()
    {
        
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (isDragged && Snapcontroller.instance.canDragTiles)
        {
            transform.position = Vector3.Lerp(transform.position, Input.mousePosition, Time.deltaTime * Snapcontroller.instance.followSpeed);
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
