using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : SteeringBehaviourTarget {

    override
    public Steering getSteering() {
        return getSteering(target.position, npc, maxAccel, visibleRays, seekT);
    }

    public static Steering getSteering(Vector3 target, Body npc, float maxAccel, bool visibleRays, SeekT seekT) {
        Steering steering = -Seek.getSteering(target, npc, maxAccel, false, seekT);

        if (visibleRays) drawRays(steering.linear, npc.velocity);

        return steering;
    }
}
