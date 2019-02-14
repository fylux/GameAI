using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SeekType
{
    SEEK_MILLINGTON, SEEK_REYNOLDS
};

public class Seek : SteeringBehaviour
{
    [SerializeField]
    private SeekType seekType;
    public Body target;

    private void Start()
    {
        base.Start();
        Debug.Log(body);
    }

    override
    public Steering Steer(Vector3 velocity)
    {
        Steering steering = new Steering();

        var desiredVelocity = (target.position -  body.position).normalized * MaxAccel;

        if (visibleRays) drawRays(desiredVelocity, velocity);

        steering.lineal = (desiredVelocity - velocity);
        return steering;

    }

    private void drawRays(Vector3 dv, Vector3 v) {
        var z = dv.normalized * 2 - transform.forward * 2;
        Debug.DrawRay(transform.position, v.normalized * 2, Color.green);
        Debug.DrawRay(transform.position, dv.normalized * 2, Color.blue);
        Debug.DrawRay(v.normalized * 2, z, Color.magenta);
    }
}
