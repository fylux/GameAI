using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : SteeringBehaviourTarget {

    override
    public Steering GetSteering() {
        return GetSteering(target.position, npc, maxAccel, visibleRays, seekT);
    }

    public static Steering GetSteering(Vector3 target, Body npc, float maxAccel, bool visibleRays = false, SeekT seekT = SeekT.REYNOLDS) {
        Steering steering = new Steering();

        var desiredVelocity = (target - npc.position).normalized * maxAccel;

        if (seekT == SeekT.REYNOLDS)
            steering.linear = (desiredVelocity - npc.velocity);
        else
            steering.linear = desiredVelocity;

        if (visibleRays)
            drawRays(npc.position, steering.linear);

        return steering;
    }
}
