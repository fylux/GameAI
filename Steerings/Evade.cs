using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Evade : SteeringBehaviourTarget {
    [SerializeField]
    private float maxPrediction;

    override
    public Steering getSteering() {
        return getSteering(target, npc, maxAccel, maxPrediction, visibleRays, seekT);
    }

    public static Steering getSteering(Body target, Body npc, float maxAccel, float maxPrediction, bool visibleRays, SeekT seekT) {
        return -Pursue.getSteering(target,npc,maxAccel,maxPrediction,visibleRays,seekT);
    }

}
