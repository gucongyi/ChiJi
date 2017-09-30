using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAndDogCharacterController : MonoBehaviour {

	public Animator _animator;

	public bool isGrounded;

	Transform selfTransform;


	void Awake () {
		selfTransform = transform;
	}

	void FixedUpdate()
	{
		//Kentoy
		CheckForGrounded();

		if (_animator == null) return;

//        float x = Input.GetAxis("Horizontal");
//        float y = Input.GetAxis("Vertical");
//
//        Move(x, y);
//
//        Debug.Log(x);

//		if(Input.GetKeyDown(KeyCode.Space))
//		{
//			_animator.Play("Upper-Body-Fire", 1, 0.0f);
//		}
	}

	void CheckForGrounded()
	{
		float distanceToGround;
		float threshold = .45f;
		RaycastHit hit;
		Vector3 offset = new Vector3(0, 0.4f,0);
		if(Physics.Raycast((transform.position + offset), -Vector3.up, out hit, 100f))
		{
			distanceToGround = hit.distance;
			if(distanceToGround < threshold)
			{
				isGrounded = true;
			}
			else
			{
				isGrounded = false;
			}
		}
	}

	public void _Fire () {
		_animator.Play("Upper-Body-Fire", 1, 0.0f);
	}

	public void _Reload () {
		int a = Random.RandomRange(0, 1);
		if (a == 0)
		{
			_animator.SetTrigger("Reload01");
		}
		else
		{
			_animator.SetTrigger("Reload02");
		}
	}

	public void _Skill01 () {
		_animator.SetTrigger("Skill01");
	}

	public void _Death () {
		Debug.LogError("Call _Death");
	}

	public void _Revive () {
		Debug.LogError("Call _Revive");
	}

//	void OnGUI () {
//		if(GUI.Button(new Rect(1115, 345, 100, 30), "Skill"))
//		{
//			_Skill();
//		}
//
//		if(GUI.Button(new Rect(1115, 445, 100, 30), "Reload0"))
//		{
//			_Reload();
//		}
//
//		if(GUI.Button(new Rect(1115, 545, 100, 30), "Fire"))
//		{
//			_Fire();
//		}
//	}

    public void Move(float x, float y) {
        _animator.SetFloat("Velx", x);
        _animator.SetFloat("Vely", y);

//		selfTransform.localPosition = Vector3.zero;
    }
}
