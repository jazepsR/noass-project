using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDriveIn()
    {
        if (CarGameController.Instance)
        {
            if (CarGameController.Instance.currentGameState == carGameState.DriveIn)
            {
                CarGameController.Instance.SetGameState(carGameState.GateSelect);
                anim.SetTrigger("toGate");
            }
        }
    }
    public void StartDriveAway()
    {
        anim.SetTrigger("leave");
    }
    public void OnDriveAway()
    {
        CarGameController.Instance.SetGameState(carGameState.DriveIn);
        Destroy(gameObject);
    }
}
