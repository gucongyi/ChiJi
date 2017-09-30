using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;

public class SectorCheck : MonoBehaviour
{
    public float distanceCheck;
    public const string PropTag = "Prop";
    public float angle;
    private float colliderWidth = 0f;
    private float colliderLength = 0f;

    private BoxCollider CheckCollider;
    public Transform SelfTrans;
    public List<Collider> triggerColliders=new List<Collider>();
    private bool isTrigger;

    void Awake()
    {
        triggerColliders.Clear();
        CheckCollider = GetComponent<BoxCollider>();
        colliderWidth = 2 * distanceCheck * Mathf.Sin(angle * Mathf.Deg2Rad);
        colliderLength = distanceCheck;
        CheckCollider.transform.localPosition = new Vector3(CheckCollider.transform.localPosition.x, CheckCollider.transform.localPosition.y,distanceCheck / 2f);
        CheckCollider.center = Vector3.zero;
        CheckCollider.size = new Vector3(colliderWidth, 2, colliderLength);
    }

    private Vector3 SelfNormalize(Vector3 point)
    {
        Vector3 newPoint=new Vector3(point.x,0,point.z);
        float distance = Vector3.Distance(newPoint, Vector3.zero);
        Vector3 norVec = newPoint / distance;
        return norVec;
    }

    private bool CheckIsInSector(GameObject otherGo)
    {
        Vector3 otherWorldPos = otherGo.transform.position;
        otherWorldPos=new Vector3(otherWorldPos.x,0, otherWorldPos.z);
        Vector3 selfWorldPos = SelfTrans.position;
        selfWorldPos=new Vector3(selfWorldPos.x,0, selfWorldPos.z);
        Vector3 forward = SelfTrans.forward;
        Vector3 forwardNormalize = SelfNormalize(forward);
        Vector3 otherDir = otherWorldPos - selfWorldPos;
        Vector3 otherDirNormalize = SelfNormalize(otherDir);//不计算y
        float cosGetAngle = Vector3.Dot(forwardNormalize, otherDirNormalize);
        float getAngle = Mathf.Acos(cosGetAngle)*Mathf.Rad2Deg;
        if(DrawCicle.mInstance.isDebugLog)
            Debug.Log("GetAngle:"+ getAngle+ " forwardNormalize:"+forwardNormalize+ "otherDirNormalize:"+ otherDirNormalize+ " selfWorldPos:" + selfWorldPos + " SelfTrans.localPosition:"+ SelfTrans.localPosition+ " otherGo.transform.position:"+ otherGo.transform.position);
        if (Mathf.Abs(getAngle) > Mathf.Abs(angle)) //在角度之外。
        {
            return false;
        }
        else
        {
            float distance = Vector3.Distance(otherWorldPos, selfWorldPos);
            if (distance<= distanceCheck)
            {//在扇形范围之内
                return true;
            }
        }
        return false;
    }

    // Use this for initialization
	void Start ()
	{
	    triggerColliders.Clear();
    }

    public void CloseCheckWhenBurrowIn()
    {
        CheckCollider.enabled = false;
        BackPackManager.mIntance.ClosePickUpUI();
        triggerColliders.Clear();
    }

    public void OpenCheckWhenBurrowOut()
    {
        CheckCollider.enabled = true;
        triggerColliders.Clear();
    }

    void FixedUpdate()
    {
        if (isTrigger)
        {
            if (triggerColliders.Count > 0)
            {
                GameObject goMin = GetMinDisProp();
                if (goMin != null)
                {
                    BackPackManager.mIntance.SetPickUpUI(goMin, goMin.GetComponent<PropController>().id, goMin.GetComponent<PropController>().durable);
                }
            }
            else
            {
                BackPackManager.mIntance.ClosePickUpUI();
            }
        }
        if (isTrigger)
        {
            triggerColliders.Clear();
            isTrigger = false;
        }
        
    }

