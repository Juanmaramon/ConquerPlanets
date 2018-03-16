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

    float _horizontal = 0f;
    float _vertical = 0f;
    Vector3 _moveDir;
    bool _running = false;
    bool _offline = false;
    bool _hitBuildDetect = false;
    RaycastHit _hitCheck;
    BasicEvent _tmpEvent;
    GameObject _placeholderBuilding;

	private void Start()
	{
        _tmpEvent = new BasicEvent();
        _running = _offline = _hitBuildDetect = false;
        EventManager.StartListening<BasicEvent>("OnExitContextMenu", OnExitContextMenu);
	}

	void FixedUpdate()
	{
        if (_offline)
            return;

        if (Input.GetKeyDown("space"))
        {
            // Now real construction
            if (_placeholderBuilding && (GameManager.instance._currentBuildings < GameManager.instance._maxBuildings))
            {
                DoBuilding();
            }
            // Enter on context menu
            else
            {
                _offline = true;
                _contextMenu.EnterContextMenu();
            }
        }

        _horizontal = Input.GetAxis("Horizontal");
        _vertical = /*(_horizontal != 0f) ? 0 :*/ Input.GetAxis("Vertical");

        _vertical = (_vertical < 0f) ? 0 : _vertical;

        _moveDir = new Vector3(_horizontal, 0.0f, _vertical).normalized;

        _rigid.MovePosition(_rigid.position + _trans.TransformDirection(_moveDir) * speed * Time.fixedDeltaTime);

        Vector3 yRotation = Vector3.up * _horizontal * rotationSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(yRotation);
        Quaternion targetRotation = _rigid.rotation * deltaRotation;
        _rigid.MoveRotation(Quaternion.Slerp(_rigid.rotation, targetRotation, 150f * Time.deltaTime));
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.cyan;
        Debug.DrawRay(_trans.position, _trans.forward * _distanceBuildCheck, Color.red);
        Gizmos.DrawWireCube(_trans.position + _trans.forward * _distanceBuildCheck, _trans.localScale);
	}

	void Update()
    {
        if (_offline)
            return;
        
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

        if (_placeholderBuilding)
            _placeholderBuilding.transform.position = _trans.position + _trans.forward * _distanceBuildCheck;
    }

    IEnumerator Build()
    {
        // @TODO: refactor to cheaper options
        Destroy(_placeholderBuilding);
        // @TODO: refactor to cheaper options
        _fxFlare.SetActive(true);
        GameManager.instance._currentBuildings++;
        _offline = true;
        _anim.SetBool("Build", true);
        var spawnBuildingPosition = _trans.position + _trans.forward * _distanceBuildCheck;
        Instantiate(buildings[0], spawnBuildingPosition, Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up));

        yield return Yielders.Get(Building.buildTime);

        _tmpEvent.Data = GameManager.instance._currentBuildings;
        EventManager.TriggerEvent("OnNewBuilding", _tmpEvent);
        _offline = false;
        _anim.SetBool("Build", false);
        // @TODO: refactor to cheaper options
        _fxFlare.SetActive(false);
    }

    void OnExitContextMenu(BasicEvent e)
    {
        _offline = false;
    }

    public void StartMakeBuilding()
    {
        if ((GameManager.instance._currentBuildings < GameManager.instance._maxBuildings))
        {
            // @TODO: refactor to cheaper options
            _placeholderBuilding = Instantiate(buildings[1], _trans);
            _placeholderBuilding.transform.position = _trans.position + _trans.forward * _distanceBuildCheck;
        }
        else
        {
            // @TODO: report user building limit reached...
        }
    }

    void DoBuilding()
    {
        // Check if there is a collider of another building
        _hitBuildDetect = Physics.BoxCast(_trans.position, _trans.localScale, transform.forward, out _hitCheck, _trans.rotation, _distanceBuildCheck);
        if (!_hitBuildDetect)
        {
            StartCoroutine(Build()); 
        }
        else
        {
            // @TODO: report user not available zone...
        }
    }
}
