using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoHelper{
    public enum EnumList
    {
        GUN = 1,
        WEAR,
        DIAMOND,
        GRENADE,
        MEDICINE,
        SKILL
    }
    public enum EnumTypeDia
    {
        UNKNOWN=0,
        RANGE,
        RATE,
        HITRATE,
        POWER,
        BULLETCAPA,
        WEIGHT
    }
    public class BaseBkgInfo
    {
        public string id;
        public string name;
        public string iconName;
        public string description;
        public int weight;
        public string prefabName;
        public EnumList type;
    }
   
    public class BkgGunInfo: BaseBkgInfo
    {
        public List<BkgDiaInfo> listGunDias=new List<BkgDiaInfo>();
        public GunBaseInfo baseInfo;
        public BkgGunInfo(string id)
        {
            this.id = id;
            baseInfo=new GunBaseInfo(id);
            listGunDias.Clear();
            this.type = EnumList.GUN;
        }
        
    }
    public class BkgWearInfo : BaseBkgInfo
    {
        public int durable;//耐久


        public BkgWearInfo(string id,int durable)
        {
            this.id = id;
            this.durable = durable;
            this.iconName = InfoFromJson.mInstance.GetValueByKey(id, "icon", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.prefabName =InfoFromJson.mInstance.GetValueByKey(id, "prefabName", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.name = InfoFromJson.mInstance.GetValueByKey(id, "name", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.description = InfoFromJson.mInstance.GetValueByKey(id, "dec", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.weight = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "weight", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString());
            this.type = EnumList.WEAR;
        }
    }
    public class BkgDiaInfo : BaseBkgInfo
    {
        public EnumTypeDia typeDia;
        public int effectValue;
        public BkgDiaInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                this.weight = 0;
                this.iconName = "";
                this.name = "";
                this.typeDia = EnumTypeDia.UNKNOWN;
                return;
            }
            this.id = id;
            this.iconName = InfoFromJson.mInstance.GetValueByKey(id, "icon", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.prefabName = InfoFromJson.mInstance.GetValueByKey(id, "prefabName", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.name = InfoFromJson.mInstance.GetValueByKey(id, "name", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.description = InfoFromJson.mInstance.GetValueByKey(id, "dec", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.weight = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "weight", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString());
            string type= InfoFromJson.mInstance.GetValueByKey(id, "type", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            switch (type)
            {
                case "range"://射程
                    typeDia=EnumTypeDia.RANGE;
                    break;
                case "rate"://射速
                    typeDia = EnumTypeDia.RATE;
                    break;
                case "hitrate"://命中
                    typeDia = EnumTypeDia.HITRATE;
                    break;
                case "power"://威力
                    typeDia = EnumTypeDia.POWER;
                    break;
                case "bullet"://容弹量
                    typeDia = EnumTypeDia.BULLETCAPA;
                    break;
                case "weight"://便携
                    typeDia = EnumTypeDia.WEIGHT;
                    break;
            }
            this.effectValue= int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "effect_value", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString());
            this.type=EnumList.DIAMOND;
        }
    }

    public class BkgGrenadeInfo:BaseBkgInfo
    {
        public BkgGrenadeInfo(string id)
        {
            this.id = id;
            this.iconName = InfoFromJson.mInstance.GetValueByKey(id, "icon", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.prefabName = InfoFromJson.mInstance.GetValueByKey(id, "prefabName", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.name = InfoFromJson.mInstance.GetValueByKey(id, "name", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.description = InfoFromJson.mInstance.GetValueByKey(id, "dec", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.weight = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "weight", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString());
            this.type = EnumList.GRENADE;
        }
    }

    public class BkgMedicineInfo : BaseBkgInfo
    {
        public BkgMedicineInfo(string id)
        {
            this.id = id;
            this.iconName = InfoFromJson.mInstance.GetValueByKey(id, "icon", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.prefabName = InfoFromJson.mInstance.GetValueByKey(id, "prefabName", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.name = InfoFromJson.mInstance.GetValueByKey(id, "name", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.description = InfoFromJson.mInstance.GetValueByKey(id, "dec", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.weight = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "weight", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString());
            this.type = EnumList.MEDICINE;
        }
    }

    public class BkgSkillInfo : BaseBkgInfo
    {
        public BkgSkillInfo(string id)
        {
            this.id = id;
            this.iconName = InfoFromJson.mInstance.GetValueByKey(id, "icon", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.prefabName = InfoFromJson.mInstance.GetValueByKey(id, "prefabName", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.name = InfoFromJson.mInstance.GetValueByKey(id, "name", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.description = InfoFromJson.mInstance.GetValueByKey(id, "dec", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString();
            this.weight = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "weight", InfoFromJson.mInstance.propJsoncolumnName, InfoFromJson.mInstance.propJsonValuesDic).ToString());
            this.type = EnumList.SKILL;
        }
    }

}
