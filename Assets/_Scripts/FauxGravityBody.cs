using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityBody : MonoBehaviour {

    [SerializeField] FauxGravityAttractor _attractor;
    [SerializeField/*, HideInInspector*/] Transform _trans;
    [SerializeField/*, HideInInspector*/] Rigidbody _rigid;

	private void Start()
    {
        if (!_attractor)
            _attractor = GameObject.FindWithTag("Planet").GetComponent<FauxGravityAttractor>();
        // Rotation and gravity driven by Attractor
        _rigid.constraints = RigidbodyConstraints.FreezeRotation;
        _rigid.useGravity = false;
	}

	void Update () 
    {
        _attractor.Attract(_trans);	
	}
}
