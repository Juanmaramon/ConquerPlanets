using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : PathManager 
{
    [SerializeField] Animator _anim;
    [SerializeField] Rigidbody _rigid;
    [SerializeField] CapsuleCollider _col;
    [HideInInspector] List<Transform> visibleTargets = new List<Transform>();
    [SerializeField] float viewRadius;
    [SerializeField] LayerMask targetMask;
    Collider[] _targetsInViewRadius = new Collider[10];

    static float visibilityTime = 2f;
    bool _findingPath = true;

	private void Start()
	{
        _findingPath = true;
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
        switch (rand)
        {
            default:
            case 1:
                NavigateTo(_trans.position + _trans.forward * 10f);
                break;
            case 2:
                NavigateTo(_trans.position + _trans.right * 10f);
                break;
            case 3:
                NavigateTo(_trans.position - _trans.forward * 10f);
                break;
            case 4:
                NavigateTo(_trans.position - _trans.right * 10f);
                break;
        }
                
        //Debug.DrawRay(_trans.position, transform.forward * 10f, Color.green, Mathf.Infinity);
 	}

	private new void Update()
	{
        if (_findingPath)
            base.Update();
 	}

    protected override void OnTargedReached()
    {
        //_anim.SetTrigger("Dance");
        _anim.SetTrigger("Idle");
        _rigid.isKinematic = false;
        _col.isTrigger = false;
        _findingPath = false;
        StartCoroutine("FindTargetsWithDelay", visibilityTime);
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        int found = Physics.OverlapSphereNonAlloc(_trans.position, viewRadius, _targetsInViewRadius, targetMask);

        for (int i = 0; i < found; i++)
        {
            visibleTargets.Add(_targetsInViewRadius[i].transform);
            Debug.Log(_targetsInViewRadius[i].name);
        }
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (!_findingPath)
        {
            yield return Yielders.Get(delay);
            FindVisibleTargets();
        }
    }
}
