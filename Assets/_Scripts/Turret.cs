using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour 
{
    [SerializeField] Transform _trans;
    [SerializeField] Rigidbody _rigid;
    [SerializeField] Transform topTurret;
    [SerializeField] Transform target;
    [SerializeField] float rotationSmoothness = .1f;
    [SerializeField] float viewRadius;
    [SerializeField] LayerMask targetMask;
    [HideInInspector] List<Transform> visibleTargets = new List<Transform>();
    [SerializeField] Animator _anim;

    Vector3 _offsetVisibility = new Vector3(0f, 1f, 0f);

    public const float buildingTime = 7f;
    float _nextExtractTime = 0f;
    float _initExtractTime = 0f;
    BasicEvent _tmpEvent;
    WaitForSeconds _yield;

    static float visibilityTime = 2f;
    static float staticTime = 2f;
    static float yieldTime = 0.2f;

	private void Start()
	{
        _yield = Yielders.Get(yieldTime);
        _tmpEvent = new BasicEvent();
        // Here UI loading refresh...
        StartCoroutine(RefreshUI());
        _anim.SetTrigger("Building");
	}

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return Yielders.Get(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(_trans.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            visibleTargets.Add(targetsInViewRadius[i].transform);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Planet")
        {
            StartCoroutine(StaticBuilding());
        }
    }

    IEnumerator StaticBuilding()
    {
        yield return Yielders.Get(staticTime);

        // Make static
        _rigid.isKinematic = true;
    }

	void Update () 
    {
        if (visibleTargets.Count > 0)
        {
            Quaternion rotation = Quaternion.LookRotation((visibleTargets[0].position +_offsetVisibility) - topTurret.position, _trans.up);
            topTurret.rotation = Quaternion.Slerp(topTurret.rotation, rotation, Time.deltaTime * rotationSmoothness);
        }
	}

    IEnumerator RefreshUI()
    {
        _tmpEvent.Data = Building.TURRET;
        EventManager.TriggerEvent("OnProgressChange", _tmpEvent);

        // Wait for build
        _nextExtractTime = Time.time + buildingTime;
        _initExtractTime = Time.time;
        while (Time.time < _nextExtractTime)
        {
            _tmpEvent.Data = (Time.time - _initExtractTime) / buildingTime;
            EventManager.TriggerEvent("OnProgressUnit", _tmpEvent);
            yield return _yield;
        }  

        // Turret complete!
        StartCoroutine("FindTargetsWithDelay", visibilityTime);
    }

}
