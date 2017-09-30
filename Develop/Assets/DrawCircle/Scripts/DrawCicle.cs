using System;
using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Random = UnityEngine.Random;
[RequireComponent(typeof(PhotonView))]
public class DrawCicle : MonoBehaviour, IPunObservable
{
    public static DrawCicle mInstance;
    public const float scaleRadiusToLocalPostionMi = 1f;
    public const float radiusCenterPointCoefficient = 0.8f;//防止相切
    private bool isGetMat = false;
    private float eachSecond = 1f;
    //private float calcArrowColorTime = 0.1f;
    public bool isDebugLog = false;

    private float CurrOutCircleDamage;

    private bool isShowDesCircle = false;
    private bool isShowMoveAndScaleCircle = false;
    private Circle sourceCircleInfo;
    private Circle desCircleInfo;
    private Circle moveCircleInfo;

    public PlayerArrowController Player;
    public class Circle
    {
        public float radius;
        public Vector3 centerPoint;

        public Circle(float radius, Vector3 centerPoint)
        {
            this.radius = radius;
            this.centerPoint = centerPoint;
        }

    }
    

    public List<CircleMoveInfo> circleMoveInfoList;
    public GameObject InitSourceCircle;

    public GameObject initDesCircle;

    public GameObject moveScaleCircle;

    void Awake()
    {
        mInstance = this;
        InitSourceCircle.SetActive(false);
        initDesCircle.SetActive(false);
        moveScaleCircle.SetActive(false);
        if(isDebugLog)
            Debug.Log("scaleRadiusToLocalPostionMi:" + scaleRadiusToLocalPostionMi);
    }

    // Use this for initialization
    void Start ()
	{
	    Circle sourceDir=new Circle(250,Vector3.zero);
	    sourceCircleInfo = sourceDir;
	    moveCircleInfo = sourceCircleInfo;
        DrawCircle(InitSourceCircle, sourceCircleInfo);
	    DrawCircle(moveScaleCircle, sourceCircleInfo);
	    moveScaleCircle.SetActive(true);
	    //InitSourceCircle.SetActive(true);
	}

    private void DrawCircle(GameObject circleGo,Circle circleInfo)
    {
        circleGo.transform.localPosition = circleInfo.centerPoint;
        circleGo.transform.localScale = Vector3.one * circleInfo.radius * 2;
    }

    public bool CheckIsInMoveCircle(Vector3 PlayerPos,GameObject whoCircle)//不看y值
    {
        float radius = whoCircle.transform.localScale.x/2f * scaleRadiusToLocalPostionMi;
        Vector3 centerPoint = whoCircle.transform.localPosition;
        float distancePlayerAndWhoCircle=Vector3DistanceRemoveY(PlayerPos, centerPoint);
        if (distancePlayerAndWhoCircle<radius)
        {
            return true;
        }
        return false;
    }

    public float Vector3DistanceRemoveY(Vector3 pos1,Vector3 pos2)
    {
        pos1=new Vector3(pos1.x,0,pos1.z);
        pos2=new Vector3(pos2.x,0,pos2.z);
        return Vector3.Distance(pos1, pos2);
    }

    public float CalcMaxDistanceToShowRed(PlayerArrowController eachPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
        }
        else
        {
            moveCircleInfo = otherMoveCircleInfo;
            desCircleInfo = otherDesCircleInfo;
        }
        if (moveCircleInfo == null || desCircleInfo == null) return 1000000;

