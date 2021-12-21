using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadGameLower : MonoBehaviour
{
    [SerializeField] private Slider roadSlider;
    [SerializeField] private Slider smallSlider;
    [SerializeField] private Slider medSlider;
    [SerializeField] private Slider bigSlider;
    [SerializeField] private Button smallButton;
    [SerializeField] private Button midButton;
    [SerializeField] private Button bigButton;
  private float maxVal = 5;
    public float roadVal = 0;
    private float smallVal = 0;
    private float medVal = 0;
    private float bigVal = 0;
    public float sliderFillSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //update sliders
        smallSlider.value = Mathf.Lerp(smallSlider.value, smallVal / maxVal, Time.deltaTime * sliderFillSpeed);
        medSlider.value = Mathf.Lerp(medSlider.value, medVal/maxVal, Time.deltaTime * sliderFillSpeed);
        bigSlider.value = Mathf.Lerp(bigSlider.value, bigVal / maxVal, Time.deltaTime * sliderFillSpeed);
        float minval = Mathf.Min(Mathf.Min(bigVal,medVal),smallVal)/maxVal;
        roadSlider.value = Mathf.Lerp(roadSlider.value, minval, Time.deltaTime * sliderFillSpeed);
        //toggle buttons

        smallButton.interactable = RoadGameController.Instance.currentGameState == roadGameState.DestinationSelect;
        midButton.interactable = RoadGameController.Instance.currentGameState == roadGameState.DestinationSelect;
        bigButton.interactable = RoadGameController.Instance.currentGameState == roadGameState.DestinationSelect;
    }

    public void AddToSmall()
    {
        smallVal = Mathf.Min(maxVal, ++smallVal);
        CompleteFill();
    }

    public void AddToMed()
    {
        medVal = Mathf.Min(maxVal, ++medVal);
        CompleteFill();
    }

    public void AddToBig()
    {
        bigVal = Mathf.Min(maxVal, ++bigVal);
        CompleteFill();
    }

    private void CompleteFill()
    {
       // ClawScript.instance.Release(1f);
        RoadGameController.Instance.SetGameState(roadGameState.Crunch);
    }

}
