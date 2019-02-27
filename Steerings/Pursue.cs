using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pursue : SteeringBehaviourTarget {

    [SerializeField]
    private float maxPrediction;

    override
    public Steering getSteering() {
        return getSteering(target, npc, maxAccel, maxPrediction, visibleRays, seekT);
    }

    public static Steering getSteering(Body target, Body npc, float maxAccel, float maxPrediction, bool visibleRays, SeekT seekT) {
        Vector3 direction = target.position - npc.position;
        float distance = direction.magnitude;
        float speed = npc.velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else
            prediction = distance / speed;

        Vector3 pred_target = target.position + (target.velocity * prediction);

        return Seek.getSteering(pred_target, npc, maxAccel, visibleRays, seekT);
    }
}
