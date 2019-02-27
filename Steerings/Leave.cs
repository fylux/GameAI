using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leave : SteeringBehaviourTarget {

    override
    public Steering getSteering() {
        return getSteering(target.position, npc, maxAccel, visibleRays, seekT);
    }

    public static Steering getSteering(Vector3 target, Body npc, float maxAccel, bool visibleRays, SeekT seekT) {
        return Flee.getSteering(target, npc, maxAccel, visibleRays, seekT);
    }
}
