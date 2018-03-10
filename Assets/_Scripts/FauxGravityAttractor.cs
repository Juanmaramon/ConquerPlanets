using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityAttractor : MonoBehaviour 
{
    [SerializeField] float gravity = -10;

    public void Attract (Transform body)
    {
        // Compute distance vector between body and planet (more distance more attraction force)
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;
        // Apply force
        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);
        // Deal with rotation smoothly
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 50 * Time.deltaTime);
    }
}
