using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonGunManager : MonoBehaviour,IPointerDownHandler,IPointerClickHandler,IDragHandler,IEndDragHandler {
    
    public void OnDrag(PointerEventData eventData)
    {
        if (BackPackManager.mIntance.CountRightFiger > 1) return;
        //Debug.Log("Drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        BackPackManager.mIntance.CloseGunInfo();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        BackPackManager.mIntance.CloseGunInfo();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //先触发OnPointDown才会触发Update
        BackPackManager.mIntance.CalcRightTouchFiger();
        if (BackPackManager.mIntance.CountRightFiger > 1) return;
        GunInfoCtl info=new GunInfoCtl();
        BackPackManager.mIntance.ShowGunInfo(info);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
