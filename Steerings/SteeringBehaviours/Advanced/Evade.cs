using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Evade : SteeringBehaviourTarget {
    [SerializeField]
    private float maxPrediction;

    override
    public Steering GetSteering() {
        return GetSteering(target, npc, maxAccel, maxPrediction, visibleRays, seekT);
    }

    public static Steering GetSteering(Body target, Body npc, float maxAccel, float maxPrediction, bool visibleRays = false, SeekT seekT = SeekT.REYNOLDS) {
        return -Pursue.GetSteering(target,npc,maxAccel,maxPrediction,visibleRays,seekT);
    }

}
