using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMoveInfo : MonoBehaviour {

    public float desCircleRadius;
    public float moveTime;
    [HideInInspector]
    public float moveSpeed;//根据距离算出来的
    [HideInInspector]
    public bool isCalcSpeed;
    public float sourceCircleLeftTime;
    public float damage;
    public float desCircleShowTime;
    [HideInInspector]
    public bool isCompleted;
    [HideInInspector]
    public bool isCalcDesCircleCenterPoint;
    [HideInInspector]
    public float moveAndScaleRadius;//用来缩圈
    [HideInInspector]
    public Vector3 moveAndScaleCenterPoint;//用来缩圈

    void Awake()
    {
        isCompleted = false;
        isCalcDesCircleCenterPoint = true;
        isCalcSpeed = false;
    }
}
