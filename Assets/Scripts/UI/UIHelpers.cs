using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIHelpers : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public TMP_Text text;
    private Color defaultTextColor;
    public Color  targetTextColor;
    public float fadeTime = 0.2f;
    public bool useFade = false;
    void Start()
    {
        defaultTextColor = text.color;
    }

    public void ClearText()
    {
        text.text = " ";
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(text !=null && useFade)
        {
            StartCoroutine(FadeColor(targetTextColor));
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (text != null && useFade)
        {
            StartCoroutine(FadeColor(defaultTextColor));
        }
    }

    IEnumerator FadeColor(Color targetColor)
    {
        float t = 0;
        Color startColor = text.color;
        while(t<1)
        {
            text.color = Color.Lerp(startColor, targetColor, t);
            t += Time.deltaTime / fadeTime;
            yield return null;
        }
        text.color = targetColor;
    }
}
