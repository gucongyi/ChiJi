using System;
using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController mInstance;
    public Transform TransFirstUI;
    public Transform TransThirdUI;
    public Transform TransFirstUIOther;
    public Button roleButton;
    public Button StartGameButton;
    public Transform TransSecondUIOther;
    public Button SecondReturnButton;
    public Button SecondOkButton;
    public Image SecondPropImage;

    public Button ThirdButtonReturn;
    public Button ThirdEnterGame;
    public GameObject ThirdTitle;
    public GameObject SingleButtonRoot;
    public GameObject DoubleButtonRoot;
    public GameObject FourButtonRoot;
    public GameObject ToggleGroupButtonRoot;

    #region Select Role
    public GameObject prefabRoleItem;
    public UIListView listviewSelectRole;
    public string WillSetRoleName = string.Empty;
    public SelectRoleType WillSetRoleType;
    public Toggle[] Toggles;
    public RoleTabItemController[] tabsCtl;
    public string CurrSelectedRoleName;
    #endregion

    public const float moveTime = 0.5f;
    public const float roleButtonY1 = -317f;
    public const float roleButtonY2 = 329f;
    public const float firstUIOtherY1= 0f;
    public const float firstUIOtherY2 = 800f;
    public const float firstUIX1 = 0f;
    public const float firstUIX2 = -1334f;
    public const float thirdUIX1 = 1334f;
    public const float thirdUIX2 = 0f;
    public const float secondUIOtherY1 = -800f;
    public const float secondUIOtherY2 = 0f;
    private float displayProgress=0f;
    void Awake()
    {
        mInstance = this;
        roleButton.onClick.AddListener(OnRoleButtonClick);
        SecondReturnButton.onClick.AddListener(OnSecondReturnButtonClick);
        SecondOkButton.onClick.AddListener(OnecondOkButtonClick);
        StartGameButton.onClick.AddListener(OnStartGameButtonClick);
        ThirdButtonReturn.onClick.AddListener(OnThirdButtonReturnClick);
        ThirdEnterGame.onClick.AddListener(OnThirdEnterGameClick);
        if (PhotonManager.Instance)
        {
            PhotonManager.Instance.ConnectLobby();
        }
    }

    public enum SelectRoleType
    {
        CAT=0,
        DOG
    }

    public SelectRoleType CurrRoleType=SelectRoleType.CAT;
    public void ShowRole(SelectRoleType type)
    {
        CurrRoleType = type;
        listviewSelectRole.InitListView(new Vector2(132f, 142f), new Vector2(22f, 14f), 2, prefabRoleItem, UIListView.AxisType.vertical);
        listviewSelectRole.setValueCallback = ShowCatDogInfoItem;
        switch (type)
        {
            case SelectRoleType.CAT:
                listviewSelectRole.ShowData(AppController.mInstance.ListCat);
                break;
            case SelectRoleType.DOG:
                listviewSelectRole.ShowData(AppController.mInstance.ListDog);
                break;
        }
    }

    public void ShowCatDogInfoItem(GameObject prefabGo, object data, int dataIndex)
    {
        AppController.CatDogInfo info = (AppController.CatDogInfo) data;
        ItemRoleController itemRoleCtl = prefabGo.GetComponent<ItemRoleController>();
        if (info.roleName== CurrSelectedRoleName)
        {
            itemRoleCtl.ShowUI(info,true);
        }
        else
        {
            itemRoleCtl.ShowUI(info, false);
        }
    }
    

    public void OnRoleButtonClick()
    {
        roleButton.interactable = false;//不允许交互了
        roleButton.GetComponent<EventTrigger>().enabled = false;
        UIMoveRole(firstUIOtherY2,roleButtonY2,secondUIOtherY2);
        CurrSelectedRoleName = AppController.mInstance.RoleNameScene();
        if (AppController.mInstance.roleType== SelectRoleType.CAT)
        {
            ShowRole(SelectRoleType.CAT);//默认显示猫
            CurrRoleType = SelectRoleType.CAT;
            Toggles[0].isOn = true;
            tabsCtl[0].SetIsOn(true);
            Toggles[1].isOn = false;
            tabsCtl[1].SetIsOn(false);
        }
        else
        {
            ShowRole(SelectRoleType.DOG);//默认显示狗
            CurrRoleType = SelectRoleType.DOG;
            Toggles[0].isOn = false;
            tabsCtl[0].SetIsOn(false);
            Toggles[1].isOn = true;
            tabsCtl[1].SetIsOn(true);
        }
        SetRoleProp(AppController.mInstance.RoleNameScene());
    }

    public void SetRoleProp(string roleName)
    {
        Sprite sprite = ResourceManager.LoadAsset("Textures", roleName+"_Prop", typeof(Sprite)) as Sprite;
        SecondPropImage.sprite = sprite;
    }

    public void OnSecondReturnButtonClick()
    {
        roleButton.interactable = true;//允许交互了
        roleButton.GetComponent<EventTrigger>().enabled = true;
        UIMoveRole(firstUIOtherY1,roleButtonY1,secondUIOtherY1);
        //换模型，换背景，换属性,换回原来的
        LobbyController.mInstance.SetRoleProp(AppController.mInstance.roleName);
        AppController.mInstance.SetBg(AppController.mInstance.roleName, AppController.mInstance.roleType);
        AppController.mInstance.SetModel(AppController.mInstance.roleName, AppController.mInstance.roleType);
    }
    public void OnecondOkButtonClick()
    {
        roleButton.interactable = true;//允许交互了
        roleButton.GetComponent<EventTrigger>().enabled = true;
        UIMoveRole(firstUIOtherY1,roleButtonY1,secondUIOtherY1);
        if (!string.IsNullOrEmpty(WillSetRoleName))
        {
            AppController.mInstance.roleName = WillSetRoleName;
            AppController.mInstance.roleType = WillSetRoleType;
            FileHelper.createOrWriteRoleFile(WillSetRoleName);
        }
    }

    public void OnStartGameButtonClick()
    {
        UIMoveStartGame(firstUIX2, thirdUIX2);
    }

    public void OnThirdButtonReturnClick()
    {
        UIMoveStartGame(firstUIX1, thirdUIX1);
    }

    public void UIMoveRole(float firstUIOtherTo, float roleButtonTo,float secondUIOtherTo)
    {
        TransFirstUIOther.DOLocalMoveY(firstUIOtherTo, moveTime).SetEase(Ease.OutCubic);
        roleButton.transform.DOLocalMoveY(roleButtonTo, moveTime).SetEase(Ease.OutCubic);
        TransSecondUIOther.DOLocalMoveY(secondUIOtherTo, moveTime).SetEase(Ease.OutCubic);
    }

    public void UIMoveStartGame(float firstUITo, float thirdUITo)
    {
        TransFirstUI.DOLocalMoveX(firstUITo, moveTime).SetEase(Ease.OutCubic);
        TransThirdUI.transform.DOLocalMoveX(thirdUITo, moveTime).SetEase(Ease.OutCubic);
    }
    



    public void OnThirdEnterGameClick()
    {
        
        AppController.mInstance.isProgressNet = true;
        UIRoot.mInstance.EnableLoading();
        LobbyController.mInstance.LoadingDisableUI();
        PhotonManager photonManager = PhotonManager.Instance;
        if (photonManager)
        {
            PhotonNetwork.ClearBattleListCache();
            photonManager.JoinOrCreateRoom("");
        }
    }

    // Use this for initialization
    void Start () {
        if (AppController.mInstance!=null)
        {
            AppController.mInstance.CloseMultiTouch();
            CurrSelectedRoleName = AppController.mInstance.RoleNameScene();
        }
    }

    public void LoadingDisableUI()
    {
        ThirdEnterGame.gameObject.SetActive(false);
        ToggleGroupButtonRoot.SetActive(false);
        SingleButtonRoot.gameObject.SetActive(false);
        DoubleButtonRoot.gameObject.SetActive(false);
        FourButtonRoot.gameObject.SetActive(false);
        ThirdButtonReturn.gameObject.SetActive(false);
        ThirdTitle.SetActive(false);
    }

    public void LoadingTimeOutOpenUI()
    {
        ThirdEnterGame.gameObject.SetActive(true);
        ToggleGroupButtonRoot.SetActive(true);
        SingleButtonRoot.gameObject.SetActive(true);
        ThirdButtonReturn.gameObject.SetActive(true);
        ThirdTitle.SetActive(true);
    }

    // Update is called once per frame
    void Update () {

        if (AppController.mInstance.isProgressNet)
        {
            AppController.mInstance.TimeBattleNetTimeOut -= Time.deltaTime;
            displayProgress += 0.003f;
            if (displayProgress > 1f)
            {
                displayProgress = 1f;
            }

            UIRoot.mInstance.LoadingBattleSlider.value = (1 - SceneController.BattleScenePercent) * displayProgress;

            if (AppController.mInstance.TimeBattleNetTimeOut < 0f)
            {
                AppController.mInstance.TimeBattleNetTimeOut = 10f;
                UIRoot.mInstance.DisableLoading();
                AppController.mInstance.isProgressNet = false;
                UIRoot.mInstance.promotGo.GetComponent<PromotController>().ShowToast("网络连接超时，请检查网络！");
                LoadingTimeOutOpenUI();
                displayProgress = 0f;

            }
        }
    }
}
