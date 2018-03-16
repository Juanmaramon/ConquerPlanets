using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContextMenu : MonoBehaviour 
{
    [SerializeField] GameObject _contextMenu;
    [SerializeField] Animator _anim;

	public void ExitContextMenu()
    {
        EventManager.TriggerEvent("OnExitContextMenu"); 
        _contextMenu.SetActive(false);
    }

    public void EnterContextMenu()
    {
        _contextMenu.SetActive(true);
        _anim.SetTrigger("Open");
    }
}
