using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : SteeringBehaviour {

    [SerializeField]
    float threshold;

    [SerializeField]
    float decayCoefficient;

    public override Steering GetSteering() {
        return Cohesion.GetSteering(npc, threshold, decayCoefficient, maxAccel);
    }

    public static Steering GetSteering(Agent npc, float threshold, float decayCoefficient, float maxAccel) {
        Steering steering = new Steering();

        int neighbours = 0;
        Vector3 centerOfMass = Vector3.zero;

        int layerMask = 1 << 9;
        Collider[] hits = Physics.OverlapSphere(npc.position, threshold, layerMask);
        foreach (Collider coll in hits)
        { //Comprobar con un SphereCast, en vez de Tag quiza usar Layers
            Agent agent = coll.GetComponent<Agent>();
            Vector3 direction = agent.position - npc.position;
            float distance = direction.magnitude;
            

            if (agent != npc && distance < threshold) {
                centerOfMass += agent.position;
                neighbours++;
            }
        }

        if (neighbours > 0) {
            centerOfMass /= neighbours;
            return Seek.GetSteering(centerOfMass, npc, maxAccel,false);
        }

        return steering;
    }
}
