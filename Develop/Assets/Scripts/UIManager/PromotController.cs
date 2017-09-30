using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotController : MonoBehaviour {
    public Canvas canvas;
    public Text ToastText;
    private bool isShowToast;
    private float DurTime = 1f;

    public void ShowToast()
    {
        isShowToast = true;
        gameObject.SetActive(true);
    }
    public void ShowToast(string content)
    {
        isShowToast = true;
        gameObject.SetActive(true);
        ToastText.text = content;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (isShowToast)
	    {
	        DurTime -= Time.deltaTime;
	        if (DurTime<=0f)
	        {
	            DurTime = 1f;
	            gameObject.SetActive(false);
	            isShowToast = false;
            }
	    }

	}
}
