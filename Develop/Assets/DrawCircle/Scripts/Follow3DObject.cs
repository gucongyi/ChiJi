using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Follow3DObject : MonoBehaviour {

    public Transform target;
    public Vector3 offset = new Vector3(0, 1, 0);
    private PlayerArrowController _playerArrowCtl;

    public Slider hpSlider;
    // Use this for initialization
    void Start()
    {
        if (target != null)
            _playerArrowCtl = target.GetComponent<PlayerArrowController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + offset);
            hpSlider.value = _playerArrowCtl.hp/100f;
        }
    }
}
