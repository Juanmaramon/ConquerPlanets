﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junk : MonoBehaviour 
{
    [SerializeField] int resources;
    [SerializeField] Animator _anim;
    public static float extractTime = 5f;

    float _nextExtractTime = 0f;
    float _initExtractTime = 0f;
    BasicEvent _tmpEvent;
    WaitForSeconds _yield;
    static float yieldTime = 0.2f;

	private void Start()
	{
        _yield = Yielders.Get(yieldTime);
        _tmpEvent = new BasicEvent();
	}

	public int ConsumeJunk()
    {
        _anim.SetTrigger("Consumed");
  
        StartCoroutine(ExtractProcess());

        return resources;
    }

	public void Destroy()
	{
        Destroy(gameObject);
	}

    IEnumerator ExtractProcess()
    {
        // Wait for build
        _nextExtractTime = Time.time + extractTime;
        _initExtractTime = Time.time;
        while (Time.time < _nextExtractTime)
        {
            _tmpEvent.Data = (Time.time - _initExtractTime) / extractTime;
            EventManager.TriggerEvent("OnProgressExtract", _tmpEvent);
            yield return _yield;
        }   

        // Consumed resource!
    }
}
