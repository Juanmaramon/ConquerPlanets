using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

public class GameManager : Singleton<GameManager>
{
    public int _maxBuildings;
    public int _currentBuildings;
    public GameObject[] _junks;
    public int _resources = 100;
    public bool DEBUG;

    static int _numberJunks = 5;
    int _activeJunk;
    Dictionary<int, int> _activesJunks = new Dictionary<int, int>();
    bool _found = false;
    int _idx;

	new void Awake () 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        base.Awake();
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i <= 5; i++)
        {
            _activeJunk = Random.Range(0, _junks.Length - 1);
            if (!_activesJunks.ContainsKey(_activeJunk))
            {
                _activesJunks.Add(_activeJunk, 1);
                _junks[_activeJunk].SetActive(true);
            }
        }

        for (int i = 0; i < (_numberJunks - _activesJunks.Count); i++)
        {
            _found = false;
            _idx = 0;
            while (!_found)
            {
                if (_activesJunks.ContainsKey(_idx))
                {
                    _idx++;
                }
                else
                {
                    _activesJunks.Add(_idx, 1);
                    _junks[_idx].SetActive(true);
                    _found = true;
                }
            }
        }
	}
}
