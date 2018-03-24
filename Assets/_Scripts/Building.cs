using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] Rigidbody _rigid;
    [SerializeField] Collider _collider;
    [SerializeField] Animator _anim;
    [SerializeField] FauxGravityBody _gravity;
    [SerializeField] GameObject _smoke;
    [SerializeField] BoxCollider _boxCollider;
    [SerializeField] GameObject _soldier;
    [SerializeField] float _spawnSoldiderDistance = 0.5f;
    [SerializeField] Transform _trans;
    public static float buildTime = 5f;
    public static float trainTime = 5f;
    static float staticTime = 2f;
    static float yieldTime = 0.2f;

    // @TODO: change to global item enum
    public static string SOLDIER = "SOLDIER";
    public static string RESOURCES = "RESOURCES";

    float _nextWaitTime = 0f;
    float _initWaitTime = 0f;
    WaitForSeconds _yield;
    BasicEvent _tmpEvent;
    bool _training = false;

    // Use this for initialization
    void Start()
    {
        _tmpEvent = new BasicEvent();
        _yield = Yielders.Get(yieldTime);
        _nextWaitTime = 0f;
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
        _nextWaitTime = Time.time + buildTime;
        _initWaitTime = Time.time;
        while (Time.time < _nextWaitTime)
        {
            _tmpEvent.Data = (Time.time - _initWaitTime) / buildTime;
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

    public void Train()
    {
        StartCoroutine(TrainSoldier());
    }

    IEnumerator TrainSoldier()
    {
        _tmpEvent.Data = SOLDIER;
        EventManager.TriggerEvent("OnProgressChange", _tmpEvent);
        _training = true;
        _boxCollider.enabled = false;
        // @TODO: cheaper option need it
        _smoke.SetActive(true);
        _anim.SetBool("Training", true);
 
        // Wait for training
        _nextWaitTime = Time.time + trainTime;
        _initWaitTime = Time.time;
        while (Time.time < _nextWaitTime)
        {
            _tmpEvent.Data = (Time.time - _initWaitTime) / trainTime;
            EventManager.TriggerEvent("OnProgressSoldier", _tmpEvent);
            yield return _yield;
        }

        // Soldier trained!
        Instantiate(_soldier, _trans.position + (_trans.forward * _spawnSoldiderDistance), Quaternion.identity);
        // @TODO: remove traces
        //Debug.DrawLine(_trans.position, _trans.position + (_trans.forward * _spawnSoldiderDistance), Color.red, 999999f);

        _anim.SetBool("Training", false);
        // @TODO: cheaper option need it
        _smoke.SetActive(false);
        _boxCollider.enabled = true;
        _training = false;

        _tmpEvent.Data = RESOURCES;
        EventManager.TriggerEvent("OnProgressChange", _tmpEvent);
    }

    public bool CanTrain()
    {
        return !_training;
    }
}