        float moveCircleRadius = moveCircleInfo.radius * scaleRadiusToLocalPostionMi;
        float desCircleRadius = desCircleInfo.radius * scaleRadiusToLocalPostionMi;
        float distanceMoveAndDesCircle = Vector3DistanceRemoveY(desCircleInfo.centerPoint, moveCircleInfo.centerPoint);
        if (distanceMoveAndDesCircle<=1f)//同心圆
        {
            eachPlayer.maxDistanceToShowRed = moveCircleRadius - desCircleRadius;
            return eachPlayer.maxDistanceToShowRed;
        }
        float distancePlayerAndMoveCircle =
            Vector3DistanceRemoveY(transform.InverseTransformPoint(eachPlayer.transform.position), moveCircleInfo.centerPoint);
        float distancePlayerAndDesCircle = Vector3DistanceRemoveY(transform.InverseTransformPoint(eachPlayer.transform.position), desCircleInfo.centerPoint);
        //已知三边求夹角
        float CosAngle1 = (Mathf.Pow(distancePlayerAndDesCircle, 2f) + Mathf.Pow(distancePlayerAndMoveCircle, 2f) -
                          Mathf.Pow(distanceMoveAndDesCircle, 2))/(2* distancePlayerAndDesCircle* distancePlayerAndMoveCircle);
        float Angle1 = Mathf.Acos(CosAngle1);
        if (isDebugLog)
            Debug.Log("Angle1:" + Angle1);
        float Angle2 = Mathf.PI - Angle1;
        if (isDebugLog)
            Debug.Log("Angle2:" + Angle2);
        //已知两边和一角求地三边
        float sinAngle3 = distancePlayerAndMoveCircle * Mathf.Sin(Angle2) / moveCircleRadius;
        float Angle3 = Mathf.Asin(sinAngle3);
        if (isDebugLog)
            Debug.Log("Angle3:" + Angle3);
        float Angle4 = Mathf.PI - Angle2 - Angle3;
        if (isDebugLog)
            Debug.Log("Angle4:" + Angle4);
        float playerToMoveCircleBound = Mathf.Sin(Angle4) * moveCircleRadius / Mathf.Sin(Angle3);
        if (isDebugLog)
            Debug.Log("playerToMoveCircleBound:" + playerToMoveCircleBound);
        float distanceDesCircleToMoveBoundsByPlayer =
            playerToMoveCircleBound + distancePlayerAndDesCircle - desCircleRadius;
        eachPlayer.maxDistanceToShowRed = distanceDesCircleToMoveBoundsByPlayer;
        if (isDebugLog)
            Debug.Log("eachPlayer maxDistanceToShowRed:" + eachPlayer.maxDistanceToShowRed);
        return eachPlayer.maxDistanceToShowRed;
    }

    Material mat = null;
    public void CalcArrowColor(PlayerArrowController eachPlayer)
    {
        
        if (PhotonNetwork.isMasterClient)
        {
        }
        else
        {
            moveCircleInfo = otherMoveCircleInfo;
            desCircleInfo = otherDesCircleInfo;
        }
        if (moveCircleInfo == null || desCircleInfo == null) return;

        Color colorMat;
        //if (isGetMat==false)
        //{
        //    isGetMat = true;
        //    //mat = eachPlayer.ArrowGo.GetComponent<MeshRenderer>().sharedMaterial;//公用材质一改所有的都一样，都改了
        //    mat = eachPlayer.ArrowGo.GetComponent<MeshRenderer>().material;
        //}
        mat = eachPlayer.ArrowGo.GetComponent<MeshRenderer>().material;
        if (mat==null)
        {
            Debug.Assert(mat!=null);
            return;
        }
        float playerToDesCircleBound=Vector3DistanceRemoveY(transform.InverseTransformPoint(eachPlayer.transform.position), desCircleInfo.centerPoint) -desCircleInfo.radius * scaleRadiusToLocalPostionMi;
        if (playerToDesCircleBound> eachPlayer.maxDistanceToShowRed)
        {
            
            colorMat = new Color(1,0,0);
        }
        else if (playerToDesCircleBound> eachPlayer.maxDistanceToShowRed * 1f/2)
        {
            float colorG=1-(playerToDesCircleBound - eachPlayer.maxDistanceToShowRed * 1f / 2) / (eachPlayer.maxDistanceToShowRed * 1f / 2);
            colorMat=new Color(1, colorG, 0);
        }
        else if(playerToDesCircleBound > 0f)//还在 圈外
        {
            float colorR = (playerToDesCircleBound - 0f) / (eachPlayer.maxDistanceToShowRed * 1f / 2);
            colorMat = new Color(colorR, 1, 0);
        }
        else
        {
            colorMat = new Color(0, 1, 0);
        }

        mat.SetColor("__Color", colorMat);
        if(isDebugLog)
            Debug.Log(eachPlayer.name + "  " + eachPlayer.maxDistanceToShowRed+"  "+"("+ colorMat.r+ ","+colorMat .g+","+ colorMat .b+ ")");

    }

    public bool isRandomPointHaveCollider(Vector3 randomCirclePoint)
    {
        RaycastHit hit;

        if (Physics.Raycast(randomCirclePoint + Vector3.up, Vector3.down, out hit))
        {
            if (isDebugLog)
                Debug.Log("Collider:"+hit.collider.name);
            if (hit.collider.name.Contains("Player")|| hit.collider.name.Contains("Terrain")|| hit.collider.gameObject.tag=="Player")
            {
                return false;
            }
            return true;
        }
        return false;
    }

    
	void FixedUpdate()
    {
        if (circleMoveInfoList == null || circleMoveInfoList.Count <= 0)
        {
            Debug.Assert(circleMoveInfoList != null && circleMoveInfoList.Count > 0);
            return;
        }
        if (PhotonNetwork.isMasterClient)
        {
            for (int index = 0; index < circleMoveInfoList.Count; index++)
            {
                if (circleMoveInfoList[index].isCompleted) continue;
                if (circleMoveInfoList[index].isCompleted == false)
                {
                    CurrOutCircleDamage = circleMoveInfoList[index].damage;
                    Vector2 desCircleCenterPoint;
                    float desCentetPointCircleRadius;
                    Vector3 desCircleCenterPointV3;
                    if (circleMoveInfoList[index].isCalcDesCircleCenterPoint)
                    {//只设置一次

                        desCentetPointCircleRadius = sourceCircleInfo.radius - circleMoveInfoList[index].desCircleRadius;
                        if (isDebugLog)
                            Debug.Log("desCentetPointCircleRadius:" + desCentetPointCircleRadius);
                        desCircleCenterPoint = Random.insideUnitCircle * desCentetPointCircleRadius * scaleRadiusToLocalPostionMi;
                        desCircleCenterPoint *= radiusCenterPointCoefficient;
                        if (isDebugLog)
                            Debug.Log("desCircleCenterPoint:(" + desCircleCenterPoint.x + "," + desCircleCenterPoint.y + ")");
                        desCircleCenterPointV3 = new Vector3(desCircleCenterPoint.x, 0, desCircleCenterPoint.y);
                        desCircleCenterPointV3 += sourceCircleInfo.centerPoint;//加上原圈的圆心，进行偏移

                        while (isRandomPointHaveCollider(desCircleCenterPointV3))
                        {
                            desCentetPointCircleRadius = sourceCircleInfo.radius - circleMoveInfoList[index].desCircleRadius;
                            if (isDebugLog)
                                Debug.Log("desCentetPointCircleRadius:" + desCentetPointCircleRadius);
                            desCircleCenterPoint = Random.insideUnitCircle * desCentetPointCircleRadius * scaleRadiusToLocalPostionMi;
                            desCircleCenterPoint *= radiusCenterPointCoefficient;
                            if (isDebugLog)
                                Debug.Log("desCircleCenterPoint:(" + desCircleCenterPoint.x + "," + desCircleCenterPoint.y + ")");
                            desCircleCenterPointV3 = new Vector3(desCircleCenterPoint.x, 0, desCircleCenterPoint.y);
                            desCircleCenterPointV3 += sourceCircleInfo.centerPoint;//加上原圈的圆心，进行偏移
                        }

                        desCircleInfo = new Circle(circleMoveInfoList[index].desCircleRadius, desCircleCenterPointV3);
                        DrawCircle(initDesCircle, desCircleInfo);
                        circleMoveInfoList[index].isCalcDesCircleCenterPoint = false;
                    }

                    circleMoveInfoList[index].desCircleShowTime -= Time.fixedUnscaledDeltaTime;
                    if (circleMoveInfoList[index].desCircleShowTime <= 0f)
                    {//显示缩圈目标圈
                        isShowDesCircle = true;
                        initDesCircle.SetActive(true);
                    }

                    if (circleMoveInfoList[index].isCalcSpeed == false)
                    {
                        circleMoveInfoList[index].moveSpeed =
                            Vector3DistanceRemoveY(sourceCircleInfo.centerPoint, desCircleInfo.centerPoint) /
                            circleMoveInfoList[index].moveTime;
                        circleMoveInfoList[index].moveAndScaleRadius = sourceCircleInfo.radius;
                        circleMoveInfoList[index].moveAndScaleCenterPoint = sourceCircleInfo.centerPoint;
                        circleMoveInfoList[index].isCalcSpeed = true;
                    }

                    if (circleMoveInfoList[index].sourceCircleLeftTime <= 0f)
                    {//开始缩圈
                        moveScaleCircle.SetActive(true);
                        if (circleMoveInfoList[index].moveAndScaleRadius <= circleMoveInfoList[index].desCircleRadius)
                        {//缩圈结束
                            circleMoveInfoList[index].isCompleted = true;
                            sourceCircleInfo = desCircleInfo;
                            DrawCircle(InitSourceCircle, sourceCircleInfo);
                            DrawCircle(moveScaleCircle, sourceCircleInfo);
                            moveCircleInfo = sourceCircleInfo;
                            //moveScaleCircle.SetActive(false);

                            isShowDesCircle = false;
                            initDesCircle.SetActive(false);
                        }
                        else
                        {
                            circleMoveInfoList[index].moveAndScaleRadius -= Time.fixedUnscaledDeltaTime * circleMoveInfoList[index].moveSpeed;
                            Vector3 centerPointMoveDir = Vector3.Normalize(desCircleInfo.centerPoint - circleMoveInfoList[index].moveAndScaleCenterPoint);//每帧计算方向
                            Vector3 moveCenterPoint = circleMoveInfoList[index].moveAndScaleCenterPoint +=
                                centerPointMoveDir * Time.fixedUnscaledDeltaTime * circleMoveInfoList[index].moveSpeed * 2f;//移动圆心比设置半径快。
                            if (Vector3DistanceRemoveY(moveCenterPoint, desCircleInfo.centerPoint) < float.Epsilon)
                            {//防止走过了
                                moveCenterPoint = desCircleInfo.centerPoint;
                            }
                            Circle desMoveInfo = new Circle(circleMoveInfoList[index].moveAndScaleRadius, moveCenterPoint);
                            moveCircleInfo = desMoveInfo;
                            DrawCircle(moveScaleCircle, desMoveInfo);
                        }
                    }
                    else
                    {
                        circleMoveInfoList[index].sourceCircleLeftTime -= Time.fixedUnscaledDeltaTime;
                    }

                    break;
                }
            }

            MinimapController.mInstance.UpdateMoveCircle(moveCircleInfo,"CicleMiniMap");
        }
        else
        {
            if (otherDesCircleInfo != null)
                DrawCircle(initDesCircle,otherDesCircleInfo);
            if (otherMoveCircleInfo != null)
            {
                DrawCircle(moveScaleCircle, otherMoveCircleInfo);
                MinimapController.mInstance.UpdateMoveCircle(otherMoveCircleInfo, "CicleMiniMap");
            }
            else
            {//刚进来为空
                if (moveScaleCircle!=null)
                {
                    otherMoveCircleInfo=new Circle(moveScaleCircle.transform.localScale.x/2, moveScaleCircle.transform.localPosition);
                }
            }
            if (otherIsShowDesCircle)
            {
                initDesCircle.SetActive(true);
            }
            else
            {
                initDesCircle.SetActive(false);
            }

        }


        if (Player != null)
        {
            if (!CheckIsInMoveCircle(transform.InverseTransformPoint(Player.transform.position), moveScaleCircle))
            {//移动圈减血
                if (isDebugLog)
                    Debug.Log("OutsideCircle");
                Camera.main.GetComponent<SepiaTone>().enabled = true;
                if (eachSecond < 0f)
                {
                    eachSecond = 1f;
                    if (PhotonNetwork.isMasterClient)
                    {
                        Player.GetComponent<CharacterBehaviour>().TakeDamage(CurrOutCircleDamage);

                    }
                    else
                    {
                        Player.GetComponent<CharacterBehaviour>().TakeDamage(otherCurrOutCircleDamage);
                    }

                    //Player.hp -= CurrOutCircleDamage;
                    //if (Player.hp<=0f)
                    //{
                    //    Player.hp = 0f;
                    //}

                    //if (isDebugLog)
                    //        Debug.Log(Player.hp);
                }
                else
                {
                    eachSecond -= Time.fixedUnscaledDeltaTime;
                }

            }
            else
            {
                Camera.main.GetComponent<SepiaTone>().enabled = false;
            }
        }
        

        //ShowSelfPlayerArrowAndColor();


    }
    
    public void ShowSelfPlayerArrowAndColor(PlayerArrowController eachPlayer)
    {
        if (!CheckIsInMoveCircle(transform.InverseTransformPoint(eachPlayer.transform.position), moveScaleCircle))
        {
            if (eachPlayer.photonView.isMine)
                eachPlayer.ArrowGo.SetActive(true);
            eachPlayer.ArrowGo.transform.LookAt(initDesCircle.transform.position);
            eachPlayer.maxDistanceToShowRed = 0f;//始终显示红色
            CalcArrowColor(eachPlayer);
        }
        else if (!CheckIsInMoveCircle(transform.InverseTransformPoint(eachPlayer.transform.position), initDesCircle))
        {
//目标圈显示箭头
            if (initDesCircle.activeInHierarchy)
            {
                if(eachPlayer.photonView.isMine)
                    eachPlayer.ArrowGo.SetActive(true);
            }
            else if (!CheckIsInMoveCircle(transform.InverseTransformPoint(eachPlayer.transform.position), moveScaleCircle))
            {
                if (eachPlayer.photonView.isMine)
                    eachPlayer.ArrowGo.SetActive(true);
            }
            else
            {
                eachPlayer.ArrowGo.SetActive(false);
            }
            eachPlayer.ArrowGo.transform.LookAt(initDesCircle.transform.position);
            //if (calcArrowColorTime <= 0f)
            //{
                CalcMaxDistanceToShowRed(eachPlayer);
                CalcArrowColor(eachPlayer);
                //calcArrowColorTime = 0.1f;
            //}
            //else
            //{
            //    calcArrowColorTime -= Time.fixedUnscaledDeltaTime;
            //}
        }
        else
        {
            eachPlayer.ArrowGo.SetActive(false);
        }
    }

    public bool otherIsShowDesCircle;
    public float otherCurrOutCircleDamage;
    public Circle otherDesCircleInfo;
    public Circle otherMoveCircleInfo;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            if (BattleSceneManager.Instance.alivePlayerNumber <= 1)return;
            if (PhotonNetwork.isMasterClient)
            {
                stream.SendNext(isShowDesCircle);
                stream.SendNext(CurrOutCircleDamage);
                if (desCircleInfo != null)
                {
                    stream.SendNext(desCircleInfo.radius);
                    stream.SendNext(desCircleInfo.centerPoint);
                }
                if (moveCircleInfo != null)
                {
                    stream.SendNext(moveCircleInfo.radius);
                    stream.SendNext(moveCircleInfo.centerPoint);
                }
            }
        }
        else if (stream.isReading)
        {
            if (BattleSceneManager.Instance.alivePlayerNumber <= 1) return;
            if (!PhotonNetwork.isMasterClient)
            {
                otherIsShowDesCircle = (bool) stream.ReceiveNext();
                otherCurrOutCircleDamage= (float)stream.ReceiveNext();
                object otherDesRadius = stream.ReceiveNext();
                object otherDesCenterPoint = stream.ReceiveNext();
                if (otherDesRadius != null && otherDesCenterPoint != null)
                {
                    otherDesCircleInfo = new Circle((float)otherDesRadius, (Vector3)otherDesCenterPoint);
                }
                object otherMoveRadius = stream.ReceiveNext();
                object otherMoveCenterPoint = stream.ReceiveNext();
                if (otherMoveRadius != null && otherMoveCenterPoint != null)
                {
                    otherMoveCircleInfo = new Circle((float)otherMoveRadius, (Vector3)otherMoveCenterPoint);
                }
            }
        }
    }
}
