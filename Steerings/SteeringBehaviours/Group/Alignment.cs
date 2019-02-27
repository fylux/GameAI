using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alignment : SteeringBehaviour
{

    private GameObject[] targets;

    [SerializeField]
    private float targetRadius;

    [SerializeField]
    private float slowRadius;

    [SerializeField]
    private float timeToTarget = 0.1f;

    [SerializeField]
    private float threshold = 3f;

    private new void Start()
    {
        base.Start();
        targets = GameObject.FindGameObjectsWithTag("NPC");
    }

    public override Steering GetSteering()
    {
        return Alignment.GetSteering(npc, targets, this.gameObject, threshold, targetRadius, slowRadius, timeToTarget);
    }

    public static Steering GetSteering(Body npc, GameObject[] targets, GameObject self, float threshold, float targetRadius, float slowRadius, float timeToTarget)
    {
        int neighbours = 0;
        float targetOrientation = 0;

        Vector3 Heading = Vector3.zero;

        foreach (GameObject boid in targets)
        {
            Body bodi = boid.GetComponent<Body>();
            Vector3 direction = bodi.position - npc.position;
            float distance = direction.magnitude;
            if (boid != self && distance < threshold)
            {
                targetOrientation += bodi.orientation;
                neighbours++;
            }
        }

        if (neighbours > 0)
        {
            targetOrientation /= neighbours;
            return Align.GetSteering(targetOrientation, npc, targetRadius, slowRadius, timeToTarget);
        }

        return new Steering();


    }

}
