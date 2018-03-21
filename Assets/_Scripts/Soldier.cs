using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : MonoBehaviour 
{
    [SerializeField] Animator _anim;

	private void Start()
	{
        var rand = Random.Range(0, 5);
        float res = 0f;
        switch (rand)
        {
            case 1:
                res = 0.25f;
                break;
            case 2:
                res = 0.5f;
                break;
            case 3:
                res = 0.75f;
                break;
            case 4:
                res = 1f;
                break;
        }

        _anim.SetFloat("Blend", res);
	}
}
