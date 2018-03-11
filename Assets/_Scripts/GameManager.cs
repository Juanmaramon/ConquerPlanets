using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

public class GameManager : Singleton<GameManager>
{
    public int _maxBuildings;
    public int _currentBuildings;

	new void Awake () 
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
	}
}
