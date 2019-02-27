using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviourTarget {

    override
    public Steering GetSteering() {
        return GetSteering(target.position, npc, maxAccel, visibleRays, seekT);
    }

    public static Steering GetSteering(Vector3 target, Body npc, float maxAccel, bool visibleRays = false, SeekT seekT = SeekT.REYNOLDS) {
        Steering steering = -Seek.GetSteering(target, npc, maxAccel, false, seekT);

        if (visibleRays) drawRays(steering.linear, npc.velocity);

        return steering;
    }
}
