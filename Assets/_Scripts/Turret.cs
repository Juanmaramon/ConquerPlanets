using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

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
    [SerializeField] ParticleSystem _fire;
    [SerializeField] ParticleSystem _shoot;
    [SerializeField] Transform _shootPoint;

    Vector3 _offsetVisibility = new Vector3(0f, 1f, 0f);

    public const float buildingTime = 7f;
    float _nextExtractTime = 0f;
    float _initExtractTime = 0f;
    BasicEvent _tmpEvent;
    WaitForSeconds _yield;
    bool _targetting;
    Collider[] _targetsInViewRadius = new Collider[10];
    float _fireRate;
    float _nextFireTime;
    RaycastHit _hit;

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
        _targetting = false;
        _fireRate = _shoot.main.duration;
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
        int found = Physics.OverlapSphereNonAlloc(_trans.position, viewRadius, _targetsInViewRadius, targetMask);

        for (int i = 0; i < found; i++)
        {
            visibleTargets.Add(_targetsInViewRadius[i].transform);
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

            if (!_targetting)
            {
                _targetting = true;
                _fire.Play(true);
                _nextFireTime = Time.time + _fireRate;
            }
            else
            {
                if (Time.time > _nextFireTime)
                {
                    _nextFireTime = Time.time + _fireRate;
                    Debug.DrawRay(_shootPoint.position, ((visibleTargets[0].position + _offsetVisibility) - _shootPoint.position).normalized, Color.red, Mathf.Infinity);
                    //Debug.DrawLine(_shootPoint.position, visibleTargets[0].position + _offsetVisibility, Color.red, Mathf.Infinity);
                    if (Physics.Raycast(_shootPoint.position, ((visibleTargets[0].position + _offsetVisibility) - _shootPoint.position).normalized, out _hit, Mathf.Infinity, targetMask, QueryTriggerInteraction.UseGlobal))
                    {
                        //Debug.DrawLine(_shootPoint.position, _trans.position + visibleTargets[0].position, Color.red);
                        if (_hit.collider)
                        {
                            Debug.Log(_hit.collider.name);
                        }
                    }
                }
            }
        }
        else
        {
            if (_targetting)
            {
                _targetting = false;
                _fire.Stop(true);
            }
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
