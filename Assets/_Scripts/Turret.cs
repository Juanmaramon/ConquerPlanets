using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour 
{
    [SerializeField] Transform _trans;
    [SerializeField] Rigidbody _rigid;
    [SerializeField] Transform topTurret;
    [SerializeField] Transform target;
    [SerializeField] float rotationSmoothness = .1f;
    [SerializeField] float viewRadius;
    [SerializeField] LayerMask targetMask;
    [HideInInspector] List<Transform> visibleTargets = new List<Transform>();

    static float visibilityTime = 2f;
    static float staticTime = 2f;

	private void Start()
	{
        StartCoroutine("FindTargetsWithDelay", visibilityTime);
	}

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return Yielders.Get(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(_trans.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            visibleTargets.Add(targetsInViewRadius[i].transform);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Planet")
        {
            StartCoroutine(StaticBuilding());
        }
    }

    IEnumerator StaticBuilding()
    {
        yield return Yielders.Get(staticTime);

        // Make static
        _rigid.isKinematic = true;
    }

	void Update () 
    {
        if (visibleTargets.Count > 0)
        {
            Quaternion rotation = Quaternion.LookRotation(visibleTargets[0].position - topTurret.position);
            topTurret.rotation = Quaternion.Slerp(topTurret.rotation, rotation, Time.deltaTime * rotationSmoothness);
        }
	}


}
