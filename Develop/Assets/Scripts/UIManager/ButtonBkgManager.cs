using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBkgManager : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IDragHandler, IEndDragHandler
{
     public DOTweenAnimation doTweenAnimation;
    public bool isInner;

    public void OnDrag(PointerEventData eventData)
    {
        if (BackPackManager.mIntance.CountRightFiger > 1) return;
        //Debug.Log("Drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (doTweenAnimation != null)
        {
            doTweenAnimation.DOPlayBackwards();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (doTweenAnimation != null)
        {
            doTweenAnimation.DOPlayBackwards();
        }
        BackPackManager.mIntance.OnButtonCloseBkgClick(isInner);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //先触发OnPointDown才会触发Update
        BackPackManager.mIntance.CalcRightTouchFiger();
        if (BackPackManager.mIntance.CountRightFiger > 1) return;
        if (doTweenAnimation!=null)
        {
            doTweenAnimation.DOPlayForward();
        }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
