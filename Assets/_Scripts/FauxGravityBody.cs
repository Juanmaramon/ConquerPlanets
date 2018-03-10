using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityBody : MonoBehaviour {

    [SerializeField] FauxGravityAttractor _attractor;
    [SerializeField] Transform _trans;
    [SerializeField] Rigidbody _rigid;

	private void Start()
    {
        // Rotation and gravity driven by Attractor
        _rigid.constraints = RigidbodyConstraints.FreezeRotation;
        _rigid.useGravity = false;
	}

	void Update () 
    {
        _attractor.Attract(_trans);	
	}
}
