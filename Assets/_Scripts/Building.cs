using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] Rigidbody _rigid;
    [SerializeField] Collider _collider;
    [SerializeField] Animator _anim;
    [SerializeField] FauxGravityBody _gravity;
    public static float buildTime = 5f;
    static float staticTime = 2f;
    static float yieldTime = 0.2f;

    float _nextBuildTime = 0f;
    float _initBuildTime = 0f;
    WaitForSeconds _yield;
    BasicEvent _tmpEvent;

    // Use this for initialization
    void Start()
    {
        _tmpEvent = new BasicEvent();
        _yield = Yielders.Get(yieldTime);
        _nextBuildTime = 0f;
        OnConstruction();
        StartCoroutine(BuildProcess());
    }

    void OnConstruction()
    {
        _anim.SetTrigger("Building");
    }

	void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.tag == "Planet") 
        {
            StartCoroutine(StaticBuilding());
        }
	}

    IEnumerator BuildProcess()
    {
        _collider.enabled = true;

        // Wait for build
        _nextBuildTime = Time.time + buildTime;
        _initBuildTime = Time.time;
        while (Time.time < _nextBuildTime)
        {
            _tmpEvent.Data = (Time.time - _initBuildTime) / buildTime;
            EventManager.TriggerEvent("OnProgressBuilding", _tmpEvent);
            yield return _yield;
        }

        // Building complete!   
    }

    IEnumerator StaticBuilding()
    {
        yield return Yielders.Get(staticTime);

        // Make static
        _rigid.isKinematic = true;
        _gravity.enabled = false;
    }
}
