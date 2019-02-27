using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Align : SteeringBehaviour
{

    [SerializeField]
    private GameObject target;

    [SerializeField]
    private float targetRadius;

    [SerializeField]
    private float slowRadius;

    [SerializeField]
    private float timeToTarget = 0.1f;

    override
	public Steering GetSteering()
    {
        Steering steering = new Steering();

        return Align.GetSteering(target.GetComponent<Body>().orientation, npc, targetRadius, slowRadius, timeToTarget);

       /* float rotacion = orienTarget - body.orientation;

        rotacion = MapToRange(rotacion);
        float rotationSize = Mathf.Abs(rotacion);

        if (rotationSize < targetRadius)
            return steering;

        float targetRotation;
        if (rotationSize > slowRadius)
            targetRotation = body.MaxRotation;
        else
            targetRotation = body.MaxRotation * rotationSize / slowRadius;

        targetRotation *= rotacion / rotationSize;

        steering.angular = targetRotation - body.rotation;
        steering.angular /= timeToTarget;

        float angularAccel = Mathf.Abs(steering.angular);

        if (angularAccel > body.MaxAngular)
        {
            steering.angular /= angularAccel;
            steering.angular *= body.MaxAngular;
        }

        return steering;*/

    }

    public static Steering GetSteering(float orienTarget, Body body, float targetRadius, float slowRadius, float timeToTarget)
    {
        Steering steering = new Steering();

       // float orienTarget = target.orientation;

        float rotacion = orienTarget - body.orientation;

        rotacion = MapToRange(rotacion);
        float rotationSize = Mathf.Abs(rotacion);

        if (rotationSize < targetRadius)
            return steering;

        float targetRotation;
        if (rotationSize > slowRadius)
            targetRotation = body.MaxRotation;
        else
            targetRotation = body.MaxRotation * rotationSize / slowRadius;

        targetRotation *= rotacion / rotationSize;

        steering.angular = targetRotation - body.rotation;
        steering.angular /= timeToTarget;

        float angularAccel = Mathf.Abs(steering.angular);

        if (angularAccel > body.MaxAngular)
        {
            steering.angular /= angularAccel;
            steering.angular *= body.MaxAngular;
        }

        return steering;
    }

    private static float MapToRange (float rotation)
    {
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
