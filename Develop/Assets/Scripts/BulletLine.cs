using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletLine : MonoBehaviour {

    private LineRenderer lineRender;

    void Awake() {
        lineRender = GetComponent<LineRenderer>();
    }

    void Start() {
        lineRender.positionCount = 2;
        lineRender.startWidth = lineRender.endWidth = 0.1f;
        lineRender.startColor = lineRender.endColor = Color.red;
    }
 
     void Update() {
        lineRender.SetPosition(0, transform.position);

        Vector3 direction = transform.forward;
        Vector3 targetPos = transform.position + transform.forward * 50f;
        Debug.DrawLine(transform.position, targetPos, Color.green);
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, direction, out hitInfo, 50f)) {
            lineRender.SetPosition(1, hitInfo.point);
            // Debug.DrawLine(transform.position, hitInfo.point, Color.green);
        } else {
            lineRender.SetPosition(1, targetPos);
        }
    }
}
