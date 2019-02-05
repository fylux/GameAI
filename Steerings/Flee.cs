using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : Steering {

    public Transform target;

    override
    public Vector3 Steer(Vector3 velocity)
    {

        var desiredVelocity = (transform.position - target.transform.position).normalized * MaxVelocity;

        if (visibleRays) drawRays(desiredVelocity);

        return (desiredVelocity - velocity);

    }

    private void drawRays(Vector3 dv)
    {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
    }
}
