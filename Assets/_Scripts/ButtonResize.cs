using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonResize : MonoBehaviour 
{
    [SerializeField] RectTransform _trans;

    Vector3 _initScale;

	private void Start()
	{
        _initScale = _trans.localScale;
	}

	public void Hover()
    {
        _trans.localScale *= 1.05f;
    }

    public void HoverOut()
    {
        _trans.localScale /= 1.05f;
    }

    public void ResetScale()
    {
        _trans.localScale = _initScale;
    }
}
