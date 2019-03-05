using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : SteeringBehaviour
{
    [SerializeField]
    float separationPriority;

    [SerializeField]
    float cohesionPriority;

    [SerializeField]
    float alignmentPriority;

    [SerializeField]
    float wanderPriority;

    [SerializeField]
    float groupArea;

    [SerializeField]
    float decayCoefficient;

    [SerializeField]
    float timeToTarget;

    [SerializeField]
    float wanderCooldown;

    [SerializeField]
    float wanderRate;

    [SerializeField]
    float wanderCircleOffset;

    [SerializeField]
    float wanderCircleRadius;

    float wanderOrientation = 0.0f;
    Steering wanderForce = new Steering();

    override
    public Steering GetSteering() {
        Steering steering = new Steering();
        steering.linear += Separation.GetSteering(npc, groupArea, decayCoefficient, maxAccel, visibleRays).linear * separationPriority;
        steering.linear += Cohesion.GetSteering(npc, groupArea, decayCoefficient, maxAccel, visibleRays).linear * cohesionPriority;
        steering.angular = Alignment.GetSteering(npc, groupArea, npc.interiorAngle, npc.exteriorAngle, timeToTarget, false).angular * alignmentPriority;
        wanderOrientation += Random.Range(-1.0f, 1.0f) * wanderRate;
        wanderForce = Wander.GetSteering(npc, wanderForce, wanderCooldown, wanderRate, wanderOrientation, wanderCircleOffset, wanderCircleRadius, maxAccel, timeToTarget, visibleRays);
        steering += wanderForce;

        return steering;
    }


}
