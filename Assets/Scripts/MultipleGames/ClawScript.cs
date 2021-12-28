using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawScript : MonoBehaviour
{
    private Animator anim;
    public static ClawScript instance;
    public TileScript grabbedTile = null;
    public float grabDistance = 25f;
    public Transform grabPoint;
    [HideInInspector] private SnapPoint snapPoint;
    [SerializeField] private SnapPoint snapPointTarget;
    bool swap = false;
    float defaultSnapSpeed;
        // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        defaultSnapSpeed = Snapcontroller.instance.followSpeed;
        anim = GetComponent<Animator>();
        snapPoint = GetComponentInChildren<SnapPoint>();
    }

    public void StartGrab()
    {
        anim.ResetTrigger("release");
        anim.SetTrigger("grab");
        swap = false;

    }
    public void StartDiscard()
    {
        anim.SetTrigger("discard");
        swap = false;

    }

    public void StartDevliver()
    {
        anim.SetTrigger("deliver");
        swap = true;

    }

    public Destination[] GetCurrentDestinations()
    {
        if (snapPoint.content == null)
            return new Destination[] { Destination.Empty };
        return snapPoint.content.GetComponent<TileScript>().data.possibleDestinations;

    }

    public void Grab()
    {
        Debug.Log("trying to grab");
        if(Snapcontroller.instance)
        {           
            float closestDistance = float.PositiveInfinity;
            TileScript closestTile = null;
            foreach (TileScript tile in TileGenerator.instance.tiles)
            {
                float dist = Vector3.Distance(tile.transform.position, grabPoint.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestTile = tile;
                }
            }
            if (closestDistance < grabDistance && closestTile != null)
            {
                Snapcontroller.instance.SetSnapPointForDraggable(closestTile.GetComponent<Draggable>(), snapPoint);
                Snapcontroller.instance.followSpeed = 1000;
            }
        }
        else
        {
            Debug.LogError("no snap controller in scene!");
        }
    }


    public void Release(float timeToLive)
    {
        anim.SetTrigger("release");
        Snapcontroller.instance.followSpeed = defaultSnapSpeed;
        if (!swap)
        {
            snapPoint.content.gameObject.AddComponent<Rigidbody2D>();
            snapPoint.content.enabled = false;
            TileGenerator.instance.tiles.Remove(snapPoint.content.GetComponent<TileScript>());
            Destroy(snapPoint.content.gameObject, timeToLive);
        }
        else
        {
            if(snapPointTarget!= null)
            {
                Snapcontroller.instance.SetSnapPointForDraggable(snapPoint.content, snapPointTarget);
            }
        }
        snapPoint.ReleaseDraggable();
    }


}
