using UnityEngine;

public class OtherHPFollow : MonoBehaviour
{
    public Transform target;
    [HideInInspector] public Transform mySelf;
    private Vector3 positionOffset = new Vector3(-80f, 10f, 0f);
    [HideInInspector]
    public float LockDistance;
    public Canvas canvas;

    private float CalcSliderScale()
    {
        float scale = 0.5f;//最大0.5f
        float distance = Vector3.Distance(mySelf.position, target.position);
        scale = (distance * 0.3f / LockDistance)+0.2f;//变化量0.3f 0.5-0.2
        return scale;
    }

    private void Update() {
        if (target)
        {

            if (mySelf == null)return;
            Vector3 targetPos=new Vector3(target.position.x, target.position.y+0.5f, target.position.z);//因为轴心点在脚底，所以提升半个身位
            transform.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(targetPos) / canvas.scaleFactor + positionOffset;//自己的局部坐标是本地屏幕坐标
            transform.GetComponent<RectTransform>().localScale = CalcSliderScale()*Vector3.one;
        }
    }
}
