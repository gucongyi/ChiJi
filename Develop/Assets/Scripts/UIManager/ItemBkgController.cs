using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemBkgController : MonoBehaviour,UIListViewItemInterface,IPointerClickHandler
{
    public Image Icon;
    public InfoHelper.BaseBkgInfo currInfo;
    Vector3 offset = new Vector3(-1f, 0.1f)*75;

    private UIListView mListView;

    public bool IsGOActiveTrue()
    {
        return gameObject ? gameObject.activeSelf : false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {//告诉那个item被按下
    //    Debug.Log("Item Pointer Down");
    //    Debug.Log("BackPackManager.mIntance.mCanvas.scaleFactor:" + BackPackManager.mIntance.mCanvas.scaleFactor);
    //    Debug.Log("BackPackManager.mIntance.mCanvasScaler.referenceResolution:" + BackPackManager.mIntance.mCanvasScaler.referenceResolution.x+","+BackPackManager.mIntance.mCanvasScaler.referenceResolution.y);
        if(mListView==null) return;
        //先触发OnPointDown才会触发Update
        BackPackManager.mIntance.CalcRightTouchFiger();
        if (BackPackManager.mIntance.CountRightFiger>1)return;
        if (currInfo.type==InfoHelper.EnumList.SKILL)return;
        mListView.theSelectItemInterface = this;
        BackPackManager.mIntance.OpenPropInfo(currInfo);
        BackPackManager.mIntance.SetPropInfoPos(currInfo.type);
        BackPackManager.mIntance.ResetUpDownColor();
        if (currInfo.type==InfoHelper.EnumList.DIAMOND)
        {
            BackPackManager.mIntance.ScaleLargeGunDiaIconsRoot();
        }
    }

    private void CalcTravelPos(PointerEventData eventData,Image CurrIcon, InfoHelper.BaseBkgInfo currInfo)
    {
        float posX = eventData.position.x/BackPackManager.mIntance.mCanvas.scaleFactor - BackPackManager.mIntance.mCanvasScaler.referenceResolution.x / 2f;//（转到1334,750）
        float posY = eventData.position.y/BackPackManager.mIntance.mCanvas.scaleFactor - BackPackManager.mIntance.mCanvasScaler.referenceResolution.y / 2f;
        
        BackPackManager.mIntance.GoTravel.transform.localPosition = new Vector3(posX, posY)+ offset;
        //BackPackManager.mIntance.GoTravel.GetComponent<Image>().sprite = CurrIcon.sprite;
        BackPackManager.mIntance.GoTravel.GetComponent<GunsDiaItemCtl>().Icon.sprite = CurrIcon.sprite;
        BackPackManager.mIntance.GoTravel.GetComponent<GunsDiaItemCtl>().info = currInfo;
    }

    public void SetUIListView(UIListView m_UIListView)
    {
        //initPrefab set
        mListView = m_UIListView;
        mListView.NormalEndDrag += NormalEndDrag;
    }

    public void NormalEndDrag(PointerEventData eventData)
    {
        BackPackManager.mIntance.ClosePropInfo();
    }

    public void UIListViewItem_OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("Item OnBeginDrag "+eventData.position);
        if (BackPackManager.mIntance.CountRightFiger > 1) return;
        if (currInfo.type == InfoHelper.EnumList.SKILL) return;
        BackPackManager.mIntance.GoTravel.SetActive(true);
        CalcTravelPos(eventData,Icon,currInfo);
        BackPackManager.mIntance.BeginDragPosY = eventData.position.y;
    }

    public void UIListViewItem_OnDrag(PointerEventData eventData)
    {
        //Debug.Log("Item OnDrag "+eventData.position);
        if (BackPackManager.mIntance.CountRightFiger > 1) return;
        if (currInfo.type == InfoHelper.EnumList.SKILL) return;
        CalcTravelPos(eventData, Icon,currInfo);
        BackPackManager.mIntance.EndDragPosY = eventData.position.y;
        BackPackManager.mIntance.ShowTextColor();
        if (currInfo.type == InfoHelper.EnumList.DIAMOND)
        {
            BackPackManager.mIntance.ShowDiaAdapte();
        }
    }

    public void UIListViewItem_OnEndDrag(PointerEventData eventData)
    {
        // Debug.Log("Item EndDrag " + eventData.position);
        if (currInfo.type == InfoHelper.EnumList.SKILL) return;
        BackPackManager.mIntance.GoTravel.SetActive(false);
        BackPackManager.mIntance.ClosePropInfo();
        BackPackManager.mIntance.EndDragPosY = eventData.position.y;
        BackPackManager.mIntance.TriggerPropUse(currInfo);
        if (currInfo.type == InfoHelper.EnumList.DIAMOND)
        {
            BackPackManager.mIntance.ScaleNormaoGunDiaIconsRoot();
            BackPackManager.mIntance.DiaAdapteDrop(BackPackManager.mIntance.GoTravel.GetComponent<GunsDiaItemCtl>());
        }
        BackPackManager.mIntance.ResetDiaAdapte();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (BackPackManager.mIntance.CountRightFiger > 0) return;//表示还有手指在操作，不关闭提示
        BackPackManager.mIntance.ClosePropInfo();
        if (currInfo.type == InfoHelper.EnumList.DIAMOND)
        {
            BackPackManager.mIntance.ScaleNormaoGunDiaIconsRoot();
        }
    }
}
