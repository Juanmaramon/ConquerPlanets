using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIContextMenu : MonoBehaviour 
{
    [SerializeField] GameObject _contextMenu;
    [SerializeField] Animator _anim;
    [SerializeField] Selectable _selected;
    [SerializeField] Text _explanationSelected;

    int _idxSelected = 0;
    float _waitIdleTime = 2f;
    bool _useArrowKeys = false;
    EventSystem _eventSystem;
    Selectable _firstSelected;

    // @TODO: refactor to string class
    static string[] _explanationsSelected = { 
        "Make a building to create sodiers", 
        "Turrets can defend places statically", 
        "THE RAY it's a powerfull weapon",
        "Exits this menu"};

    static string _idleArrowExplanation = "Use arrow keys or A/D to select what to do";

	private void Awake()
	{
        _eventSystem = EventSystem.current;
        _firstSelected = _selected;
	}

	public void ExitContextMenu()
    {
        HoverOutButton(_selected);
        _eventSystem.SetSelectedGameObject(null);
        EventManager.TriggerEvent("OnExitContextMenu");
        _explanationSelected.text = "";
        _contextMenu.SetActive(false);
    }

    public void EnterContextMenu()
    {
        _idxSelected = 0;
        _useArrowKeys = false;
        _explanationSelected.text = _explanationsSelected[_idxSelected];
        _contextMenu.SetActive(true);
        _selected = _firstSelected;
        HoverButton(_selected);
        _anim.SetTrigger("Open");

        StartCoroutine(ShowIdleExplanation());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            _useArrowKeys = true;
            Selectable next = _selected.FindSelectableOnRight();

            if (next != null)
            {
                HoverOutButton(_selected);
                HoverButton(next);
                _selected = next;
                _idxSelected = ((_idxSelected + 1) % (_explanationsSelected.Length));
                _explanationSelected.text = _explanationsSelected[_idxSelected];
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            _useArrowKeys = true;
            Selectable next = _selected.FindSelectableOnLeft();

            if (next != null)
            {
                HoverOutButton(_selected);
                HoverButton(next);
                _selected = next;
                _idxSelected = (_idxSelected == 0) ? (_explanationsSelected.Length - 1) : (_idxSelected - 1);
                _explanationSelected.text = _explanationsSelected[_idxSelected];
            }
        }
        else if (Input.GetKeyDown("space"))
        {
            _eventSystem.SetSelectedGameObject(_selected.gameObject);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitContextMenu();
        }
    }

    void HoverOutButton(Selectable selected)
    {
        selected.GetComponent<ButtonResize>().HoverOut();
        selected.GetComponent<Image>().color = selected.GetComponent<Button>().colors.normalColor;
    }

    void HoverButton(Selectable next)
    {
        next.GetComponent<ButtonResize>().Hover();
        next.GetComponent<Image>().color = next.GetComponent<Button>().colors.highlightedColor;        
    }

    IEnumerator ShowIdleExplanation()
    {
        yield return Yielders.Get(_waitIdleTime);
        // If player don't use arrow keys, explain it
        if (!_useArrowKeys)
            _explanationSelected.text = _idleArrowExplanation;
    }
}
