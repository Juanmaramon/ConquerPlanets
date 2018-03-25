using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Junk : MonoBehaviour 
{
    [SerializeField] int resources;
    [SerializeField] Animator _anim;
    [SerializeField] Renderer _rend;
    [SerializeField] GameObject _base;
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
        Material _mat = _rend.material;
        _mat.SetFloat("_Mode", 2.0f);
        _mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _mat.SetInt("_ZWrite", 0);
        _mat.DisableKeyword("_ALPHATEST_ON");
        _mat.EnableKeyword("_ALPHABLEND_ON");
        _mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        _mat.renderQueue = 3000;
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

    public void ToogleBase(bool _active)
    {
        _base.SetActive(_active);
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position +  transform.up, .5f);
	}
}
