using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoot : MonoBehaviour
{
    public static UIRoot mInstance;
    public Camera UiCamera;
    [HideInInspector]
    public GameObject promotGo;
    [HideInInspector]
    public GameObject LoadingBattleGo;
    [HideInInspector]
    public Slider LoadingBattleSlider;

    public void EnableLoading()
    {
        if (LoadingBattleGo)
        {
            LoadingBattleGo.SetActive(true);
        }
    }
    public void DisableLoading()
    {
        if (LoadingBattleGo)
        {
            LoadingBattleGo.SetActive(false);
        }
    }

    void Awake()
    {
        if (mInstance == null)
        {
            mInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (mInstance != this)
                Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
