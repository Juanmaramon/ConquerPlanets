using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [SerializeField] float speed = 10f;

    [SerializeField] Rigidbody _rigid;
    float _horizontal = 0f;
    float _vertical = 0f;
    Vector3 _moveDir;
    	
	private void FixedUpdate()
	{
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = (_horizontal != 0f) ? 0 : Input.GetAxisRaw("Vertical");

        _moveDir = new Vector3(_horizontal, 0.0f, _vertical).normalized;

        _rigid.MovePosition(_rigid.position + transform.TransformDirection(_moveDir) * speed * Time.deltaTime);
    }
}
