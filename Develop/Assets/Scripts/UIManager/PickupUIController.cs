using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PickupUIController : MonoBehaviour/*,IPointerEnterHandler*/{

    private Transform target;
    private string id;
    private GameObject willPickGo;
    private int durable;

    public Vector3 offset = new Vector3(1, 1, 0);

    public Button buttonPickUp;

    public Image icon;

    public Text TextName;

    public Text TextDes;

    void Awake()
    {
        buttonPickUp.onClick.AddListener(OnPickUpClick);
    }

    // Use this for initialization
    void Start()
    {

    }

    public void OnPickUpClick()
    {
        PropManager.mInstance.Give(id, willPickGo,this, durable);
        BackPackManager.mIntance.ClosePickUpUI();//拾取完关闭
    }

    public void ShowUI(GameObject go,string id, int durable)
    {
        target = go.transform;
        willPickGo = go;
        this.durable = durable;
        GameObject initGo = null;
        this.id = id;
        if (id.Contains("100"))//gun
        {
            InfoHelper.BkgGunInfo gunInfo = new InfoHelper.BkgGunInfo(id);
            Sprite sprite = ResourceManager.LoadAsset("Textures", gunInfo.baseInfo.iconName, typeof(Sprite)) as Sprite;
            icon.sprite = sprite;
            TextName.text = gunInfo.baseInfo.name;
            TextDes.text= gunInfo.baseInfo.dec;
        }
        else if (id.Contains("600"))//wear
        {
            InfoHelper.BkgWearInfo wearInfo = new InfoHelper.BkgWearInfo(id, durable);
            Sprite sprite = ResourceManager.LoadAsset("Textures", wearInfo.iconName, typeof(Sprite)) as Sprite;
            icon.sprite = sprite;
            TextName.text = wearInfo.name;
            TextDes.text = wearInfo.description;
        }
        else if (id.Contains("200"))//dia
        {
            InfoHelper.BkgDiaInfo diaInfo = new InfoHelper.BkgDiaInfo(id);
            Sprite sprite = ResourceManager.LoadAsset("Textures", diaInfo.iconName, typeof(Sprite)) as Sprite;
            icon.sprite = sprite;
            TextName.text = diaInfo.name;
            TextDes.text = diaInfo.description;
        }
        else if (id.Contains("300"))//grenage
        {
            InfoHelper.BkgGrenadeInfo grenageInfo = new InfoHelper.BkgGrenadeInfo(id);
            Sprite sprite = ResourceManager.LoadAsset("Textures", grenageInfo.iconName, typeof(Sprite)) as Sprite;
            icon.sprite = sprite;
            TextName.text = grenageInfo.name;
            TextDes.text = grenageInfo.description;
        }
        else if (id.Contains("400"))//medicine
        {
            InfoHelper.BkgMedicineInfo medicineInfo = new InfoHelper.BkgMedicineInfo(id);
            Sprite sprite = ResourceManager.LoadAsset("Textures", medicineInfo.iconName, typeof(Sprite)) as Sprite;
            icon.sprite = sprite;
            TextName.text = medicineInfo.name;
            TextDes.text = medicineInfo.description;
        }
        else if (id.Contains("500"))//skill
        {
            InfoHelper.BkgSkillInfo skillInfo = new InfoHelper.BkgSkillInfo(id);
            Sprite sprite = ResourceManager.LoadAsset("Textures", skillInfo.iconName, typeof(Sprite)) as Sprite;
            icon.sprite = sprite;
            TextName.text = skillInfo.name;
            TextDes.text = skillInfo.description;
        }
    }

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    OnPickUpClick();
    //}

    // Update is called once per frame
    //void Update()
    //{
    //    if (target != null)
    //    {
    //        transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
    //    }
    //}
}
