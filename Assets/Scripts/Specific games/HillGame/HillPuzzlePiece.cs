using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HillPuzzlePiece : MonoBehaviour
{
    public List<HillSmallPiece> smallPieces;
    public List<HillSmallPiece> availableSmallPieces;
    public SliderData[] sliderData;
    public CanvasGroup canvasGroup;
    private Animator anim;
    [HideInInspector] public HillSmallPiece activeSmallPiece;
    // Start is called before the first frame update

    private void Awake()
    {
        anim = GetComponent<Animator>();
        SetUpSmallPieces();
    }
    void Start()
    {
        Clear();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    private void SetUpSmallPieces()
    {
        availableSmallPieces = Helpers.CopyList(smallPieces);
        foreach (HillSmallPiece piece in smallPieces)
        {
            piece.gameObject.SetActive(false);
        }
    }

    public void SelectSmallPiece()
    {
        if(activeSmallPiece)
        {
            activeSmallPiece.outline.SetActive(false);
        }
        activeSmallPiece = availableSmallPieces[Random.Range(0, availableSmallPieces.Count)];
        activeSmallPiece.gameObject.SetActive(true);
        activeSmallPiece.outline.SetActive(true);
    }

    public void CompleteSmallPiece()
    {
        activeSmallPiece.fill.fillAmount = 1;
        activeSmallPiece.gameObject.SetActive(true);
        availableSmallPieces.Remove(activeSmallPiece);
        activeSmallPiece = null;
    }

    public bool IsAvailable()
    {
        return (availableSmallPieces.Count != 0);
    }
    // Update is called once per frame
    void Update()
    {

        foreach(SliderData sliderData in sliderData)
        {
            sliderData.UpdateSlider();
        }
    }

    public void FadeOut()
    {
        anim.SetBool("fadeIn",false);
    }


    public void FadeIn()
    {
        anim.SetBool("fadeIn",true);
    }

    public void Clear()
    {
        foreach(SliderData slider in sliderData)
        {
            slider.Clear();
        }
    }
}
[System.Serializable]
public class SliderData
{
    public Slider slider;
   // [HideInInspector]
    public float sliderValue = 0;
    public Image sliderBar;

    internal void UpdateSlider()
    {
        slider.value = Mathf.Lerp(slider.value, sliderValue, Time.deltaTime * HillGameController.Instance.sliderFillSpeed);
    }
    public void UpdateSliderValue( float updadeBy)
    {
        sliderValue += updadeBy;
        sliderBar.sprite = sliderValue == 1 ? HillGameController.Instance.sliderFullSprite : HillGameController.Instance.sliderEmptySprite;
    }

    public void Clear()
    {
        sliderValue = 0;
        slider.value = 0;
        slider.maxValue = 1;
        sliderBar.sprite = HillGameController.Instance.sliderEmptySprite;
    }

}
