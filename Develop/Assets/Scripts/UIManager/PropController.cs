using System.Collections;
using System.Collections.Generic;
using CatsAndDogs;
using UnityEngine;

public class PropController : MonoBehaviour
{
    public string id;
    public int durable;

    public float speed;

    public bool isOtherNew = false;//非主机 替换
	// Use this for initialization
	void Start () {
		
	}
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            if (PhotonNetwork.isMasterClient)
            {
                stream.SendNext(id);
                stream.SendNext(durable);
            }
            else if (isOtherNew)
            {
                stream.SendNext(id);
                stream.SendNext(durable);
            }
        }
        else if (stream.isReading)
        {
            if (!PhotonNetwork.isMasterClient)
            {//同步id
                string id = (string)stream.ReceiveNext();
                this.id = id;
                int durable = (int)stream.ReceiveNext();
                this.durable = durable;
            }
            else if (isOtherNew)
            {
                string id = (string)stream.ReceiveNext();//主机接收
                this.id = id;
                int durable = (int)stream.ReceiveNext();
                this.durable = durable;
            }
        }
    }
    
    // Update is called once per frame
    void Update () {
		transform.Rotate(Vector3.up,Time.deltaTime*speed);
	}
}
