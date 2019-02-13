using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : Steering {

    public Transform target;

    private void Start()
    {

    }

    override
    public void Steer(Vector3 velocity)
    {

        var desiredVelocity = (transform.position - target.transform.position).normalized * MaxVelocity;

        if (visibleRays) drawRays(desiredVelocity);

        vl = (desiredVelocity - velocity);

    }

    private void drawRays(Vector3 dv)
    {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
    }
}
