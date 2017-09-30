using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;
using UnityEngine.UI;

public class BkgWearUIManager : MonoBehaviour
{
    public const float totalDurable = 100;//耐久
    public Text TextCurrDuarabl;
    public Text TextTotalDuarabl;
    public Image ImageWear;

    public void ShowWearInfo(InfoHelper.BkgWearInfo info)
    {
        if(info==null)return;
        TextCurrDuarabl.text = info.durable+"";
        TextTotalDuarabl.text = totalDurable + "";
        var sprite = ResourceManager.LoadAsset("Textures", info.iconName, typeof(Sprite)) as Sprite;
        ImageWear.sprite = sprite;
    }
    
    
}
