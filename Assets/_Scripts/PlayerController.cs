using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour 
{
    [SerializeField] float speed = 10f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField, HideInInspector] Transform _trans;
    [SerializeField, HideInInspector] Rigidbody _rigid;
    [SerializeField] Animator _anim;
    [SerializeField] GameObject[] buildings;
    [SerializeField] float _distanceBuildCheck = 5f;
    [SerializeField] UIContextMenu _contextMenu;
    [SerializeField] GameObject _fxFlare;
    [SerializeField] string _layerBuilding;    
    [SerializeField] string _defaultLayer;
    [SerializeField] SmoothFollow _camFollow;
    [SerializeField] BoxCollider _buildCheck;

    float _horizontal = 0f;
    float _vertical = 0f;
    Vector3 _moveDir;
    bool _running = false;
    bool _offline = false;
    bool _hitBuildDetect = false;
    RaycastHit _hitCheck;
    BasicEvent _tmpEvent;
    GameObject _placeholderBuilding;
    GameObject _placeholderTurret;
    bool _extract = false;
    bool _train = false;
    Transform _resourcesTrans;
    Building _trainingBunker;
    Vector3 _yRotation;
    Quaternion _deltaRotation;
    Quaternion _targetRotation;
    float _offsetTurretPlaceholder = 1.5f;
    bool _canBuild = true;

    // @TODO: const file with static values
    public static int BUILDING_RESOURCES = 100;
    public static int TURRET_RESOURCES = 200;
    public static int SOLDIER_RESOURCES = 50;

    // @TODO: refactor to string class
    static string LIMIT_BUILDINGS_REACHED = "Cannot build, maximum buildings reached";
    static string NOT_AVAILABLE_TO_BUILD = "Zone occupied, build in other place";
    static string BUILD_INSTRUCTIONS = "[Space] build, [ESC] cancel";
    static string EXTRACT_RESOURCES = "[Space] extract resources from debris";
    static string TRAIN_SOLDIERS = "[Space] train soldiers";

	private void Start()
	{
        _tmpEvent = new BasicEvent();
        _running = _offline = _hitBuildDetect = _extract = false;
        EventManager.StartListening<BasicEvent>("OnExitContextMenu", OnExitContextMenu);
	}

	private void OnDestroy()
	{
        EventManager.StopListening<BasicEvent>("OnExitContextMenu", OnExitContextMenu);
	}

	void FixedUpdate()
	{
        if (_offline)
            return;
        
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = /*(_horizontal != 0f) ? 0 :*/ Input.GetAxis("Vertical");

        _vertical = (_vertical < 0f) ? 0 : _vertical;

        _moveDir = new Vector3(_horizontal, 0.0f, _vertical).normalized;

        _rigid.MovePosition(_rigid.position + _trans.TransformDirection(_moveDir) * speed * Time.fixedDeltaTime);

        _yRotation = Vector3.up * _horizontal * rotationSpeed * Time.fixedDeltaTime;
        _deltaRotation = Quaternion.Euler(_yRotation);
        _targetRotation = _rigid.rotation * _deltaRotation;
        _rigid.MoveRotation(Quaternion.Slerp(_rigid.rotation, _targetRotation, 150f * Time.deltaTime));
    }

	/*private void OnDrawGizmos()
	{
        Gizmos.color = Color.cyan;
        Debug.DrawRay(_trans.position, _trans.forward * _distanceBuildCheck, Color.red);
        Gizmos.DrawWireCube(_trans.position + _trans.forward * _distanceBuildCheck, _trans.localScale * 2);
	}*/

	void Update()
    {
        if (_offline)
            return;

        // @TODO: refactor actions with a finite state machine!!
        if (Input.GetKeyDown("space"))
        {
            // Now real construction
            if (_placeholderBuilding && (GameManager.instance._currentBuildings < GameManager.instance._maxBuildings))
            {
                DoBuilding();
            }
            else if (_placeholderBuilding && (GameManager.instance._currentBuildings >= GameManager.instance._maxBuildings))
            {
                // @TODO: report user max buldings reached

            }
            else if (_placeholderTurret)
            {
                DoTurret();
            }
            else if (_extract)
            {
                ExtractResources();
            }
            else if (_train)
            {
                _offline = true;
                _contextMenu.EnterContextMenu(false);
            }
            // Enter on context menu
            else
            {
                _offline = true;
                _contextMenu.EnterContextMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && (_placeholderBuilding || _placeholderTurret))
        {
            // @TODO: refactor to cheaper options
            Destroy(_placeholderBuilding);
            Destroy(_placeholderTurret);
            _buildCheck.enabled = false;
        }

        // Update animation state
        if (_moveDir != Vector3.zero && !_running)
        {
            _running = true;
            _anim.SetTrigger("Run");
        }
        else if (_running && _moveDir == Vector3.zero)
        {
            _running = false;
            _anim.SetTrigger("Stop");
        }
    }

    IEnumerator BuildBuilding()
    {
        _buildCheck.enabled = false;
        // @TODO: refactor to cheaper options
        Destroy(_placeholderBuilding);
 
        GameManager.instance._currentBuildings++;
        _offline = true;
        _anim.SetBool("Build", true);
        var spawnBuildingPosition = _trans.position + _trans.forward * _distanceBuildCheck;

        gameObject.layer = LayerMask.NameToLayer(_layerBuilding);

        Instantiate(buildings[0], spawnBuildingPosition, Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up));

        // Little animation to get worker close building
        _trans.position += _trans.forward;

        // @TODO: refactor to cheaper options
        _fxFlare.SetActive(true);

        yield return Yielders.Get(Building.buildTime);

        _tmpEvent.Data = GameManager.instance._currentBuildings;
        EventManager.TriggerEvent("OnNewBuilding", _tmpEvent);
        _offline = false;
        _anim.SetBool("Build", false);

        // @TODO: refactor to cheaper options
        _fxFlare.SetActive(false);

        // Little animation to restore worker position
        _trans.position -= _trans.forward * 2;
        gameObject.layer = LayerMask.NameToLayer(_defaultLayer);

        // UI refresh
        GameManager.instance._resources -= BUILDING_RESOURCES;
        _tmpEvent.Data = GameManager.instance._resources;
        EventManager.TriggerEvent("OnNewResources", _tmpEvent);
    }

    IEnumerator BuildTurret()
    {
        _buildCheck.enabled = false;
        // @TODO: refactor to cheaper options
        Destroy(_placeholderTurret);

        //GameManager.instance._currentBuildings++;
        _offline = true;
        _anim.SetBool("Build", true);
        var spawnBuildingPosition = _trans.position + _trans.forward * _distanceBuildCheck;

        gameObject.layer = LayerMask.NameToLayer(_layerBuilding);

        Instantiate(buildings[2], spawnBuildingPosition, Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up));

        // Little animation to get worker close building
        _trans.position += _trans.forward;

        // @TODO: refactor to cheaper options
        _fxFlare.SetActive(true);

        yield return Yielders.Get(Turret.buildingTime);

        //_tmpEvent.Data = GameManager.instance._currentBuildings;
        //EventManager.TriggerEvent("OnNewBuilding", _tmpEvent);
        _offline = false;
        _anim.SetBool("Build", false);

        // @TODO: refactor to cheaper options
        _fxFlare.SetActive(false);

        // Little animation to restore worker position
        _trans.position -= _trans.forward * 2;
        gameObject.layer = LayerMask.NameToLayer(_defaultLayer);

        // UI refresh
        GameManager.instance._resources -= TURRET_RESOURCES;
        _tmpEvent.Data = GameManager.instance._resources;
        EventManager.TriggerEvent("OnNewResources", _tmpEvent);
    }

    void OnExitContextMenu(BasicEvent e)
    {
        _offline = false;
    }

    public void StartTrainingSoldier()
    {
        if (_trainingBunker.CanTrain())
            _trainingBunker.Train();

        _train = false;
        _trainingBunker = null;        
    }

    public void StartMakeBuilding()
    {
        if ((GameManager.instance._currentBuildings < GameManager.instance._maxBuildings))
        {
            // @TODO: refactor to cheaper options
            _placeholderBuilding = Instantiate(buildings[1], _trans);
            _placeholderBuilding.transform.position = _trans.position + _trans.forward * _distanceBuildCheck;
            _tmpEvent.Data = BUILD_INSTRUCTIONS;
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
            _buildCheck.enabled = true;
        }
        else
        {
            _tmpEvent.Data = LIMIT_BUILDINGS_REACHED;
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
        }
    }

    public void StartMakeTurret()
    {
  //      if ((GameManager.instance._currentBuildings < GameManager.instance._maxBuildings))
  //      {
            // @TODO: refactor to cheaper options
            _placeholderTurret = Instantiate(buildings[3], _trans);
            _placeholderTurret.transform.position = _trans.position + _trans.forward * _distanceBuildCheck + (_trans.up * _offsetTurretPlaceholder);
            _tmpEvent.Data = BUILD_INSTRUCTIONS;
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
            _buildCheck.enabled = true;
/*        }
        else
        {
            _tmpEvent.Data = LIMIT_BUILDINGS_REACHED;
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
        }*/
    }

    void DoBuilding()
    {
        // Check if there is a collider of another building
        if (_canBuild)
        {
            StartCoroutine(BuildBuilding()); 
        }
        else
        {
            _tmpEvent.Data = NOT_AVAILABLE_TO_BUILD;
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
        }
    }

    void DoTurret()
    {
        // Check if there is a collider of another turret
        if (_canBuild)
        {
            StartCoroutine(BuildTurret());
        }
        else
        {
            _tmpEvent.Data = NOT_AVAILABLE_TO_BUILD;
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
        }       
    }

	private void OnTriggerEnter(Collider other)
	{
        if ((_placeholderTurret != null) || (_placeholderBuilding != null))
        {
            if (other.tag == "Junk" || other.tag == "Bunker" || other.tag == "Soldier")
            {
                _canBuild = false;
            }

            return;
        }

        if (other.tag == "Junk")
        {
            _tmpEvent.Data = EXTRACT_RESOURCES;
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
            _extract = true;
            _resourcesTrans = other.transform;
            _resourcesTrans.GetComponent<Junk>().ToogleBase(true);
        }
        else if (other.tag == "Bunker")
        {
            _tmpEvent.Data = TRAIN_SOLDIERS;
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
            _train = true;
            _trainingBunker = other.GetComponent<Building>();
            _trainingBunker.ToogleBase(true);
        }
	}

    private void OnTriggerExit(Collider other)
    {
        if ((_placeholderTurret != null) || (_placeholderBuilding != null))
        {
            if (other.tag == "Junk" || other.tag == "Bunker" || other.tag == "Soldier")
            {
                _canBuild = true;
            }

            return;
        }

        if (other.tag == "Junk")
        {
            _tmpEvent.Data = "";
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
            _extract = false;
            _resourcesTrans.GetComponent<Junk>().ToogleBase(false);
            _resourcesTrans = null;
        }
        else if (other.tag == "Bunker")
        {
            _train = false;
            _tmpEvent.Data = "";
            EventManager.TriggerEvent("OnNewExplanation", _tmpEvent);
            _trainingBunker.ToogleBase(false);
        }
    }

    void ExtractResources()
    {
        StartCoroutine(Extract()); 
    }

    IEnumerator Extract()
    {
        _camFollow.enabled = false;
        var consumeReturn = _resourcesTrans.GetComponent<Junk>().ConsumeJunk();
        // @TODO: put real resources from junk
        GameManager.instance._resources += consumeReturn[0];
        _offline = true;
        _anim.SetBool("Build", true);
  
        // Little animation to get worker close building
        gameObject.layer = LayerMask.NameToLayer(_layerBuilding);
        _trans.LookAt(_resourcesTrans.position);
        //_trans.position += _trans.forward;
 
        // @TODO: refactor to cheaper options
        _fxFlare.SetActive(true);

        yield return Yielders.Get(consumeReturn[1]);

        _tmpEvent.Data = GameManager.instance._resources;
        EventManager.TriggerEvent("OnNewResources", _tmpEvent);
        _offline = _extract = false;
        _camFollow.enabled = true;
        _anim.SetBool("Build", false);

        // @TODO: refactor to cheaper options
        _fxFlare.SetActive(false);

        // Little animation to restore worker position
        //_trans.position -= _trans.forward;
        gameObject.layer = LayerMask.NameToLayer(_defaultLayer);
    }
}
