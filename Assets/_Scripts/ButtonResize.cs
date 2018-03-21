using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonResize : MonoBehaviour 
{
    [SerializeField] RectTransform _trans;
    [SerializeField] float _scales = 1.05f;

    Vector3 _initScale;

	private void Awake()
	{
        _initScale = _trans.localScale;
	}

	public void Hover()
    {
        _trans.localScale *= _scales;
    }

    public void HoverOut()
    {
        _trans.localScale = _initScale;
    }

    public void ResetScale()
    {
        _trans.localScale = _initScale;
    }
}
