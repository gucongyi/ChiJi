using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CatsAndDogs {
    public class BackPackManager : MonoBehaviour
    {
        public const int maxWeight = 100;
        public GameObject GoBtnBkg;
        public GameObject GoBkgContent;
        public GameObject GoPropInfo;
        public GameObject GoDiaIconsRoot;
        public GameObject GoGunRoot;
        public GameObject GoGunInfoRoot;
        public GameObject GoWearRoot;
        public Transform[] GunDiaIconsPos;
        public GameObject[] GoGunDiasEffect;
        public Image[] ImageGunDias;
        public GameObject prefabItem;
        public Canvas mCanvas;
        public CanvasScaler mCanvasScaler;
        public GameObject GoTravel;
        public Transform[] PosPropInfoArray;
        public PickupUIController pickUI;
        public Transform propBkgOpenPos;
        public Transform proBkgClosePos;
        private bool isBkgOpen;

        #region Weight

        public Text TextInnerCurrWeight;
        public Text TextInnerSplitWeight;
        public Text TextInnerMaxWeight;
        public Slider SliderButtonBkg;

        #endregion
        [HideInInspector]
        public float BeginDragPosY;
        [HideInInspector]
        public float EndDragPosY;

        public float TriggerFunPosY;
        public Text TextPropInfoUp;
        public Text TextPropInfoDown;
        public static BackPackManager mIntance;
        public float CurrWeight;

        #region Gun

        private InfoHelper.BkgGunInfo CurrGunInfo;
        private List<InfoHelper.BkgDiaInfo> CurrGunDiasInfoList=new List<InfoHelper.BkgDiaInfo>();

        #endregion
        #region Wear

        private InfoHelper.BkgWearInfo CurrWearInfo;

        #endregion
        #region dia
        public UIListView listViewDiamond;
        private List<InfoHelper.BkgDiaInfo> listDia = new List<InfoHelper.BkgDiaInfo>();
        #endregion
        #region grenade
        public UIListView listViewGrenade;
        private List<InfoHelper.BkgGrenadeInfo> listGrenade = new List<InfoHelper.BkgGrenadeInfo>();
        #endregion
        #region Prop
        public UIListView listViewProp;
        private List<InfoHelper.BkgMedicineInfo> listMedicine = new List<InfoHelper.BkgMedicineInfo>();
        #endregion
        #region Skill
        public UIListView listViewSkill;
        private List<InfoHelper.BkgSkillInfo> listSkill = new List<InfoHelper.BkgSkillInfo>();
        private InfoHelper.BkgSkillInfo CurrSkillInfo;
        #endregion

        #region RigisterEvent

        public System.Action<InfoHelper.BaseBkgInfo> OnUsePropEvent;
        public System.Action<InfoHelper.BaseBkgInfo> OnGiveUpPropEvent;


        #endregion

        // Use this for initialization
        void Awake()
        {
            mIntance = this;
            CurrWeight = 0f;
        }

        public void SetPickUpUI(GameObject go, string id,int durable)
        {
           
            pickUI.gameObject.SetActive(true);
            if (isBkgOpen)//背包打开状态
            {
                pickUI.transform.position = propBkgOpenPos.position;
            }
            else//背包打开状态
            {
                pickUI.transform.position = proBkgClosePos.position;
            }
            pickUI.ShowUI(go, id, durable);
            
        }

        public void ClosePickUpUI()
        {
            pickUI.gameObject.SetActive(false);
        }

        public void ShowWeight()
        {
            TextInnerCurrWeight.text=CurrWeight+"";
            TextInnerMaxWeight.text = maxWeight + "";
            if (CurrWeight> maxWeight)
            {
                ColorHelper.SetColor(TextInnerCurrWeight,"#FF0000");
                ColorHelper.SetColor(TextInnerSplitWeight, "#FF0000");
                ColorHelper.SetColor(TextInnerMaxWeight, "#FF0000");
            }
            SliderButtonBkg.value= CurrWeight/maxWeight;
    }

        public void ShowGunInfo(GunInfoCtl ctl)
        {
            GoGunInfoRoot.SetActive(true);
            ShowGunInfoDia();
        }
        public void CloseGunInfo()
        {
            GoGunInfoRoot.SetActive(false);
        }

        public void ShowDiaAdapte()
        {
            int index = 0;
            foreach (var eachPos in GunDiaIconsPos)
            {
                float distance = Vector3.Distance(GoTravel.transform.localPosition, eachPos.localPosition);
                //Debug.Log("distance:" + distance);
                if (distance<20f)//在圆内了
                {
                    GoGunDiasEffect[index].SetActive(true);
                }
                else
                {
                    GoGunDiasEffect[index].SetActive(false);
                }
                index++;
            }
        }

        public void ResetDiaAdapte()
        {
            foreach (var eachEffect in GoGunDiasEffect)
            {
                eachEffect.SetActive(false);
            }
        }

        public void DiaAdapteDrop(GunsDiaItemCtl ctl)
        {
            int index = 0;
            foreach (var eachPos in GunDiaIconsPos)
            {
                float distance = Vector3.Distance(GoTravel.transform.localPosition, eachPos.localPosition);
                //Debug.Log("distance:" + distance);
                if (distance < 20f)//在圆内了
                {
                    GunsDiaItemCtl CurrDiaItemCtl = ImageGunDias[index].GetComponent<GunsDiaItemCtl>();
                    CurrDiaItemCtl.Icon.sprite = ctl.Icon.sprite;
                    listDia.Remove((InfoHelper.BkgDiaInfo)ctl.info);
                    if (CurrDiaItemCtl.info!=null)
                    {
                        listDia.Add((InfoHelper.BkgDiaInfo)CurrDiaItemCtl.info);
                    }
                    listViewDiamond.ShowData(listDia);
                    CurrDiaItemCtl.info = ctl.info;//替换
                    //记录当前的信息。
                    CurrGunDiasInfoList[index] = (InfoHelper.BkgDiaInfo)ctl.info;
                    CurrGunInfo.listGunDias[index] = CurrGunDiasInfoList[index];
                    if (BattleSceneManager.Instance != null)
                    {
                        Debug.Log("Gem index:" + index);
                        BattleSceneManager.Instance.myCharacter.backpack.MountGem(CurrGunDiasInfoList[index].id,index);
                        
                    }
                }
                index++;
            }
        }

        public void ScaleLargeGunDiaIconsRoot()
        {
            GoDiaIconsRoot.transform.localScale=Vector3.one;
        }
        public void ScaleNormaoGunDiaIconsRoot()
        {
            GoDiaIconsRoot.transform.localScale = Vector3.one*0.8f;
        }

        public void OpenPropInfo(InfoHelper.BaseBkgInfo currInfo)
        {
            GoPropInfo.SetActive(true);
            GoPropInfo.GetComponent<BkgPropInfoManager>().ShowPropInfoUI(currInfo);
        }

        public void ClosePropInfo()
        {
            GoPropInfo.SetActive(false);
        }

        public void SetPropInfoPos(InfoHelper.EnumList who)
        {
            switch (who)
            {
                case InfoHelper.EnumList.DIAMOND:
                    GoPropInfo.transform.localPosition = PosPropInfoArray[0].localPosition;
                    break;
                case InfoHelper.EnumList.GRENADE:
                    GoPropInfo.transform.localPosition = PosPropInfoArray[1].localPosition;
                    break;
                case InfoHelper.EnumList.MEDICINE:
                    GoPropInfo.transform.localPosition = PosPropInfoArray[2].localPosition;
                    break;
                case InfoHelper.EnumList.SKILL:
                    GoPropInfo.transform.localPosition = PosPropInfoArray[3].localPosition;
                    break;

            }
        }

        public void ShowTextColor()
        {
            if (EndDragPosY-BeginDragPosY> TriggerFunPosY)//在开始位置上方
            {
                ColorHelper.SetColor(TextPropInfoUp, "#E50303FF");
                ColorHelper.SetColor(TextPropInfoDown, "#FFFFFFFF");
            }
            else if (BeginDragPosY-EndDragPosY > TriggerFunPosY)
            {
                ColorHelper.SetColor(TextPropInfoUp, "#FFFFFFFF");
                ColorHelper.SetColor(TextPropInfoDown, "#E50303FF");
            }
            else 
            {
                ColorHelper.SetColor(TextPropInfoUp, "#FFFFFFFF");
                ColorHelper.SetColor(TextPropInfoDown, "#FFFFFFFF");
            }
        }

        public void TriggerPropUse(InfoHelper.BaseBkgInfo currInfo)
        {
            if (BackPackManager.mIntance.CountRightFiger > 1) return;
            if (EndDragPosY - BeginDragPosY > TriggerFunPosY)//上滑使用
            {
                Debug.Log("use:"+currInfo.iconName+" "+currInfo.type);
                if (OnUsePropEvent!=null)
                {
                    if (currInfo.type!=InfoHelper.EnumList.DIAMOND&& currInfo.type != InfoHelper.EnumList.SKILL)
                    {
                        OnUsePropEvent(currInfo);
                        CurrWeight -= currInfo.weight;
                    }
                }
                RemoveFromUIScrollView(currInfo,true);
            }
            else if(BeginDragPosY  - EndDragPosY > TriggerFunPosY)//下滑丢弃
            {
                Debug.Log("giveup:" + currInfo.iconName + " " + currInfo.type);
                if (OnGiveUpPropEvent != null)
                {
                    if (currInfo.type != InfoHelper.EnumList.DIAMOND && currInfo.type != InfoHelper.EnumList.SKILL)
                    {
                        OnGiveUpPropEvent(currInfo);
                        CurrWeight -= currInfo.weight;
                    }
                }
                RemoveFromUIScrollView(currInfo,false);
            }
        }

        private void RemoveFromUIScrollView(InfoHelper.BaseBkgInfo currInfo,bool isUp)
        {
            switch (currInfo.type)
            {
                case InfoHelper.EnumList.DIAMOND:
                    if (isUp == false)
                    {//下滑时直接删除，上拉要到指定位置。
                        listDia.Remove((InfoHelper.BkgDiaInfo)currInfo);
                        listViewDiamond.ShowData(listDia);
                    }
                    break;
                case InfoHelper.EnumList.GRENADE:
                    listGrenade.Remove((InfoHelper.BkgGrenadeInfo) currInfo);
                    listViewGrenade.ShowData(listGrenade);
                    break;
                case InfoHelper.EnumList.MEDICINE:
                    listMedicine.Remove((InfoHelper.BkgMedicineInfo) currInfo);
                    listViewProp.ShowData(listMedicine);
                    break;
                //case InfoHelper.EnumList.SKILL:
                //    listSkill.Remove((InfoHelper.BkgSkillInfo) currInfo);
                //    listViewSkill.ShowData(listSkill);
                //    break;
            }
        }

        public void ResetUpDownColor()
        {
            ColorHelper.SetColor(TextPropInfoUp, "#FFFFFFFF");
            ColorHelper.SetColor(TextPropInfoDown, "#FFFFFFFF");
        }
        
        public void OnButtonCloseBkgClick(bool isInner) {
            if (BackPackManager.mIntance.CountRightFiger > 1) return;
            if (isInner)
            {
                GoBtnBkg.SetActive(true);
                GoBkgContent.SetActive(false);
                isBkgOpen = false;
                BattleUIManager.Instance.rightPanelEnable = true;
            }
            else//外边点进来
            {
                GoBtnBkg.SetActive(false);
                GoBkgContent.SetActive(true);
                isBkgOpen = true;

                ShowGun();
                ShowWear();
                ShowDiamond();
                ShowGreade();
                ShowMedicine();
                ShowSkill();
                BattleUIManager.Instance.rightPanelEnable = false;
            }
        }

        void Start() {
            listDia.Clear();
            listGrenade.Clear();
            listMedicine.Clear();
            listSkill.Clear();
            CurrGunDiasInfoList.Clear();
            InfoHelper.BkgDiaInfo dia1=new InfoHelper.BkgDiaInfo("");
            InfoHelper.BkgDiaInfo dia2 = new InfoHelper.BkgDiaInfo("");
            InfoHelper.BkgDiaInfo dia3 = new InfoHelper.BkgDiaInfo("");
            CurrGunDiasInfoList.Add(dia1);
            CurrGunDiasInfoList.Add(dia2);
            CurrGunDiasInfoList.Add(dia3);
            GoBtnBkg.SetActive(true);
            GoBkgContent.SetActive(false);

            //TestGun("1002");
            //TestWear("6002",100);
            //TestDia();
            //TestGreade();
            //TestMedicine();
            //TestSkill();
        }

        public string RepalaceGun(string gunId)
        {
            string oldId = string.Empty;
            InfoHelper.BkgGunInfo info = new InfoHelper.BkgGunInfo(gunId);
            foreach (var diaInfo in CurrGunDiasInfoList)
            {//宝石一直在
                info.listGunDias.Add(diaInfo);
            }
            if (CurrGunInfo!=null)
            {
                CurrWeight -= CurrGunInfo.weight;
                oldId = CurrGunInfo.id;
            }
            CurrGunInfo = info;
            CurrWeight += CurrGunInfo.weight;//替换重量
            if (BattleSceneManager.Instance!=null)
            {
                BattleSceneManager.Instance.myCharacter.backpack.ReplaceGun(info.id);
            }
            return oldId;
        }

        public class DiaGunInfo
        {
            public int Count;
            public List<string> iconNameList;
            public int effectValue;
        }

        Dictionary<InfoHelper.EnumTypeDia, DiaGunInfo> dicGunDia = new Dictionary<InfoHelper.EnumTypeDia, DiaGunInfo>();
        public void ShowGun()
        {
            //显示gun信息
            GoGunRoot.GetComponent<BkgGunUIManager>().ShowGunInfo(CurrGunInfo);
        }

        public void ShowGunInfoDia()
        {
            CaclGunEachDiaCount();
            GoGunRoot.GetComponent<BkgGunUIManager>().ShowInfoDia(CurrGunInfo, dicGunDia);
        }

        private void CaclGunEachDiaCount()
        {
//根据枪的类型进行统计宝石个数
            dicGunDia.Clear();
            foreach (var diaInfo in CurrGunDiasInfoList)
            {
                if (diaInfo.typeDia == InfoHelper.EnumTypeDia.UNKNOWN) continue;
                DiaGunInfo DiaGunInfo = null;
                if (dicGunDia.TryGetValue(diaInfo.typeDia, out DiaGunInfo))
                {
                    DiaGunInfo.Count++;
                    DiaGunInfo.effectValue = diaInfo.effectValue;
                    DiaGunInfo.iconNameList.Add(diaInfo.iconName);
                }
                else
                {
                    DiaGunInfo = new DiaGunInfo();
                    DiaGunInfo.Count = 1;
                    DiaGunInfo.effectValue = diaInfo.effectValue;
                    DiaGunInfo.iconNameList = new List<string>();
                    DiaGunInfo.iconNameList.Add(diaInfo.iconName);
                    dicGunDia.Add(diaInfo.typeDia, DiaGunInfo);
                }
            }
        }

        public void TestGun(string gunId)
        {
            RepalaceGun(gunId);
        }
        public class WearOldInfo
        {
            public string wearId;
            public int durable;
        }

        public void SetArmorDuarable(float Durable)
        {
            if (CurrWearInfo != null)
            {
                CurrWearInfo.durable = (int) Durable;
            }
        }

        public WearOldInfo RepalaceWear(string wearId, int durable)
        {
            WearOldInfo oldInfo = null;
            InfoHelper.BkgWearInfo info = new InfoHelper.BkgWearInfo(wearId, durable);
            if (CurrWearInfo!=null)
            {
                CurrWeight -= CurrWearInfo.weight;
                oldInfo=new WearOldInfo();
                oldInfo.wearId = CurrWearInfo.id;
                oldInfo.durable = CurrWearInfo.durable;
            }
            else//第一次捡到
            {
                info.durable = 100;
            }
            CurrWearInfo = info;
            CurrWeight += CurrWearInfo.weight;//替换重量
            if (BattleSceneManager.Instance != null)
            {
                BattleSceneManager.Instance.myCharacter.backpack.ReplaceWear(CurrWearInfo.id,durable);
            }
            return oldInfo;
        }
        
        public void ShowWear()
        {
            //显示gun信息
            GoWearRoot.GetComponent<BkgWearUIManager>().ShowWearInfo(CurrWearInfo);
        }

        public void TestWear(string wearId, int durable)
        {
            RepalaceWear(wearId,durable);
        }

        public void AddDiamond(string id)
        {
            InfoHelper.BkgDiaInfo info = new InfoHelper.BkgDiaInfo(id);
            listDia.Add(info);
            CurrWeight += info.weight;
            if (BattleSceneManager.Instance != null)
            {
                BattleSceneManager.Instance.myCharacter.backpack.AddGem(info.id);
            }
        }

        public void ShowDiamond()
        {
            listViewDiamond.InitListView(new Vector2(80f, 80f), new Vector2(8f, 8f), 1, prefabItem, UIListView.AxisType.horizontal, isUseItemInterface: true);
            listViewDiamond.setValueCallback = ShowDiaItem;
            listViewDiamond.ShowData(listDia);
        }

        private void TestDia() {
            AddDiamond("2001");
            AddDiamond("2001");
            AddDiamond("2002");
            AddDiamond("2003");
            AddDiamond("2004");
            AddDiamond("2005");
            AddDiamond("2001");
            AddDiamond("2002");
            AddDiamond("2003");
        }
        private void ShowDiaItem(GameObject prefabObject, object data, int dataIndex) {
            InfoHelper.BkgDiaInfo diaInfo = (InfoHelper.BkgDiaInfo)data;
            ItemBkgController itemCtl = prefabObject.GetComponent<ItemBkgController>();
            Sprite sprite = ResourceManager.LoadAsset("Textures", diaInfo.iconName, typeof(Sprite)) as Sprite;
            Debug.Assert(sprite != null);
            if (sprite != null) {
                itemCtl.Icon.sprite = sprite;
            }
            itemCtl.currInfo= diaInfo;
        }

        public void AddGreade(string id)
        {
            InfoHelper.BkgGrenadeInfo info=new InfoHelper.BkgGrenadeInfo(id);
            listGrenade.Add(info);
            CurrWeight += info.weight;
        }

        public void ShowGreade()
        {
            listViewGrenade.InitListView(new Vector2(80f, 80f), new Vector2(8f, 8f), 1, prefabItem, UIListView.AxisType.horizontal, isUseItemInterface: true);
            listViewGrenade.setValueCallback = ShowGrenadeItem;
            listViewGrenade.ShowData(listGrenade);
        }
        private void TestGreade() {
            
            AddGreade("3001");
            AddGreade("3002");
            AddGreade("3003");
            AddGreade("3004");
            AddGreade("3005");
            AddGreade("3006");
            AddGreade("3007");
            AddGreade("3008");
        }
        private void ShowGrenadeItem(GameObject prefabObject, object data, int dataIndex) {
            InfoHelper.BkgGrenadeInfo diaInfo = (InfoHelper.BkgGrenadeInfo)data;
            ItemBkgController itemCtl = prefabObject.GetComponent<ItemBkgController>();
            Sprite sprite = ResourceManager.LoadAsset("Textures", diaInfo.iconName, typeof(Sprite)) as Sprite;
            Debug.Assert(sprite != null);
            if (sprite != null) {
                itemCtl.Icon.sprite = sprite;
            }
            itemCtl.currInfo = diaInfo;
        }


        public void AddMedicine(string id)
        {
            InfoHelper.BkgMedicineInfo info=new InfoHelper.BkgMedicineInfo(id);
            listMedicine.Add(info);
            CurrWeight += info.weight;
            if (BattleSceneManager.Instance != null)
            {
                BattleSceneManager.Instance.myCharacter.backpack.AddMedicine(info.id);
            }
        }

        public void ShowMedicine()
        {
            listViewProp.InitListView(new Vector2(80f, 80f), new Vector2(8f, 8f), 1, prefabItem, UIListView.AxisType.horizontal, isUseItemInterface: true);
            listViewProp.setValueCallback = ShowMedicineItem;
            listViewProp.ShowData(listMedicine);
        }
        private void TestMedicine() {
            
            AddMedicine("4001");
            AddMedicine("4002");
            AddMedicine("4003");
            AddMedicine("4004");
            AddMedicine("4005");
            AddMedicine("4006");
            AddMedicine("4007");
            AddMedicine("4008");
            
        }
        private void ShowMedicineItem(GameObject prefabObject, object data, int dataIndex) {
            InfoHelper.BkgMedicineInfo diaInfo = (InfoHelper.BkgMedicineInfo)data;
            ItemBkgController itemCtl = prefabObject.GetComponent<ItemBkgController>();
            Sprite sprite = ResourceManager.LoadAsset("Textures", diaInfo.iconName, typeof(Sprite)) as Sprite;
            Debug.Assert(sprite != null);
            if (sprite != null) {
                itemCtl.Icon.sprite = sprite;
            }
            itemCtl.currInfo = diaInfo;
        }


        public void ReplaceSkill(string id)
        {
            if (CurrSkillInfo!=null)
            {
                CurrWeight -= CurrSkillInfo.weight;
                listSkill.Remove(CurrSkillInfo);
            }
            InfoHelper.BkgSkillInfo info=new InfoHelper.BkgSkillInfo(id);
            listSkill.Add(info);
            CurrWeight += info.weight;
            CurrSkillInfo = info;
            if (BattleSceneManager.Instance != null)
            {
                BattleSceneManager.Instance.myCharacter.skill.SetSkill(CurrSkillInfo.id);
            }
        }

        

        public void ShowSkill()
        {
            listViewSkill.InitListView(new Vector2(80f, 80f), new Vector2(8f, 8f), 1, prefabItem, UIListView.AxisType.horizontal, isUseItemInterface: true);
            listViewSkill.setValueCallback = ShowSkillItem;
            listViewSkill.ShowData(listSkill);
        }
        private void TestSkill() {

            ReplaceSkill("5001");
            ReplaceSkill("5005");
            //AddSkill("5002");
            //AddSkill("5003");
            //AddSkill("5004");
            //AddSkill("5005");
            //AddSkill("5005");
            //AddSkill("5006");
            //AddSkill("5007");
        }
        private void ShowSkillItem(GameObject prefabObject, object data, int dataIndex) {
            InfoHelper.BkgSkillInfo diaInfo = (InfoHelper.BkgSkillInfo)data;
            ItemBkgController itemCtl = prefabObject.GetComponent<ItemBkgController>();
            Sprite sprite = ResourceManager.LoadAsset("Textures", diaInfo.iconName, typeof(Sprite)) as Sprite;
            Debug.Assert(sprite != null);
            if (sprite != null) {
                itemCtl.Icon.sprite = sprite;
            }
            itemCtl.currInfo = diaInfo;
        }

        public int CountRightFiger = 0;
        // Update is called once per frame
        void Update()
        {
            CalcRightTouchFiger();
            ShowWeight();
        }

        public void CalcRightTouchFiger()
        {
            CountRightFiger = 0; //没帧重新计算。
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if ((Input.GetTouch(i).position.x / BackPackManager.mIntance.mCanvas.scaleFactor) >
                        (BackPackManager.mIntance.mCanvasScaler.referenceResolution.x / 2f))
                    {
                        CountRightFiger++;
                    }
                }
            }
        }
    }
}

