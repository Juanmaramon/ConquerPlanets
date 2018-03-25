using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] Transform _trans;
    static int numNeighboards = 3;
    public Vector3 _position;
    public Waypoint[] neighbors = new Waypoint[numNeighboards];
//    public bool visited = false;

	public Waypoint previous
	{
		get;
		set;
	}

	public float distance
	{
		get;
		set;
	}

	/*private void Awake()
	{
        _position = transform.position;
        //_trans = transform;
        //Invoke("Process", 5f);
	}*/

	/*private void Process()
	{
        float mindst = Mathf.Infinity;
        Transform minTrans = null;
        List<Transform> picked = new List<Transform>();
        for (int j = 0; j < numNeighboards; j++)
        {
            mindst = Mathf.Infinity;
            for (int i = 0; i < _pathRemove._waypoints.Length; i++)
            {
                if (_pathRemove._waypoints[i].name != gameObject.name)
                {
                    if (!picked.Contains(_pathRemove._waypoints[i].GetComponent<Transform>()))
                    {
                        if (Vector3.Distance(_pathRemove._waypoints[i]._position, _position) < mindst)
                        {
                            mindst = Vector3.Distance(_pathRemove._waypoints[i]._position, _position);
                            minTrans = _pathRemove._waypoints[i].GetComponent<Transform>();
                        }
                    }
                }
            }
            picked.Add(minTrans);
        }

        for (int i = 0; i < numNeighboards; i++)
        {
            neighbors[i] = picked[i].GetComponent<Waypoint>();
        }
	}*/

	/*void OnDrawGizmos()
	{
        if (neighbors == null)
			return;
        Gizmos.color = Color.red; //new Color (0f, 0f, 0f);
        for (int i = numNeighboards - 1; i >= 0; i--)
		{
            if (neighbors[i] != null)
                Gizmos.DrawLine (_trans.position, neighbors[i]._position);
		}

        //if (visited)
        //    Gizmos.color = Color.cyan;
        //else
            Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_trans.position, .5f);
	}*/
}
