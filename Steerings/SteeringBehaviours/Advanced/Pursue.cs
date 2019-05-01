using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Pursue : SteeringBehaviourTarget {

    [SerializeField]
    float maxPrediction;

    override
    public Steering GetSteering() {
        return GetSteering(target, npc, maxAccel, maxPrediction, visibleRays);
    }

    public static Steering GetSteering(Agent target, Agent npc, float maxAccel, float maxPrediction, bool visibleRays = false) {
        float distance =  Util.HorizontalDist(target.position, npc.position);
        float speed = npc.velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else
            prediction = distance / speed;

        Vector3 predTarget = target.position + (target.velocity * prediction);

        return Seek.GetSteering(predTarget, npc, maxAccel, visibleRays);
    }

    private void OnDrawGizmos()
    {
        float distance = Util.HorizontalDist(target.position, npc.position);
        float speed = npc.velocity.magnitude;

        float prediction;
        if (speed <= distance / maxPrediction)
            prediction = maxPrediction;
        else
            prediction = distance / speed;

        Vector3 predTarget = target.position + (target.velocity * prediction);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(predTarget, 0.3f);
    }

}
