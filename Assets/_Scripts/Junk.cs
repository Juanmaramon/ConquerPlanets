using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junk : MonoBehaviour 
{
    [SerializeField] int resources;
    [SerializeField] Animator _anim;
    [SerializeField] Renderer _rend;
    public int extractTime = 5;

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

	public int[] ConsumeJunk()
    {
        _rend.material.SetFloat("_Mode", 2.0f);
        _anim.SetTrigger("Consumed");
  
        StartCoroutine(ExtractProcess());

        return new int[] { resources, extractTime };
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

        Destroy();
    }
}
