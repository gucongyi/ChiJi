using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;
using UnityEngine.UI;

public class BkgGunUIManager : MonoBehaviour
{
    public const float totalSlider = 100;
    public Text TextGunName;
    public Image ImageGun;
    public List<Image> ImageDias;
    public Text TextGunInfoName;
    public Slider SliderRange;
    public List<Image> ImageDiasRange;
    public Slider SliderRate;
    public List<Image> ImageDiasRate;
    public Slider SliderHitRate;
    public List<Image> ImageDiasHitRate;
    public Slider SliderPower;
    public List<Image> ImageDiasPower;
    public Slider SliderBulletCapacity;
    public List<Image> ImageDiasBulletCapacity;
    public Slider SliderWeight;
    public List<Image> ImageDiasBulletWeight;

    public void ShowGunInfo(InfoHelper.BkgGunInfo info)
    {
        TextGunName.text = info.baseInfo.name;
        TextGunInfoName.text = info.baseInfo.name;
        var sprite = ResourceManager.LoadAsset("Textures", info.baseInfo.iconName, typeof(Sprite)) as Sprite;
        ImageGun.sprite = sprite;
        ImageGun.SetNativeSize();
        for (int i = 0; i < 3; i++)
        {
            //显示枪的宝石图标
            Sprite spriteDia = null;
            if (string.IsNullOrEmpty(info.listGunDias[i].iconName))
            {
                spriteDia = ResourceManager.LoadAsset("Textures", "BackPackDiaFrame", typeof(Sprite)) as Sprite;
            }
            else
            {
                spriteDia = ResourceManager.LoadAsset("Textures", info.listGunDias[i].iconName, typeof(Sprite)) as Sprite;
            }
            ImageDias[i].sprite = spriteDia;
        }
    }

    public void ShowInfoDia(InfoHelper.BkgGunInfo info,Dictionary<InfoHelper.EnumTypeDia, BackPackManager.DiaGunInfo> dicGunDia)
    {
        int rangeValue = info.baseInfo.uiRange;
        int rateValue = info.baseInfo.rate;
        int hitRateValue = info.baseInfo.hitRate;
        int powerValue = info.baseInfo.uiPower;
        int bulletCapaValue = info.baseInfo.bulletCapacity;
        int weightValue = info.baseInfo.weight;
        CloseAddListImage();
        if (dicGunDia.Count > 0)
        {
            foreach (var eachDia in dicGunDia)
            {
                BackPackManager.DiaGunInfo infoDia = dicGunDia[eachDia.Key];

                switch (eachDia.Key)
                {
                     case InfoHelper.EnumTypeDia.RANGE:
                         rangeValue=SetGunInfoDia(infoDia,ImageDiasRange, info.baseInfo.uiRange);
                         break;
                    case InfoHelper.EnumTypeDia.RATE:
                        rateValue=SetGunInfoDia(infoDia, ImageDiasRate, info.baseInfo.rate);
                        break;
                    case InfoHelper.EnumTypeDia.HITRATE:
                        hitRateValue=SetGunInfoDia(infoDia, ImageDiasHitRate, info.baseInfo.hitRate);
                        break;
                    case InfoHelper.EnumTypeDia.POWER:
                        powerValue=SetGunInfoDia(infoDia, ImageDiasPower, info.baseInfo.uiPower);
                        break;
                    case InfoHelper.EnumTypeDia.BULLETCAPA:
                        bulletCapaValue=SetGunInfoDia(infoDia, ImageDiasBulletCapacity, info.baseInfo.bulletCapacity);
                        break;
                    case InfoHelper.EnumTypeDia.WEIGHT:
                        weightValue=SetGunInfoDia(infoDia, ImageDiasBulletWeight,info.baseInfo.weight);
                        break;
                }
            }
        }

        SliderRange.value = (float)rangeValue / totalSlider;
        SliderRate.value = (float)rateValue / 1000f;
        SliderHitRate.value = (float)hitRateValue / totalSlider;
        SliderPower.value = (float)powerValue / totalSlider;
        SliderBulletCapacity.value = (float)bulletCapaValue / totalSlider;
        SliderWeight.value = (float)weightValue / totalSlider;

    }

    private void CloseListImage(IList<Image> whichImageList)
    {
        foreach (var image in whichImageList)
        {
            image.gameObject.SetActive(false);
        }
    }

    private void CloseAddListImage()
    {
        CloseListImage(ImageDiasRange);
        CloseListImage(ImageDiasRate);
        CloseListImage(ImageDiasHitRate);
        CloseListImage(ImageDiasPower);
        CloseListImage(ImageDiasBulletCapacity);
        CloseListImage(ImageDiasBulletWeight);
    }

    private int SetGunInfoDia(BackPackManager.DiaGunInfo infoDia,IList<Image> whichImageList,int souceValue)
    {
        for (int i = 0; i < infoDia.Count; i++)
        {
            whichImageList[i].gameObject.SetActive(true);
            var sprite = ResourceManager.LoadAsset("Textures", infoDia.iconNameList[i], typeof(Sprite)) as Sprite;
            whichImageList[i].sprite = sprite;
            souceValue += infoDia.effectValue;
        }
        return souceValue;
    }
}
