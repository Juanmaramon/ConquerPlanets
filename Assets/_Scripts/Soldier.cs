using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : PathManager 
{
    [SerializeField] Animator _anim;
    [SerializeField] Waypoints _waypoints;
    [SerializeField] Rigidbody _rigid;
    [SerializeField] CapsuleCollider _col;

	private void Awake()
	{
        _waypoints = Waypoints.instance;
	}

	private void Start()
	{
        var rand = Random.Range(0, 5);
        float res = 0f;
        switch (rand)
        {
            case 1:
                res = 0.25f;
                break;
            case 2:
                res = 0.5f;
                break;
            case 3:
                res = 0.75f;
                break;
            case 4:
                res = 1f;
                break;
        }

        _anim.SetFloat("Blend", res);
        _rigid.isKinematic = true;
        _col.isTrigger = true;
        _anim.SetTrigger("Run");
        NavigateTo(transform.position + transform.forward * 10f);
       // Debug.DrawRay(_trans.position, transform.forward * 10f, Color.green, Mathf.Infinity);
 	}

	private new void Update()
	{
        base.Update();
	}

    protected override void OnTargedReached()
    {
        _anim.SetTrigger("Dance");
        _rigid.isKinematic = false;
        _col.isTrigger = false;
    }
}
