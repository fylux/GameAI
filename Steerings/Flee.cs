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
        return Flee.Steer(target.position, body, MaxAccel, visibleRays, seekType);
        /* steering = new Steering();

        var desiredVelocity = (body.position - target.position).normalized * MaxAccel;

        if (seekType == SeekType.REYNOLDS)
            steering.lineal = (desiredVelocity - body.velocity);
        else
            steering.lineal = desiredVelocity;

        if (visibleRays)
            drawRays(steering.lineal, body.velocity);

        return steering;*/

    }

    public static Steering Steer(Vector3 target, Body body, float MaxAccel, bool visibleRays = false, SeekType seekType = SeekType.REYNOLDS)
    {
        Steering steering = Seek.Steer(target, body, MaxAccel, false, seekType);
        steering.lineal *= -1f;
        steering.angular *= -1f;

        if (visibleRays)
            drawRays(steering.lineal, body.velocity);

        return steering;
    }

    private static void drawRays(Vector3 pos, Vector3 v) {
        Debug.DrawRay(pos, v.normalized * 2, Color.green);
    }
}
