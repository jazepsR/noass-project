using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunerGameTractor : MonoBehaviour
{
    private Animator anim;
    public SnapPoint snapPoint;
    public static BunerGameTractor instance;

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Deliver1()
    {
        DeliverToDestination(true);
    }

    public void Deliver2()
    {
        DeliverToDestination(false);
    }

    public void SetFill()
    {
        BunkerGameController.Instance.SetGameState(bunkerGameState.FillConveyor);
    }

    public void DeliverToDestination(bool isFirst)
    {
        SnapPoint activeSnap = isFirst ? BunkerGameController.Instance.bunker1.GetAvailableSnapPoint()
            : BunkerGameController.Instance.bunker2.GetAvailableSnapPoint();
        snapPoint.content.transform.parent = activeSnap.transform;
        Snapcontroller.instance.SetSnapPointForDraggable(snapPoint.content, activeSnap);
        snapPoint.ReleaseDraggable();

    }
    public void DriveToDestination(bool isFirst)
    {
        if (isFirst)
        {
            anim.SetTrigger("depo1");
        }
        else 
        {
            anim.SetTrigger("depo2");
        }
        snapPoint.content.transform.parent = snapPoint.transform;
    }

    public void CloseGates()
    {
        if (!BunkerGameController.Instance.bunker1.IsEmpty())
        {
           BunkerGameController.Instance.bunker1.ToggleBunkerDoor(false);
        }


        if (!BunkerGameController.Instance.bunker2.IsEmpty())
        {
            BunkerGameController.Instance.bunker2.ToggleBunkerDoor(false);
        }
        BunkerGameController.Instance.SetGameState(bunkerGameState.FillConveyor);
    }

    public void StartMove()
    {
        anim.SetTrigger("receive");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