    void OnTriggerEnter(Collider other)
    {//父对象必须是钢体，刚体不要和Collider放在同一层级，会一一检测碰撞器，所有碰撞器检测完了在执行Update.
        if (other == null)//拾取完
        {
            return;
        }
        if (!SelfTrans.GetComponent<PhotonView>().isMine)
        {//多个公用一个拾取UI问题
            return;
        }
        if (other.gameObject.tag== PropTag)
        {
            if (CheckIsInSector(other.gameObject))
            {
                //Debug.Log(other.name + " is in Sector");
                if (!triggerColliders.Contains(other))
                    triggerColliders.Add(other);
                //BackPackManager.mIntance.SetPickUpUI(other.gameObject,other.gameObject.GetComponent<PropController>().id, other.gameObject.GetComponent<PropController>().durable);
            }
            //Debug.Log(other.name + " is Enter");
        }
        
    }
    void OnTriggerStay(Collider other)
    {
        if (other==null)//拾取完
        {
            return;
        }
        if (!SelfTrans.GetComponent<PhotonView>().isMine)
        {//多个公用一个拾取UI问题
            return;
        }
        if (other.gameObject.tag == PropTag)
        {
            isTrigger = true;
            if (CheckIsInSector(other.gameObject))
            {
                if (!triggerColliders.Contains(other))
                {
                    //Debug.Log(other.name + " is in Sector");
                    triggerColliders.Add(other);
                    //BackPackManager.mIntance.SetPickUpUI(other.gameObject, other.gameObject.GetComponent<PropController>().id, other.gameObject.GetComponent<PropController>().durable);
                }
            }
            else
            {
                if (triggerColliders.Contains(other))
                {//在触发器里，但是不在扇形范围内
                    triggerColliders.Remove(other);
                    //BackPackManager.mIntance.ClosePickUpUI();
                }
            }
        }
        //if (triggerColliders.Count > 0)
        //{
        //    foreach (var eachCollider in triggerColliders)
        //    {
        //        Debug.Log(eachCollider.name + " is in Sector");
        //    }
        //}
    }
    void OnTriggerExit(Collider other)
    {
        if (other == null)//拾取完
        {
            return;
        }
        if (!SelfTrans.GetComponent<PhotonView>().isMine)
        {//多个公用一个拾取UI问题
            return;
        }
        if (other.gameObject.tag == PropTag)
        {
            if (triggerColliders.Contains(other))
            {
                triggerColliders.Remove(other);
                //BackPackManager.mIntance.ClosePickUpUI();
            }
            Debug.Log(other.name + " is Exit");
        }
    }

    private GameObject GetMinDisProp()
    {
        if (triggerColliders.Count > 0)
        {
            List<float> distances = new List<float>();
            distances.Clear();
            for (int i = 0; i < triggerColliders.Count; i++)
            {
                if (triggerColliders[i] == null) //被捡走了，丢失引用异常
                {
                    triggerColliders.RemoveAt(i);
                    continue;
                }
                distances.Add(Vector3.Distance(SelfTrans.position, triggerColliders[i].gameObject.transform.position));
            }

            int min = 0;
            if (distances.Count == 1)
            {
                min = 0;
            }
            else if (distances.Count > 1)
            {
                min = 0;
                for (int j = 1; j < distances.Count; j++)
                {
                    if (distances[j] < distances[min])
                    {
                        min = j;
                    }
                }
            }
            if (triggerColliders.Count > 0)
            {
                return triggerColliders[min].gameObject;
            }
        }
        return null;
    }
    // Update is called once per frame
    void Update () {
        if (DrawCicle.mInstance==null)return;
        if (DrawCicle.mInstance.isDebugLog == false) return;
         Quaternion r = SelfTrans.rotation;
	    Vector3 f0 = (SelfTrans.position + (r * Vector3.forward) * distanceCheck);
	    Debug.DrawLine(SelfTrans.position, f0, Color.red);

	    Quaternion r0 = Quaternion.Euler(SelfTrans.rotation.eulerAngles.x, SelfTrans.rotation.eulerAngles.y - angle, SelfTrans.rotation.eulerAngles.z);
	    Quaternion r1 = Quaternion.Euler(SelfTrans.rotation.eulerAngles.x, SelfTrans.rotation.eulerAngles.y + angle, SelfTrans.rotation.eulerAngles.z);

	    Vector3 f1 = (SelfTrans.position + (r0 * Vector3.forward) * distanceCheck);
	    Vector3 f2 = (SelfTrans.position + (r1 * Vector3.forward) * distanceCheck);

	    Debug.DrawLine(SelfTrans.position, f1, Color.red);
	    Debug.DrawLine(SelfTrans.position, f2, Color.red);
	    for (int i=(int)(2-angle);i< angle;i+=2) {
	        Quaternion rEach= Quaternion.Euler(SelfTrans.rotation.eulerAngles.x, SelfTrans.rotation.eulerAngles.y +i, SelfTrans.rotation.eulerAngles.z);
	        Vector3 fEach = (SelfTrans.position + (rEach * Vector3.forward) * distanceCheck);
	        Debug.DrawLine(SelfTrans.position, fEach, Color.red);
        }

	    Debug.DrawLine(f0, f1, Color.red);
	    Debug.DrawLine(f0, f2, Color.red);
    }
}
