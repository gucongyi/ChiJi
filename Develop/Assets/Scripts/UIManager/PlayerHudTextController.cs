using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudTextController : MonoBehaviour
{
    public Slider BgSlider;

    public Slider ChangeSlider;

    public Slider FgSlider;
    

    private float currHpValue;
    private float currHp = 0f;
    private const float defalutChangeSliderEulerZ = 31.09f;
    public const float radio = 0.667f;//120度的弧，180的做法
    private bool isIncrease;//区分开两个事件
    [HideInInspector]
    public bool isCompelete;

    public void Init()
    {
        BgSlider.value = radio;
        FgSlider.value = radio;
        ChangeSlider.fillRect.localEulerAngles = new Vector3(ChangeSlider.fillRect.localEulerAngles.x, ChangeSlider.fillRect.localEulerAngles.y, defalutChangeSliderEulerZ + 120f);
        ChangeSlider.value = radio;
        currHpValue = radio;
        Debug.Log("radio:"+ radio);
        Debug.Log("currHpValue:" + currHpValue);
        Debug.Log("ChangeSlider.fillRect.localEulerAngles:(" + ChangeSlider.fillRect.localEulerAngles.x+","+ ChangeSlider.fillRect.localEulerAngles.y+","+ ChangeSlider.fillRect.localEulerAngles.z+")");

    }

    public void ResetHp(Character lockCharacter)
    {
        float hudValue = lockCharacter.hp / lockCharacter.maxHP;
        BgSlider.value = radio* hudValue;
        FgSlider.value = radio * hudValue;
        ChangeSlider.fillRect.localEulerAngles = new Vector3(ChangeSlider.fillRect.localEulerAngles.x, ChangeSlider.fillRect.localEulerAngles.y, defalutChangeSliderEulerZ + (radio * hudValue)*120f / radio);
        ChangeSlider.value = radio * hudValue;
        currHpValue = radio * hudValue;

    }

    public void IncreaseToHp(float hpValue)
    {
        if (isCompelete == false) return;//正在执行
        hpValue =limitCurrValue(hpValue);
        hpValue = hpValue * radio;
        isIncrease = true;

        float delta = (hpValue - currHpValue);
        ChangeSlider.value = delta*110f/100;//延长一点，确保能够接上
        currDelta = delta;
        //先旋转
        ChangeSlider.fillRect.localEulerAngles = new Vector3(ChangeSlider.fillRect.localEulerAngles.x, ChangeSlider.fillRect.localEulerAngles.y, ChangeSlider.fillRect.localEulerAngles.z + currDelta * 120f / radio);

        //ChangeSlider.value = radio - currHpValue;//反向的
        currHpValue = hpValue;
        
        limitCurrValue();
        isCompelete = false;
        DOTweenAnimation[] changeAnims = ChangeSlider.fillRect.GetComponents<DOTweenAnimation>();
        foreach (var changeAnim in changeAnims)
        {
            if (changeAnim.id.Contains("fade00"))
            {
                changeAnim.DORestart(true);//下次可以继续播放，否则播不出来
                changeAnim.DOPlayForward();
            }
        }
    }

    public void OnBgSliderChange(float value)
    {
        //unity自己bug,不停Input走动导致的
        limitCurrValue();
        BgSlider.value = currHpValue;
    }

    public void OnIncreaseToHpAnimComplete()
    {//变化层播放完成
        if (isIncrease)
        {
            BgSlider.value = currHpValue;
            FgSlider.value = currHpValue;
            isCompelete = true;
        }
    }

    private void limitCurrValue()
    {
        if (currHpValue>radio)
        {
            currHpValue = radio;
        }
        else if (currHpValue < 0f)
        {
            currHpValue = 0f;
        }
    }

    private float limitCurrValue(float hpValue)
    {
        if (hpValue > 1)
        {
            hpValue = 1;
        }
        else if (hpValue < 0f)
        {
            hpValue = 0f;
        }
        return hpValue;
    }

    private float currDelta;
    public void DecreaseToHp(float hpValue)
    {
        if(isCompelete==false)return;//正在执行
        hpValue=limitCurrValue(hpValue);
        hpValue = hpValue * radio;
        isIncrease = false;
        //ChangeSlider.value = radio-currHpValue;//从当前值亮一下 radio-hpValue;//反向的
        //闪的那一层的变化量
        float delta = currHpValue - hpValue;
        ChangeSlider.value = delta;
        currDelta = delta;

        currHpValue = hpValue;
        limitCurrValue();
        isCompelete = false;
        FgSlider.value = currHpValue;//前层直接下来
        DOTweenAnimation[] changeAnims = ChangeSlider.fillRect.GetComponents<DOTweenAnimation>();
        foreach (var changeAnim in changeAnims)
        {
            if (changeAnim.id.Contains("fade20"))
            {
                changeAnim.DORestart(true);//下次可以继续播放，否则播不出来
                changeAnim.DOPlayForward();
            }
        }
    }

    public void OnDecreaseToHpAnimComplete()
    {//变化层播放完成
        if (!isIncrease)
        {
            //ChangeSlider.value = radio-currHpValue;//反向的
            BgSlider.DOValue(currHpValue, 1f);//后层渐变
            isCompelete = true;
            //闪的那一层的变化量旋转角度
            ChangeSlider.fillRect.localEulerAngles = new Vector3(ChangeSlider.fillRect.localEulerAngles.x, ChangeSlider.fillRect.localEulerAngles.y, ChangeSlider.fillRect.localEulerAngles.z - currDelta * 120f / radio);
            
        }
    }

    // Use this for initialization
    void Awake()
    {
        isCompelete = true;
    }

    // Update is called once per frame
    void Update () {
	    if (isCompelete)
	    {
	        //防止动画没有到最终效果
	        Color oldColor = ChangeSlider.fillRect.GetComponent<Image>().color;
	        ChangeSlider.fillRect.GetComponent<Image>().color = new Color(oldColor.r, oldColor.g, oldColor.b, 0f);
        }
        ////test
        //if (!isCompelete) return;
        //currHp = limitCurrValue(currHp);

        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    currHp += 0.05f;
        //    IncreaseToHp(currHp);
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    currHp -= 0.1f;
        //    DecreaseToHp(currHp);
        //}

    }
}
