using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cohesion : SteeringBehaviour {

    GameObject[] targets;

    [SerializeField]
    float threshold;

    [SerializeField]
    float decayCoefficient;

    public override Steering GetSteering() {
        return Cohesion.GetSteering(npc, targets, this.gameObject, threshold, decayCoefficient, maxAccel);
    }

    public static Steering GetSteering(Agent npc, GameObject[] targets, GameObject self, float threshold, float decayCoefficient, float maxAccel) {
        Steering steering = new Steering();

        int neighbours = 0;
        Vector3 centerOfMass = Vector3.zero;

        foreach (GameObject boid in targets) { //Comprobar con un SphereCast, en vez de Tag quiza usar Layers
            Body bodi = boid.GetComponent<Body>();
            Vector3 direction = bodi.position - npc.position;
            float distance = direction.magnitude;
            

            if (boid != self && distance < threshold) {
                centerOfMass += bodi.position;
                neighbours++;
            }
        }

        if (neighbours > 0) {
            centerOfMass /= neighbours;
            return Seek.GetSteering(centerOfMass, npc, maxAccel,false);
        }

        return steering;
    }

	// Use this for initialization
	public new void Start () {
        base.Start();
        targets = GameObject.FindGameObjectsWithTag("NPC");
	}
}
