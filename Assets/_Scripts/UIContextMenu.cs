using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIContextMenu : MonoBehaviour 
{
    [SerializeField] GameObject _contextMenu;
    [SerializeField] Animator _anim;
    [SerializeField] Selectable _generalSelected;
    [SerializeField] Selectable _detailSelected;
    [SerializeField] Text _explanationSelected;
    [SerializeField] PlayerController _player;
    [SerializeField] GameObject _generalMenu;
    [SerializeField] GameObject _detailMenu;

    int _idxSelected = 0;
    float _waitIdleTime = 2f;
    float _waitActiveTime = 0.2f;
    bool _useArrowKeys = false;
    EventSystem _eventSystem;
    Selectable _firstGeneralSelected;
    Selectable _firstDetailSelected;
    Selectable _selected1;
    bool _active = false;
    bool _general = true;

    // @TODO: refactor to string class
    static string[] _explanationsGeneralSelected = { 
        "Make a building to create sodiers", 
        "Turrets can defend places statically", 
        "THE RAY it's a powerfull weapon",
        "Exits this menu"
    };
    static string[] _explanationsDetailSelected = {
        "Train soldiers to defeat/attack bases",
        "",
        "",
        "Exits this menu"
    };
    static string _idleArrowExplanation = "Use arrow keys or A/D to select what to do";
    static string _notEnoughtResources = "<color=#ff0000ff>Not enought resources</color>";

	private void Awake()
	{
        _eventSystem = EventSystem.current;
        _firstGeneralSelected = _generalSelected;
        _firstDetailSelected = _detailSelected;
	}

	public void ExitContextMenu()
    {
        HoverOutButton(_selected1);
        _eventSystem.SetSelectedGameObject(null);
        EventManager.TriggerEvent("OnExitContextMenu");
        _explanationSelected.text = "";
        _active = false;
        _contextMenu.SetActive(false);
    }

    public void EnterContextMenu(bool general = true)
    {
        _general = general;
        _contextMenu.SetActive(true);
        _active = false;
        _idxSelected = 0;
        _useArrowKeys = false;
        _generalMenu.SetActive(_general);
        _detailMenu.SetActive(!_general);
        _explanationSelected.text = _general ? _explanationsGeneralSelected[_idxSelected] : _explanationsDetailSelected[_idxSelected];
        _selected1 = _general ? _firstGeneralSelected : _firstDetailSelected;
        HoverButton(_selected1);
        _anim.SetTrigger("Open");

        StartCoroutine(ShowIdleExplanation());
        StartCoroutine(ActiveMenu());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            _useArrowKeys = true;
            Selectable next = _selected1.FindSelectableOnRight();

            if (next != null)
            {
                HoverOutButton(_selected1);
                HoverButton(next);
                _selected1 = next;
                _idxSelected = _general ? ((_idxSelected + 1) % (_explanationsGeneralSelected.Length)) : ((_idxSelected + 1) % (_explanationsDetailSelected.Length));
                _explanationSelected.text = _general ? _explanationsGeneralSelected[_idxSelected] : _explanationsDetailSelected[_idxSelected];
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            _useArrowKeys = true;
            Selectable next = _selected1.FindSelectableOnLeft();

            if (next != null)
            {
                HoverOutButton(_selected1);
                HoverButton(next);
                _selected1 = next;
                if (_general)
                    _idxSelected = (_idxSelected == 0) ? (_explanationsGeneralSelected.Length - 1) : (_idxSelected - 1);
                else
                    _idxSelected = (_idxSelected == 0) ? (_explanationsDetailSelected.Length - 1) : (_idxSelected - 1);
                _explanationSelected.text = _general ? _explanationsGeneralSelected[_idxSelected] : _explanationsDetailSelected[_idxSelected];
            }
        }
        else if (Input.GetKeyDown("space"))
        {
            // Don't show explanation about UI navigation
            //_useArrowKeys = true;
            // Check resources
            if (_active)
                switch (_selected1.name)
                {
                    case "Buildings":
                        if (GameManager.instance._resources < PlayerController.BUILDING_RESOURCES)
                        {
                            _explanationSelected.text = _notEnoughtResources;
                            return;
                        }
                        else
                        {
                            _player.StartMakeBuilding();
                            ExitContextMenu();
                        }
                        break;
                    case "Turrets":
                        if (GameManager.instance._resources < PlayerController.TURRET_RESOURCES)
                        {
                            _explanationSelected.text = _notEnoughtResources;
                            return;
                        }
                        else
                        {
                            _player.StartMakeTurret();
                            ExitContextMenu();
                        }
                        break;
                    case "Soldiers":
                        if (GameManager.instance._resources < PlayerController.SOLDIER_RESOURCES)
                        {
                            _explanationSelected.text = _notEnoughtResources;
                            return;
                        }
                        else
                        {
                            _player.StartTrainingSoldier();
                            ExitContextMenu();
                        }                        
                        break;
                    case "ExitContextMenu":
                        ExitContextMenu();
                        break;
                }
            // Active button
            //_eventSystem.SetSelectedGameObject(_selected.gameObject);
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

    IEnumerator ActiveMenu()
    {
        yield return Yielders.Get(_waitActiveTime);

        _active = true;
    }
}
