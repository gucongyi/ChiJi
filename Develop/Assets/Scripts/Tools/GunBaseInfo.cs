using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBaseInfo
{
    public class Range
    {
        public int distance;
        public int damage;

        public Range(int distance,int damage)
        {
            this.distance = distance;
            this.damage = damage;
        }
    }
    public string id;
    public string iconName;
    public string prefabName;
    public string name;
    public string dec;
    public int rate;
    public List<Range> rangeList=new List<Range>();
    public int hitRate;
    public int weight;
    public int bulletCapacity;
    public float velReload;
    public int uiRange;
    public int uiPower;

    public GunBaseInfo(string id)
    {
        this.id = id;
        this.iconName = InfoFromJson.mInstance.GetValueByKey(id, "icon", InfoFromJson.mInstance.weaponJsoncolumnName,InfoFromJson.mInstance.weaponJsonValuesDic).ToString();
        this.prefabName= InfoFromJson.mInstance.GetValueByKey(id, "prefabName", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString();
        this.name= InfoFromJson.mInstance.GetValueByKey(id, "name", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString();
        this.dec = InfoFromJson.mInstance.GetValueByKey(id, "dec", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString();
        this.rate= int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "firing_rate", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString());
        string range= InfoFromJson.mInstance.GetValueByKey(id, "range", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString();
        var listFormJson=range.Split(';');
        rangeList.Clear();
        foreach (var eachRange in listFormJson)
        {
            var rangeFields=eachRange.Split(',');
            Range each=new Range(int.Parse(rangeFields[0]) , int.Parse(rangeFields[1]));
            rangeList.Add(each);
        }
        this.hitRate = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "hit_rate", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString());
        this.weight = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "weight", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString());
        this.bulletCapacity = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "bomb_capacity", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString());
        this.velReload = float.Parse(InfoFromJson.mInstance.GetValueByKey(id, "Reload", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString());
        this.uiRange = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "show_range", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString());
        this.uiPower = int.Parse(InfoFromJson.mInstance.GetValueByKey(id, "show_power", InfoFromJson.mInstance.weaponJsoncolumnName, InfoFromJson.mInstance.weaponJsonValuesDic).ToString());
    }
}
