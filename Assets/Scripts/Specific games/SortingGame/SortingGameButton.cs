using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingGameButton : MonoBehaviour
{
    public Destination destination;
    // Start is called before the first frame update
    public void Click()
    {
        SortingGameController.Instance.DestinationSelect(destination);
    }
}
