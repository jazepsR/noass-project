using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BunkerController : MonoBehaviour
{
    public Slider[] sliders;
    public Animator doorAnim;
    public SnapPoint[] snapPoints;
    private float processMaterialTime = 15f;
    private bool processing = false;
    // Start is called before the first frame update
    void Start()
    {
        ResetSliders();
        ToggleBunkerDoor(true);
    }

    public SnapPoint GetAvailableSnapPoint()
    {
        foreach(SnapPoint snap in snapPoints)
        {
            if(!snap.occupied)
            {
                //Debug.LogError("FOUND IT");
                return snap;
            }
        }
        return null;
    }

    public void ResetSliders()
    {
        foreach(Slider slider in sliders)
        {
            slider.value = 0;
            slider.maxValue = 1;
        }

    }

    public void ToggleBunkerDoor(bool open)
    {
        //Debug.LogError("changing bunker " + gameObject.name + " door to " + open);
        if(open)
        {
            doorAnim.ResetTrigger("close");
            doorAnim.SetTrigger("open");
        }
        else
        {
            doorAnim.ResetTrigger("open");
            doorAnim.SetTrigger("close");
            if(!IsEmpty() && !processing)
            {
                ProcessMaterials();
            }
        }
    }


    private void ProcessMaterials()
    {
        List<int> indexes = new List<int> { 0, 1, 2 };
        int barToComplete = Random.Range(0, 3);
        Slider targetSlider = sliders[barToComplete];
        StartCoroutine(FillBar(targetSlider, 1));
        indexes.RemoveAt(barToComplete);
        foreach(int index in indexes)
        {
            float targetVal = Mathf.Min(1,sliders[index].value + Random.Range(0.3f,0.8f));
            StartCoroutine(FillBar(sliders[index], targetVal));
        }
    }

    public bool IsTypeFilled(int id)
    {
        return sliders[id].value > 0.97f;
    }

    private IEnumerator FillBar(Slider bar, float targetVal)
    {
        float t = 0;
        float startVal = bar.value;
        processing = true;
        while(t<=1)
        {
            bar.value = Mathf.Lerp(startVal, targetVal, t);
            t += Time.deltaTime / processMaterialTime;
            yield return null;
        }
        bar.value = targetVal;
        ClearMaterials();
    }

    private void ClearMaterials()
    {
        if (IsEmpty())
            return;
        processing = false;
        ToggleBunkerDoor(true);
        foreach(SnapPoint snap in snapPoints)
        {
            TileGenerator.instance.tiles.Remove(snap.content.GetComponent<TileScript>());
            snap.ReleaseDraggable();
            Destroy(snap.transform.GetChild(0).gameObject);
        }
        BunkerGameController.Instance.ToggleDestinationButtons(true);
    }


    public bool IsEmpty()
    {
        bool isEmpty = false;
        foreach(SnapPoint point in snapPoints)
        {
            if (point.occupied)
            {
            }
            else
            {
                isEmpty = true;
                break;
            }
        }
        return isEmpty;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
