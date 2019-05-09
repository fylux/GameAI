using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehaviourTarget {

    /* [SerializeField]
       private float targetRadius;

       [SerializeField]
       private float slowRadius;*/

    [SerializeField]
    float timeToTarget = 0.1f;

    override
	public Steering GetSteering() {
        return Align.GetSteering(target.orientation, npc, npc.interiorAngle, npc.exteriorAngle, timeToTarget, visibleRays);
    }

    public static Steering GetSteering(float orienTarget, Agent npc, float targetRadius, float slowRadius, float timeToTarget, bool visibleRays) {
        Steering steering = new Steering();

        float rotacion = orienTarget - npc.orientation;

        rotacion = MapToRange(rotacion);
        float rotationSize = Mathf.Abs(rotacion);

        if (rotationSize < targetRadius)
            return steering;

        float targetRotation;
        if (rotationSize > slowRadius)
            targetRotation = npc.MaxRotation;
        else
            targetRotation = npc.MaxRotation * rotationSize / slowRadius;

        targetRotation *= rotacion / rotationSize;

        steering.angular = targetRotation - npc.rotation;
        steering.angular /= timeToTarget;

        float angularAccel = Mathf.Abs(steering.angular);

        if (angularAccel > npc.MaxAngular) {
            steering.angular /= angularAccel;
            steering.angular *= npc.MaxAngular;
        }

        if (visibleRays)
             drawRays(npc.position, Util.OrientationToVector(orienTarget), Color.magenta);

        return steering;
    }

    static float MapToRange (float rotation) {
        rotation %= 360.0f;

        if (Mathf.Abs(rotation) > 180.0f)
        {
            if (rotation < 0.0f)
                rotation += 360.0f;
            else
                rotation -= 360.0f;
        }
        return rotation;
    }
}
