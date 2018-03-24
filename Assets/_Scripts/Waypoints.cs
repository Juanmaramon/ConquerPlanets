using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Utilities;

public class Waypoints : Singleton<Waypoints>
{
    public Waypoint[] _waypoints = new Waypoint[PlaceWaypoints.numPoints];
}
