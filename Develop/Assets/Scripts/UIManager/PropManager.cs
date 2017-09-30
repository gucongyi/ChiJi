using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;

public class PropManager : MonoBehaviour
{
    public List<PropController> propList;

    public PhotonView thisPhoton;

    public static PropManager mInstance;

    void Awake()
    {
        mInstance = this;
        thisPhoton = PhotonView.Get(this);
    }

    // Use this for initialization
    void Start () {
        PhotonNetwork.automaticallySyncScene = true;
        BackPackManager.mIntance.RepalaceGun("1003");//默认一把手枪
	    if (PhotonNetwork.isMasterClient)//主机网络创建，其他人也能看到
        {
            foreach (var prop in propList)
            {
                GameObject initGo = null;
                if (prop.id.Contains("100"))//gun
                {
                    InfoHelper.BkgGunInfo gunInfo=new InfoHelper.BkgGunInfo(prop.id);
                    initGo=PhotonNetwork.Instantiate(gunInfo.baseInfo.prefabName, prop.transform.position,prop.transform.rotation,0);
                }
                else if(prop.id.Contains("600"))//wear
                {
                    InfoHelper.BkgWearInfo wearInfo = new InfoHelper.BkgWearInfo(prop.id,100);
                    initGo = PhotonNetwork.Instantiate(wearInfo.prefabName, prop.transform.position, prop.transform.rotation, 0);
                    initGo.GetComponent<PropController>().durable = 100;//耐久赋值
                }
                else if (prop.id.Contains("200"))//dia
                {
                    InfoHelper.BkgDiaInfo diaInfo = new InfoHelper.BkgDiaInfo(prop.id);
                    initGo = PhotonNetwork.Instantiate(diaInfo.prefabName, prop.transform.position, prop.transform.rotation, 0);
                }
                else if (prop.id.Contains("300"))//grenage
                {
                    InfoHelper.BkgGrenadeInfo grenageInfo = new InfoHelper.BkgGrenadeInfo(prop.id);
                    initGo = PhotonNetwork.Instantiate(grenageInfo.prefabName, prop.transform.position, prop.transform.rotation, 0);
                }
                else if (prop.id.Contains("400"))//medicine
                {
                    InfoHelper.BkgMedicineInfo medicineInfo = new InfoHelper.BkgMedicineInfo(prop.id);
                    initGo = PhotonNetwork.Instantiate(medicineInfo.prefabName, prop.transform.position, prop.transform.rotation, 0);
                }
                else if (prop.id.Contains("500"))//skill
                {
                    InfoHelper.BkgSkillInfo skillInfo = new InfoHelper.BkgSkillInfo(prop.id);
                    initGo = PhotonNetwork.Instantiate(skillInfo.prefabName, prop.transform.position, prop.transform.rotation, 0);
                }
                initGo.GetComponent<PropController>().id = prop.id;//id赋值

            }
	    }

	}


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            if (PhotonNetwork.isMasterClient)
            {
            }
        }
        else if (stream.isReading)
        {
            
        }
    }

    public void Give(string id, GameObject willPickObj,PickupUIController pickUpUI,int durabale=100)
    {
        if (id.Contains("100"))//gun
        {
            string oldId=BackPackManager.mIntance.RepalaceGun(id);
            InfoHelper.BkgGunInfo gunInfo = new InfoHelper.BkgGunInfo(oldId);
            GameObject initGo = PhotonNetwork.Instantiate(gunInfo.baseInfo.prefabName, willPickObj.transform.position, willPickObj.transform.rotation, 0);
            initGo.GetComponent<PropController>().id = oldId;//id赋值
            BackPackManager.mIntance.ShowGun();//显示背包中信息
            pickUpUI.ShowUI(initGo, oldId,0);//显示刚替换下来的
        }
        else if (id.Contains("600"))//wear
        {
            BackPackManager.WearOldInfo oldInfo=BackPackManager.mIntance.RepalaceWear(id,durabale);
            BackPackManager.mIntance.ShowWear();
            if (oldInfo!=null)
            {
                InfoHelper.BkgWearInfo wearInfo = new InfoHelper.BkgWearInfo(oldInfo.wearId, oldInfo.durable);
                GameObject initGo = PhotonNetwork.Instantiate(wearInfo.prefabName, willPickObj.transform.position, willPickObj.transform.rotation, 0);
                initGo.GetComponent<PropController>().id = wearInfo.id;//id赋值
                initGo.GetComponent<PropController>().durable = wearInfo.durable;//id赋值
                pickUpUI.ShowUI(initGo, wearInfo.id, wearInfo.durable);//显示刚替换下来的
            }
        }
        else if (id.Contains("200"))//dia
        {
            BackPackManager.mIntance.AddDiamond(id);
            BackPackManager.mIntance.ShowDiamond();
        }
        else if (id.Contains("300"))//grenage
        {
            BackPackManager.mIntance.AddGreade(id);
            BackPackManager.mIntance.ShowGreade();
        }
        else if (id.Contains("400"))//medicine
        {
            BackPackManager.mIntance.AddMedicine(id);
            BackPackManager.mIntance.ShowMedicine();
        }
        else if (id.Contains("500"))//skill
        {
            BackPackManager.mIntance.ReplaceSkill(id);
            BackPackManager.mIntance.ShowSkill();
        }
        PhotonNetwork.Destroy(willPickObj);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
