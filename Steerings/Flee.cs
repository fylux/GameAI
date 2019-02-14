using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviour
{
    public Body target;

    private void Start()
    {

    }

    override
    public Steering Steer(Vector3 velocity)
    {
        Steering steering = new Steering();

        var desiredVelocity = (body.position - target.position).normalized * MaxAccel;

        if (visibleRays) drawRays(desiredVelocity);

        steering.lineal = (desiredVelocity - velocity);

        return steering;
    }

    private void drawRays(Vector3 dv)
    {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, transform.forward * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(transform.position + transform.forward * 2, z, Color.magenta);
    }
}
