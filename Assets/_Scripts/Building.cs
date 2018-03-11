using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] Material[] materials;
    [SerializeField] Renderer _rend;
    [SerializeField] Rigidbody _rigid;
    [SerializeField] Collider _collider;
    public static float buildTime = 5f;
    static float staticTime = 1f;

    // Use this for initialization
    void Start()
    {
        OnConstruction();
        StartCoroutine(BuildProcess());
    }

    void OnConstruction()
    {
        _rend.material = materials[1];
    }

	void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.tag == "Planet") 
        {
            StartCoroutine(StaticBuilding());
        }
	}

    IEnumerator BuildProcess()
    {
        _collider.enabled = true;

        // Wait for build
        yield return Yielders.Get(buildTime);

        // Building complete!   
        _rend.material = materials[0];
    }

    IEnumerator StaticBuilding()
    {
        yield return Yielders.Get(staticTime);

        // Make static
        _rigid.isKinematic = true;
    }
}
