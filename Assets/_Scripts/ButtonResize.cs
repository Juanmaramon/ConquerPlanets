using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonResize : MonoBehaviour 
{
    [SerializeField] RectTransform _trans;

    public void Hover()
    {
        _trans.localScale *= 1.05f;
    }

    public void HoverOut()
    {
        _trans.localScale /= 1.05f;
    }
}
