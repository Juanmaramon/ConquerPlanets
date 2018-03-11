using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] Rigidbody _rigid;
    [SerializeField] Collider _collider;
    [SerializeField] Animator _anim;
    public static float buildTime = 5f;
    static float staticTime = 2f;

    // Use this for initialization
    void Start()
    {
        OnConstruction();
        StartCoroutine(BuildProcess());
    }

    void OnConstruction()
    {
        _anim.SetTrigger("Building");
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
    }

    IEnumerator StaticBuilding()
    {
        yield return Yielders.Get(staticTime);

        // Make static
        _rigid.isKinematic = true;
    }
}
