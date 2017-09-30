using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRoleController : MonoBehaviour
{
    public Image ImageSelected;

    public Image ImageIcon;

    public Button ButtonItem;

    public AppController.CatDogInfo currInfo ;

    public void ShowUI(AppController.CatDogInfo info,bool isSelected)
    {
        currInfo = info;
        if (isSelected)
        {
            ImageSelected.gameObject.SetActive(true);
        }
        else
        {
            ImageSelected.gameObject.SetActive(false);
        }
        
        string iconName = info.roleName;
        Sprite sprite = ResourceManager.LoadAsset("Textures", iconName, typeof(Sprite)) as Sprite;
        Debug.Assert(sprite != null);
        if (sprite != null)
        {
            ImageIcon.sprite = sprite;
            ImageIcon.SetNativeSize();
        }
    }
    
    // Use this for initialization
    void Start () {
        ButtonItem.onClick.AddListener(OnButtonItemClick);

    }

    void OnButtonItemClick()
    {
        if (LobbyController.mInstance.CurrSelectedRoleName==currInfo.roleName)
        {
            //点击同一个，不需要再次点击
            return;
        }
        int ItemCount = LobbyController.mInstance.listviewSelectRole.GetComponent<ScrollRect>().content.transform
            .childCount;
        for (int i = 0; i < ItemCount; i++)
        {//全部关闭
            LobbyController.mInstance.listviewSelectRole.GetComponent<ScrollRect>().content.transform.GetChild(i)
                .GetComponent<ItemRoleController>().ImageSelected.gameObject.SetActive(false);
        }
        this.ImageSelected.gameObject.SetActive(true);
        //换模型，换背景，换属性
        LobbyController.mInstance.SetRoleProp(currInfo.roleName);
        AppController.mInstance.SetBg(currInfo.roleName, currInfo.type);
        AppController.mInstance.SetModel(currInfo.roleName, currInfo.type);
        LobbyController.mInstance.WillSetRoleName = currInfo.roleName;
        LobbyController.mInstance.WillSetRoleType = currInfo.type;
        LobbyController.mInstance.CurrSelectedRoleName = currInfo.roleName;
    }

    // Update is called once per frame
	void Update () {
		
	}
}
