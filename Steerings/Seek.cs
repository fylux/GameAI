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
    public Steering Steer() {
        return Steer(target.position, body, MaxAccel, visibleRays, seekType);
    }

    public static Steering Steer(Vector3 target, Body body, float MaxAccel, bool visibleRays = false, SeekType st = SeekType.REYNOLDS)
    {
        Steering steering = new Steering();

        var desiredVelocity = (target - body.position).normalized * MaxAccel;

        if (st == SeekType.REYNOLDS)
            steering.lineal = (desiredVelocity - body.velocity);
        else
            steering.lineal = desiredVelocity;

        if (visibleRays)
            drawRays(body.position, steering.lineal);

        return steering;
    }


    private static void drawRays(Vector3 pos,  Vector3 v) {
        Debug.DrawRay(pos, v.normalized * 2, Color.green);
    }
}
