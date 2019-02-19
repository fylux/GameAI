using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SeekType
{
    MILLINGTON, REYNOLDS
};

public class Seek : SteeringBehaviourTarget
{
    [SerializeField]
    private SeekType seekType = SeekType.REYNOLDS;


    override
    public Steering Steer()
    {
        Steering steering = new Steering();

        var desiredVelocity = (target.position -  body.position).normalized * MaxAccel;

        if (seekType == SeekType.REYNOLDS)
            steering.lineal = (desiredVelocity - body.velocity);
        else
            steering.lineal = desiredVelocity;

        if (visibleRays)
            drawRays(steering.lineal, body.velocity);

        return steering;

    }

    public static Steering Steer(Vector3 punto, Body body, SeekType st, bool visibleRays, float MaxAccel)
    {
        Steering steering = new Steering();

        Debug.Log(punto);
        var desiredVelocity = (punto - body.position).normalized * MaxAccel;

        if (st == SeekType.REYNOLDS)
            steering.lineal = (desiredVelocity - body.velocity);
        else
            steering.lineal = desiredVelocity;

        return steering;

    }


    private void drawRays(Vector3 dv, Vector3 v) {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, v.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(v.normalized * 2, z, Color.magenta);
    }
}
