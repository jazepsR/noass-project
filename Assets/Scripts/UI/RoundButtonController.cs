using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RoundButtonController : MonoBehaviour
{
    public Image fill;
    public Destination destination;
    // Start is called before the first frame update
    private int max;
    private int current;
    private float fillSpeed = 20;


    public void Update()
    {
        fill.fillAmount = Mathf.Lerp(fill.fillAmount,Mathf.Min(1, (float)current / (float)max), Time.deltaTime * fillSpeed);
    }

    public void Setup(int max)
    {
        this.max = max;
        fill.fillAmount = 0;
    }

    public void UpdateFill(int toAdd)
    {
        current += toAdd;
    }

    public bool IsFull()
    {
        return (float)current / (float)max >= 1f;
    }



}
