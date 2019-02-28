using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : SteeringBehaviourTarget {

  /*  [SerializeField]
    private float targetRadius;

    [SerializeField]
    private float slowRadius;*/

    [SerializeField]
    private float timeToTarget = 0.1f;

    override
    public Steering GetSteering()
    {
        return Face.GetSteering(target.position, npc, npc.interiorAngle, npc.exteriorAngle, timeToTarget);
    }

    public static Steering GetSteering(Vector3 targetPosition, Agent npc, float targetRadius, float slowRadius, float timeToTarget)
    {
        Steering steering = new Steering();

        Vector3 direction = targetPosition - npc.position;

        if (direction.magnitude <= 0.0f)
            return steering;

        float targetOrientation = Mathf.Atan2(direction.x, direction.z);
        targetOrientation *= Mathf.Rad2Deg;

        // virtualTarget.GetComponent<Agent>.orientation = targetOrientation
        // return Align.Steer(...), siendo el target con el que nos vamos a alinear el virtualTarget

        return Align.GetSteering(targetOrientation, npc, targetRadius, slowRadius, timeToTarget);
    }


}
