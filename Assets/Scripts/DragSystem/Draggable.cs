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
    private Animator anim;
    private bool canDragTile;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }


    private void Start()
    {
        canDragTile = Snapcontroller.instance.canDragTiles;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (isDragged && canDragTile)
        {
            transform.position = Vector3.Lerp(transform.position, Input.mousePosition, Time.deltaTime * Snapcontroller.instance.followSpeed);
        }
    }


    public void BeginDestroy()
    {
        canDragTile = false;
        anim.SetTrigger("destroy");
    }

    public void Destroy()
    {
        try
        {
            TileGenerator.instance.ClearTile(currentSnapPoint.GetComponent<TileScript>());
        }
        catch
        {
            Debug.Log("Didn't find tile script");
        }
        currentSnapPoint.ReleaseDraggable();
        Destroy(gameObject);
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

    public void OnDrag(PointerEventData eventData)
    {

    }
}
