using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceWaypoints : MonoBehaviour 
{
    [SerializeField] GameObject prefab;
    [SerializeField] float sphereRadius = 15f;
    public static int numPoints = 150;
    [SerializeField] Transform root;
    [SerializeField] Waypoints _waypoints;

	private void Start()
	{
        for (int i = 0; i < numPoints; i++)
        {
            Ray ray = new Ray(transform.position, Random.insideUnitSphere);
            Vector3 sphereCenter = transform.position;
            float distance;
            if (CheckRaySphere(ray, sphereCenter, sphereRadius, out distance))
            {
                // did hit sphere.
                Vector3 position = ray.GetPoint(distance);
                Vector3 normal = (position - sphereCenter).normalized;
                Quaternion rotation = Quaternion.LookRotation(normal);
                var sphere = Instantiate(prefab, position, rotation);
                sphere.name = i.ToString();
                sphere.transform.SetParent(root);
                _waypoints._waypoints[i] = sphere.GetComponent<Waypoint>();
            }
        }
	}

	bool CheckRaySphere(Ray ray, Vector3 sphereOrigin, float sphereRadius, out float distance)
    {
        Vector3 localPoint = ray.origin - sphereOrigin;
        float temp = -Vector3.Dot(localPoint, ray.direction);
        float det = temp * temp - Vector3.Dot(localPoint, localPoint) + sphereRadius * sphereRadius;
        if (det < 0) { distance = Mathf.Infinity; return false; }
        det = Mathf.Sqrt(det);
        float intersection0 = temp - det;
        float intersection1 = temp + det;
        if (intersection0 >= 0)
        {
            if (intersection1 >= 0)
            {
                distance = Mathf.Min(intersection0, intersection1); return true;
            }
            else
            {
                distance = intersection0; return true;
            }
        }
        else if (intersection1 >= 0)
        {
            distance = intersection1; return true;
        }
        else
        {
            distance = Mathf.Infinity; return false;
        }
    }
}
