using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : SteeringBehaviour {

   /* [SerializeField]
    private float targetRadius;

    [SerializeField]
    private float slowRadius;*/

    [SerializeField]
    private float timeToTarget = 0.1f;

    [SerializeField]
    private float threshold = 3f;

    public override Steering GetSteering() {
        return Alignment.GetSteering(npc, threshold, npc.interiorAngle, npc.exteriorAngle, timeToTarget);
    }

    public static Steering GetSteering(Agent npc, float threshold, float targetRadius, float slowRadius, float timeToTarget)  {
        int neighbours = 0;
        float targetOrientation = 0;

        Vector3 Heading = Vector3.zero;

        int layerMask = 1 << 9;
        Collider[] hits = Physics.OverlapSphere(npc.position, threshold, layerMask);
        foreach (Collider coll in hits)
        {
            Agent agent = coll.GetComponent<Agent>();
            Vector3 direction = agent.position - npc.position;
            float distance = direction.magnitude;
            if (agent != npc && distance < threshold) {
                targetOrientation += agent.orientation;
                neighbours++;
            }
        }

        if (neighbours > 0) {
            targetOrientation /= neighbours;
            return Align.GetSteering(targetOrientation, npc, targetRadius, slowRadius, timeToTarget);
        }

        return new Steering();


    }

}
