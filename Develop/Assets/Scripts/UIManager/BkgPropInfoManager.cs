using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BkgPropInfoManager : MonoBehaviour
{
    public Image Tcon;
    public Text TextName;
    public Text TextDes;
    public Text TextWeight;
    public void ShowPropInfoUI(InfoHelper.BaseBkgInfo currInfo)
    {
        if (currInfo!=null)
        {
            Tcon.sprite=  ResourceManager.LoadAsset("Textures", currInfo.iconName, typeof(Sprite)) as Sprite;
            TextName.text = currInfo.name;
            TextDes.text = currInfo.description;
            TextWeight.text = currInfo.weight+"";
        }
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
