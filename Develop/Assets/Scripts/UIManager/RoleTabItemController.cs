using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleTabItemController : MonoBehaviour
{
    public GameObject goBg;
    public GameObject goSelected;
    public LobbyController.SelectRoleType thisToggleSelectType;

    public void SetIsOn(bool isOn)//Reset时候用
    {
        if (isOn)
        {
            LobbyController.mInstance.ShowRole(thisToggleSelectType);
            goSelected.SetActive(true);
            goBg.SetActive(false);
        }
        else
        {
            goSelected.SetActive(false);
            goBg.SetActive(true);
        }
    }

    public void OnToggleValueChanged(bool isToggle)
    {
        if (isToggle)
        {
            if (LobbyController.mInstance.CurrRoleType == thisToggleSelectType)
            {
                return; //没变过，不触发操作
            }
        }
        if (isToggle)
        {
            LobbyController.mInstance.ShowRole(thisToggleSelectType);
            goSelected.SetActive(true);
            goBg.SetActive(false);
        }
        else
        {
            goSelected.SetActive(false);
            goBg.SetActive(true);
        }
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
