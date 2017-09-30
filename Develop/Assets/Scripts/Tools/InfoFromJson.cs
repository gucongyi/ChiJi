using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class InfoFromJson:MonoBehaviour
{
    public static InfoFromJson mInstance;
    public TextAsset WeaponJsonFile;
    public List<string> weaponJsoncolumnName=new List<string>();
    public List<string> weaponJsonTypeName = new List<string>();
    public Dictionary<string,object> weaponJsonValuesDic = new Dictionary<string, object>();

    public TextAsset propJsonFile;
    public List<string> propJsoncolumnName = new List<string>();
    public List<string> propJsonTypeName = new List<string>();
    public Dictionary<string, object> propJsonValuesDic = new Dictionary<string, object>();

    void Awake()
    {
        mInstance = this;
        weaponJsoncolumnName.Clear();
        weaponJsonTypeName.Clear();
        weaponJsonValuesDic.Clear();

        propJsoncolumnName.Clear();
        propJsonTypeName.Clear();
        propJsonValuesDic.Clear();

        ParseJson(WeaponJsonFile, weaponJsoncolumnName, weaponJsonTypeName, weaponJsonValuesDic);
        ParseJson(propJsonFile, propJsoncolumnName, propJsonTypeName, propJsonValuesDic);
    }

    public  void ParseJson(TextAsset JsonFile, List<string> whichJsoncolumnName, List<string> whichJsonTypeName, Dictionary<string, object> whichJsonValuesDic)
    {
        string jsonString = JsonFile.text;
        JsonData jsonObjects = JsonMapper.ToObject(jsonString);//有多少个对象
        if (jsonObjects.Count>3)
        {//所有列名
            foreach (var key in jsonObjects[0].Keys)
            {//得到key
                //Debug.Log(key);
                whichJsoncolumnName.Add(key);
            }
            for (int i=0;i<whichJsoncolumnName.Count;i++)
            {//得到type
                string key = whichJsoncolumnName[i];
                whichJsonTypeName.Add(jsonObjects[1][key].ToString());//除了第一列之后的
            }
        }
        for (int row = 2; row < jsonObjects.Count; row++)
        {
            whichJsonValuesDic.Add(jsonObjects[row][whichJsoncolumnName[1]].ToString(), jsonObjects[row]);//id为键
        }
        Debug.Log("Parse Success");
        //string dec=GetValueByKey("1001","dec", weaponJsoncolumnName, weaponJsonValuesDic).ToString();
        //Debug.Log("dec:"+ dec);
    }

    public  object GetValueByKey(string id,string key, List<string> whichJsoncolumnName, Dictionary<string, object> whichJsonValuesDic)
    {
        object valueGet=null;
        object valueJson = null;
        if (whichJsonValuesDic.TryGetValue(id, out valueJson))
        {
            JsonData data = (JsonData) valueJson;
            if (whichJsoncolumnName.Contains(key))
            {
                valueGet = data[key];
            }
            else
            {
                Debug.Assert(whichJsoncolumnName.Contains(key));
            }
        }
        else
        {
            Debug.LogError("Dic not id:"+id);
        }

        return valueGet;
    }
}
