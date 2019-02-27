using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Separation : SteeringBehaviour {

    GameObject[] targets;

    [SerializeField]
    float threshold;

    [SerializeField]
    float decayCoefficient;

    public override Steering GetSteering()
    {
        return Separation.GetSteering(npc, targets, this.gameObject, threshold, decayCoefficient, maxAccel);
    }

    public static Steering GetSteering(Agent npc, GameObject[] targets, GameObject self, float threshold, float decayCoefficient, float maxAccel)
    {
        Steering steering = new Steering();

        foreach (GameObject boid in targets) //Comprobar con un SphereCast, en vez de Tag quiza usar Layers
        {
            Body bodi = boid.GetComponent<Body>();
            Vector3 direction = bodi.position - npc.position;
            float distance = direction.magnitude;
            if (boid != self && distance < threshold)
            {
                // Debug.Log("La distancia es de " + distance + ", el threshold es de " + threshold + ", por lo tanto MOVEMOS");
                float strength = Mathf.Min(decayCoefficient / (distance * distance), maxAccel);

                direction = direction.normalized;
                steering.linear += strength * direction;
            }
        }

        steering.linear = (-1) * steering.linear;

        return steering;
    }

	// Use this for initialization
	public new void Start () {
        base.Start();
        targets = GameObject.FindGameObjectsWithTag("NPC");
	}

    /*
     * 
     * foreach (GameObject boid in targets) //Comprobar con un SphereCast, en vez de Tag quiza usar Layers
        {
            Body bodi = boid.GetComponent<Body>();
            if (boid != this && (bodi.position - npc.position).magnitude <= threshold)
            {
                steering.linear.x += bodi.position.x - npc.position.x;
                steering.linear.z += bodi.position.z - npc.position.z;
                numVecinos++;
            }

        }
        if (numVecinos != 0)
        {
            steering.linear.x /= numVecinos;
            steering.linear.y /= numVecinos;

            steering.linear = steering.linear * (-1);
        }

        steering.linear = steering.linear.normalized;
        steering.linear = steering.linear * maxSeparation;

        return steering;
     * */
}
