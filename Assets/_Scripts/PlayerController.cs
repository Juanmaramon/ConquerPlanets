using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [SerializeField] float speed = 10f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField, HideInInspector] Transform _trans;
    [SerializeField, HideInInspector] Rigidbody _rigid;
    [SerializeField] Animator _anim;
    [SerializeField] GameObject[] buildings;

    float _horizontal = 0f;
    float _vertical = 0f;
    Vector3 _moveDir;
    bool _running = false;
    bool _offline = false;

	private void Start()
	{
        _running = _offline = false;
	}

	void FixedUpdate()
	{
        if (_offline)
            return;

        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(Build());
        }

        _horizontal = Input.GetAxis("Horizontal");
        _vertical = /*(_horizontal != 0f) ? 0 :*/ Input.GetAxis("Vertical");

        _vertical = (_vertical < 0f) ? 0 : _vertical;

        _moveDir = new Vector3(_horizontal, 0.0f, _vertical).normalized;

        _rigid.MovePosition(_rigid.position + _trans.TransformDirection(_moveDir) * speed * Time.fixedDeltaTime);

        Vector3 yRotation = Vector3.up * _horizontal * rotationSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(yRotation);
        Quaternion targetRotation = _rigid.rotation * deltaRotation;
        _rigid.MoveRotation(Quaternion.Slerp(_rigid.rotation, targetRotation, 150f * Time.deltaTime));
    }

    void Update()
    {
        if (_offline)
            return;
        
        // Update animation state
        if (_moveDir != Vector3.zero && !_running)
        {
            _running = true;
            _anim.SetTrigger("Run");
        }
        else if (_running && _moveDir == Vector3.zero)
        {
            _running = false;
            _anim.SetTrigger("Stop");
        }
    }

    IEnumerator Build()
    {
        _offline = true;
        _anim.SetBool("Build", true);
        var spawnBuildingPosition = _trans.position + transform.forward * 4;
        Instantiate(buildings[0], spawnBuildingPosition, Quaternion.identity);

        yield return Yielders.Get(Building.buildTime);

        _offline = false;
        _anim.SetBool("Build", false);
    }
}
