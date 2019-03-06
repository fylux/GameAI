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
        wanderOrientation += Random.Range(-1.0f, 1.0f) * wanderRate;
        wanderForce = Wander.GetSteering(npc, wanderForce, wanderCooldown, wanderRate, wanderOrientation, wanderCircleOffset, wanderCircleRadius, maxAccel, timeToTarget, visibleRays);

        return GetSteering(npc, groupArea, decayCoefficient, maxAccel, visibleRays, timeToTarget, wanderCooldown, wanderRate, wanderOrientation, wanderCircleOffset, wanderCircleRadius, wanderForce, cohesionPriority, separationPriority, alignmentPriority, wanderPriority);
    }


    //OJO! En el steering que sea que vaya a llamar de forma estática a esta función, se deberán ejecutar las dos siguientes lineas
    //   wanderOrientation += Flocking.UpdateWanderOrientation(wanderOrientation, wanderRate)
    //   wanderForce = Wander.GetSteering(npc, wanderForce, wanderCooldown, wanderRate, wanderOrientation, wanderCircleOffset, wanderCircleRadius, maxAccel, timeToTarget, visibleRays)
    //  Es por eso que trabajar con flocking de manera estática está desaconsejado
    public static Steering GetSteering(Agent npc, float groupArea, float decayCoefficient, float maxAccel, bool visibleRays, float timeToTarget, float wanderCooldown, float wanderRate, float wanderOrientation, float wanderCircleOffset, float wanderCircleRadius, Steering wanderForce, float cohesionPriority, float separationPriority, float alignmentPriority, float wanderPriority)
    {
        Steering steering = new Steering();
        steering.linear += Separation.GetSteering(npc, groupArea, decayCoefficient, maxAccel, visibleRays).linear * separationPriority;
        steering.linear += Cohesion.GetSteering(npc, groupArea, decayCoefficient, maxAccel, visibleRays).linear * cohesionPriority;
        steering.angular = Alignment.GetSteering(npc, groupArea, npc.interiorAngle, npc.exteriorAngle, timeToTarget, false).angular * alignmentPriority;
        steering += Steering.ApplyPriority(wanderForce, wanderPriority);

        return steering;
    }

    public static float UpdateWanderOrientation(float wanderOrientation, float wanderRate)
    {
        return wanderOrientation + Random.Range(-1.0f, 1.0f) * wanderRate;
    }

    public static Steering UpdateWanderForce(Agent npc, Steering wanderForce, float wanderCooldown, float wanderRate, float wanderOrientation, float wanderCircleOffset, float wanderCircleRadius, float maxAccel, float timeToTarget, bool visibleRays)
    {
        return Wander.GetSteering(npc, wanderForce, wanderCooldown, wanderRate, wanderOrientation, wanderCircleOffset, wanderCircleRadius, maxAccel, timeToTarget, visibleRays);
    }


}
