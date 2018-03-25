using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PathManager : MonoBehaviour
{
    public float walkSpeed = 5.0f;

    [SerializeField] protected Transform _trans;

    private Stack<Vector3> currentPath;
    private Vector3 currentWaypointPosition;
    private float moveTimeTotal;
    private float moveTimeCurrent;
    protected bool pathPlanned;

    public void NavigateTo(Vector3 destination)
    {
        pathPlanned = true;
        currentPath = new Stack<Vector3>();
        var currentNode = FindClosestWaypoint(_trans.position);
        var endNode = FindClosestWaypoint(destination);
        if (currentNode == null || endNode == null || currentNode == endNode)
        {
            Stop();
            return;
        }
        var openList = new SortedList<float, Waypoint>();
        var closedList = new List<Waypoint>();
        openList.Add(0, currentNode);
        currentNode.previous = null;
        currentNode.distance = 0f;
        while (openList.Count > 0)
        {
            currentNode = openList.Values[0];
            openList.RemoveAt(0);
            var dist = currentNode.distance;
            closedList.Add(currentNode);
            if (currentNode == endNode)
            {
                break;
            }
            foreach (var neighbor in currentNode.neighbors)
            {
                neighbor.visited = true;
                if (closedList.Contains(neighbor) || openList.ContainsValue(neighbor))
                    continue;
                neighbor.previous = currentNode;
                neighbor.distance = dist + (neighbor._position - currentNode._position).magnitude;
                var distanceToTarget = (neighbor._position - endNode._position).magnitude;
                openList.Add(neighbor.distance + distanceToTarget, neighbor);
            }
        }
        if (currentNode == endNode)
        {
            while (currentNode.previous != null)
            {
                currentPath.Push(currentNode._position);
                currentNode = currentNode.previous;
            }
            currentPath.Push(_trans.position);
        }
        if (currentPath.Count == 0)
        {
            Stop();
        }
    }

    public void Stop()
    {
        OnTargedReached();
        pathPlanned = false;
        currentPath = null;
        moveTimeTotal = 0;
        moveTimeCurrent = 0;
    }

    protected void Update()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            if (moveTimeCurrent < moveTimeTotal)
            {
                moveTimeCurrent += Time.deltaTime;
                if (moveTimeCurrent > moveTimeTotal)
                    moveTimeCurrent = moveTimeTotal;
                _trans.position = Vector3.Lerp(currentWaypointPosition, currentPath.Peek(), moveTimeCurrent / moveTimeTotal);
                _trans.LookAt(currentPath.Peek(), _trans.up);
            }
            else
            {
                currentWaypointPosition = currentPath.Pop();
                if (currentPath.Count == 0)
                    Stop();
                else
                {
                    moveTimeCurrent = 0;
                    moveTimeTotal = (currentWaypointPosition - currentPath.Peek()).magnitude / walkSpeed;
                }
            }
        }
    }

    private Waypoint FindClosestWaypoint(Vector3 target)
    {
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        for (int i = Waypoints.instance._waypoints.Length - 1; i >= 0; i--)
        {
            var dist = (Waypoints.instance._waypoints[i]._position - target).magnitude;
            if (dist < closestDist)
            {
                closest = Waypoints.instance._waypoints[i].gameObject;
                closestDist = dist;
            }
        }
        if (closest != null)
        {
            return closest.GetComponent<Waypoint>();
        }
        return null;
    }

    protected abstract void OnTargedReached();
}