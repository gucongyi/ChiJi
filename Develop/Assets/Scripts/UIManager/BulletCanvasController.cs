using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletCanvasController : MonoBehaviour
{
    public Text TextBullet;

    public Slider ReloadSlider;

    public Text TextTime;

    [HideInInspector] public float TimeReload;
    private float timeoffset;//防止切回来没显示满
    private bool isReload;
    private float animTotalTime;
    public void Reload(float animTime)
    {
        isReload = true;
        animTotalTime = animTime;
        TimeReload = animTime;
        TextBullet.gameObject.SetActive(false);
        ReloadSlider.gameObject.SetActive(true);
    }

    // Use this for initialization
	void Start ()
	{
	    isReload = false;
	    timeoffset = 0.004f;
    }
	
	// Update is called once per frame
	void Update () {
	    if (isReload)
	    {
	        TimeReload -= (Time.deltaTime-timeoffset);
	        TextTime.text = string.Format("{0:##0.0}", TimeReload);
	        ReloadSlider.value = TimeReload / animTotalTime;
            if (TimeReload<0f)
	        {
	            isReload = false;
	            TextBullet.gameObject.SetActive(true);
	            ReloadSlider.gameObject.SetActive(false);
            }
	    }

	}
}
