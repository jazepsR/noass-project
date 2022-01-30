using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HillPieceController : MonoBehaviour
{
    public Transform targetParent;
    public List<SnapPoint> targetPoints;
    int activeBar = 0;
    public List<HillPuzzlePiece> pieces;
    HillPuzzlePiece activePiece;
    [SerializeField]
    public LayerData[] layerDatas;
    // Start is called before the first frame update
    void Start()
    {
        targetPoints.AddRange(targetParent.GetComponentsInChildren<SnapPoint>());
        foreach (SnapPoint point in targetPoints)
        {
            point.delegatedAssingMethod = DelegatedMethod;
            point.delegatedCheckMethod = DelegatedCheckMethod;
        }
        SetActivePiece(GetAvailablePiece());
        ActivateTargetPoints();
    }

    public void ActivateTargetPoints()
    {
        for(int i =0;i<targetPoints.Count;i++)
        {
            bool shouldBeActive = activeBar ==i;
            targetPoints[i].enabled = shouldBeActive;
        }
    }
    public bool DelegatedCheckMethod(SnapPoint point, TileScript ts)
    {
        if(Time.timeSinceLevelLoad <1f)
        {
            return true;
        }    
        int pointIndex = targetPoints.FindIndex(d => d == point);
        bool correctType = Helpers.ListContainsTile(ts, layerDatas[activeBar].possibleDestinations);
        if (pointIndex == activeBar && correctType)
        {
            return true;
        }
        else
        {
            HillGameController.Instance.UpdateScore(-HillGameController.Instance.pointsForCorrectDestination);
            ts.GetComponent<Animator>().SetTrigger("error");
            return false;
        }

    }
    public void DelegatedMethod(SnapPoint point)
    {
        int pointIndex = targetPoints.FindIndex(d => d == point);
        bool correctType = Helpers.ListContainsTile(point.content.GetComponent<TileScript>(), layerDatas[activeBar].possibleDestinations);
        if (pointIndex == activeBar && correctType) 
        {
            point.content.BeginDestroy();
            activePiece.sliderData[activeBar].UpdateSliderValue(1f / HillGameController.Instance.piecesToCompleteLine);
            int points = HillGameController.Instance.pointsForCorrectDestination;
            if (activePiece.sliderData[activeBar].sliderValue==1) 
            {
                activeBar++;
                ActivateTargetPoints();
                //activePiece.activeSmallPiece.fill.fillAmount = (float)activeBar / 5f;
                points += HillGameController.Instance.pointsForCorrectLine;
                if(activeBar==5)
                {
                    HillGameController.Instance.completeButton.SetState(true);
                }
            }
            HillGameController.Instance.UpdateScore(points);
        }
        else
        {
            HillGameController.Instance.UpdateScore(-HillGameController.Instance.pointsForCorrectDestination);
            point.content.GetComponent<Animator>().SetTrigger("error");
        }
    }

    public void SetNextPiece()
    {
        HillGameController.Instance.completeButton.SetClickOutcome(true,1f);
        HillGameController.Instance.completeButton.SetState(false);
        activePiece.CompleteSmallPiece();
        SetActivePiece(GetAvailablePiece(), true);
        HillGameController.Instance.UpdateScore(HillGameController.Instance.pointsForCompletePiece);
        activeBar = 0;
        ActivateTargetPoints();
    }

    public int GetAvailablePiece()
    {
       var tempList=  pieces.ShuffleList<HillPuzzlePiece>();
        foreach(HillPuzzlePiece piece in tempList)
        {
            if(piece.IsAvailable())
            {
                return pieces.IndexOf(piece);
            }
        }
        return -1;
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }

    void SetActivePiece(int id,bool animated =false)
    {
        if (id == -1)
        {
            HillGameController.Instance.OnGameComplete(TopBarController.instance.secondsRemaining);
        }
        else
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                if (animated&& pieces[i] == activePiece)
                {
                    pieces[i].FadeOut();
                }
                else
                {
                    pieces[i].gameObject.SetActive(false);
                }
                
            }
            activePiece = pieces[id];
            activePiece.gameObject.SetActive(true);
            activePiece.SelectSmallPiece();
            activePiece.FadeIn();
            activePiece.Clear();
        }
    }
}
[System.Serializable]
public class LayerData
{
  public List<Destination> possibleDestinations;
}
