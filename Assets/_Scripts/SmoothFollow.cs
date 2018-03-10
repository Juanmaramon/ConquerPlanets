using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollow : MonoBehaviour {

    [SerializeField] Transform target;
    [SerializeField] float smoothness = 1f;
    [SerializeField] float rotationSmoothness = .1f;
    [SerializeField, HideInInspector] Camera _cam;

	public Vector3 offset;

	private Vector3 velocity = Vector3.zero;

    float _minFov = 40f;
    float _maxFov = 70f;
    float _sensitivity = 5f;
    float _fov;

	private void Update()
	{
        _fov = _cam.fieldOfView;
        _fov += Input.GetAxis("Mouse ScrollWheel") * _sensitivity;
        _fov = Mathf.Clamp(_fov, _minFov, _maxFov);
        _cam.fieldOfView = _fov;		
	}

	void FixedUpdate () 
    {
		if (target == null)
		{
			return;
		}

		Vector3 newPos = target.TransformDirection(offset);
		transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothness);

		Quaternion targetRot = Quaternion.LookRotation(-transform.position.normalized, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSmoothness);
	}
}
