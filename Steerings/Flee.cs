using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviour
{
    [SerializeField]
    private SeekType seekType = SeekType.REYNOLDS;

    [SerializeField]
    private Body target;

    override
    public Steering Steer()
    {
        Steering steering = new Steering();

        var desiredVelocity = (body.position - target.position).normalized * MaxAccel;

        if (seekType == SeekType.REYNOLDS)
            steering.lineal = (desiredVelocity - body.velocity);
        else
            steering.lineal = desiredVelocity;

        if (visibleRays)
            drawRays(steering.lineal, body.velocity);

        return steering;

    }

    private void drawRays(Vector3 dv, Vector3 v) {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, v.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(v.normalized * 2, z, Color.magenta);
    }
}
