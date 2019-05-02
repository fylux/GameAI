using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LWYG : SteeringBehaviourTarget {

 /*   [SerializeField]
    private float targetRadius;

    [SerializeField]
    private float slowRadius;*/

    [SerializeField]
    float timeToTarget = 0.1f;

    [SerializeField]
    float maxPrediction;

    public override Steering GetSteering() {
        return LWYG.GetSteering(target, npc, maxPrediction, npc.interiorAngle, npc.exteriorAngle, timeToTarget, visibleRays);
    }

    public static Steering GetSteering(Agent target, Agent npc, float maxPrediction, float targetRadius, float slowRadius, float timeToTarget, bool visibleRays)
    {
        float distance = Util.HorizontalDist(target.position, npc.position);
        float speed = npc.velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else
            prediction = distance / speed;

        Vector3 predTarget = target.position + (target.velocity * prediction);

        return Face.GetSteering(predTarget, npc, targetRadius, slowRadius, timeToTarget, visibleRays);
    }
}
