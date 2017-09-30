using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;

public class AppController :MonoBehaviour
{
    public static AppController mInstance;
    [HideInInspector]
    public  SceneController MySceneController;
    [HideInInspector]
    public  float TimeBattleNetTimeOut;
    [HideInInspector]
    public bool isProgressNet;
    [HideInInspector]
    public string roleName;

    [HideInInspector] public LobbyController.SelectRoleType roleType;
    private GameObject CurrRoleModelGo;


    public class CatDogInfo
    {
        public string roleName;
        public LobbyController.SelectRoleType type;

        public CatDogInfo(string roleName, LobbyController.SelectRoleType type)
        {
            this.roleName = roleName;
            this.type = type;
        }
    }
    public List<CatDogInfo> ListCat=new List<CatDogInfo>();
    public List<CatDogInfo> ListDog = new List<CatDogInfo>();

    private Transform RoleInRoot;
    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else {
            if (mInstance != this)
                Destroy(gameObject);
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Application.targetFrameRate = 60;
        MySceneController=SceneController.GenerateObject("SceneManager").GetComponent<SceneController>();
        ResourceManager.GenerateObject("ResourceManager");

        TimeBattleNetTimeOut = 10f;


        CatDogInfo infoCat1=new CatDogInfo("AssassinCat",LobbyController.SelectRoleType.CAT);
        CatDogInfo infoCat2 = new CatDogInfo("AssassinCat1", LobbyController.SelectRoleType.CAT);
        ListCat.Clear();
        ListCat.Add(infoCat1);
        ListCat.Add(infoCat2);
        
        CatDogInfo infoDog1 = new CatDogInfo("NinjiaDog", LobbyController.SelectRoleType.DOG);
        CatDogInfo infoDog2 = new CatDogInfo("NinjiaDog1", LobbyController.SelectRoleType.DOG);
        ListDog.Clear();
        ListDog.Add(infoDog1);
        ListDog.Add(infoDog2);
    }

    public void OpenMultiTouch()
    {
        Input.multiTouchEnabled = true;
    }
    public void CloseMultiTouch()
    {
        Input.multiTouchEnabled = false;
    }

    public string RoleNameScene()
    {
        return roleName;
    }

    public void SetBg(string roleName, LobbyController.SelectRoleType CurrRoleType)
    {
        if (CurrRoleType== LobbyController.SelectRoleType.CAT)
        {
            roleName = "Cat";
        }
        else
        {
            roleName = "Dog";
        }
        GameObject BgGo=GameObject.Find("RoleRoot/Bg");
        MeshRenderer render = BgGo.GetComponent<MeshRenderer>();
        Texture2D texture = ResourceManager.LoadAsset("Textures", roleName+"_Bg", typeof(Texture2D)) as Texture2D;
        render.material.SetTexture("_MainTex", texture);
    }

    public void SetModel(string roleName, LobbyController.SelectRoleType currType)
    {
        if (RoleInRoot==null)
        {
            GameObject roleInRootGo = GameObject.Find("RoleInRoot");
            if (roleInRootGo!=null)
            {
                RoleInRoot = roleInRootGo.transform;
            }
        }
        ResourceManager.LoadResourceAsync(roleName+"Show", completed: (GameObject go) =>
        {
            if (CurrRoleModelGo!=null)
            {
                Destroy(CurrRoleModelGo);
                ResourceManager.Ins.RemoveDicPrefabGo(roleName + "Show");
            }
            ResourceManager.Ins.AddDicPrefabGo(roleName + "Show", go);
            go.transform.parent = RoleInRoot;
            go.transform.localPosition = Vector3.zero;
            if (currType==LobbyController.SelectRoleType.CAT)
            {
                go.transform.localPosition = new Vector3(0f,-0.23f,0f);
            }
            go.transform.localEulerAngles = Vector3.zero;
            go.transform.localScale = Vector3.one;
            CurrRoleModelGo = go;

        }, path: "ModelUIShow");
    }

    public void ResetRoleAndLobbyUI()
    {
        GameObject roleInRootGo = GameObject.Find("RoleInRoot");
        if (roleInRootGo==null)
        {
            Debug.Assert(roleInRootGo!=null);
        }
        RoleInRoot = roleInRootGo.transform;
        roleName = FileHelper.LoadAndRoleNameFile();
        if (string.IsNullOrEmpty(roleName))
        {
            roleName = "NinjiaDog";
        }
        if (roleName.Contains("Cat"))
        {
            SetBg(RoleNameScene(),LobbyController.SelectRoleType.CAT);
            roleType = LobbyController.SelectRoleType.CAT;
        }
        else
        {
            SetBg(RoleNameScene(), LobbyController.SelectRoleType.DOG);
            roleType = LobbyController.SelectRoleType.DOG;
        }
        SetModel(RoleNameScene(), roleType);
        ResourceManager.LoadResourceAsync("CanvasLobby", completed: (GameObject go) =>
        {
            ResourceManager.Ins.AddDicPrefabGo("CanvasLobby", go);
            go.transform.parent = UIRoot.mInstance.gameObject.transform;
            Canvas loginCanvas = go.GetComponent<Canvas>();
            loginCanvas.worldCamera = UIRoot.mInstance.UiCamera;

        });
        if (PhotonManager.Instance)
        {
            PhotonManager.Instance.ConnectLobby();
        }
    }

    // Use this for initialization
	void Start ()
	{
        Debug.Log("Application.persistentDataPath:"+Application.persistentDataPath);
	    ResetRoleAndLobbyUI();
        ResourceManager.LoadResourceAsync("CanvasPromot", completed: (GameObject go) =>
	    {
	        ResourceManager.Ins.AddDicPrefabGo("CanvasPromot", go);
            go.transform.parent = UIRoot.mInstance.gameObject.transform;
	        Canvas promotCanvas = go.GetComponent<Canvas>();
	        promotCanvas.worldCamera = UIRoot.mInstance.UiCamera;
            go.SetActive(false);
	        UIRoot.mInstance.promotGo = go;
	    });
	    ResourceManager.LoadResourceAsync("CanvasLoading", completed: (GameObject go) =>
	    {
	        ResourceManager.Ins.AddDicPrefabGo("CanvasLoading", go);
	        go.transform.parent = UIRoot.mInstance.gameObject.transform;
	        Canvas loadingCanvas = go.GetComponent<Canvas>();
	        loadingCanvas.worldCamera = UIRoot.mInstance.UiCamera;
	        go.SetActive(false);
	        UIRoot.mInstance.LoadingBattleGo = go;
            UIRoot.mInstance.LoadingBattleSlider = go.GetComponent<LoadingBattleScene>().LoadingSlider;
	    });
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
