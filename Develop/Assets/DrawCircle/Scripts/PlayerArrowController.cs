using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;

public class PlayerArrowController : MonoBehaviour
{
    public float moveSpeed;
    public float hp;
    public GameObject ArrowGo;
    public float maxDistanceToShowRed;//每个人自己有个最大距离，否则所有人显示一样了
    public PhotonView photonView;

    private Transform mTrans;

    private DrawCicle drawCicle;
    // Use this for initialization
    void Start ()
	{
	    mTrans = transform;
	    hp = 100f;
	    drawCicle = BattleSceneManager.Instance.TransDrawCircleRoot.GetComponent<DrawCicle>();
	    Debug.Log("BattleSceneManager.Instance:" + BattleSceneManager.Instance);
        Debug.Log("drawCicle:" + drawCicle);
	    photonView = transform.GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKey(KeyCode.W))
	        mTrans.Translate(Vector3.forward * Time.deltaTime* moveSpeed);
	    if (Input.GetKey(KeyCode.S))
	        mTrans.Translate(Vector3.back * Time.deltaTime* moveSpeed);

        if (Input.GetKey(KeyCode.A))
            mTrans.Translate(Vector3.left * Time.deltaTime* moveSpeed);
        if (Input.GetKey(KeyCode.D))
            mTrans.Translate(Vector3.right * Time.deltaTime* moveSpeed);
    }
    void LateUpdate()
    {
        //每个人自己计算箭头信息。
        drawCicle.ShowSelfPlayerArrowAndColor(this);
    }
}
