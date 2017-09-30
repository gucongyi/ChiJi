using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherHpController : MonoBehaviour {
    public static OtherHpController mInstance;
    public OtherHPFollow otherHpFollow;
    public PlayerHudTextController hudTextController;
    void Awake()
    {
        mInstance = this;
    }
    // Use this for initialization
    void Start () {
        OtherHpController.mInstance.hudTextController.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
