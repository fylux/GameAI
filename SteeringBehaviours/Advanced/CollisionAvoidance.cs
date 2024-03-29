﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CollisionAvoidance : SteeringBehaviour {

    [SerializeField]
    float collisionRadius = 0.6f;

    GameObject[] targets;

    new
    protected void Start() {
        base.Start();
        targets = GameObject.FindGameObjectsWithTag("NPC");
    }

    override
    public Steering GetSteering() {
        Steering steering = new Steering();

        float shortestTime = Mathf.Infinity;
        Agent firstTarget = null;
        float firstMinSeparation = 0.0f;
        float firstDistance = 0.0f;
        Vector3 firstRelativePos = Vector3.zero;
        Vector3 firstRelativeVel = Vector3.zero;

        foreach (GameObject t in targets) {
            Agent target = t.GetComponent<Agent>();
            if (Vector3.Distance(target.position, npc.position) > 3f) //Addition
                continue;
            Vector3 relativePos = target.position - npc.position;
            Vector3 relativeVel = target.velocity - npc.velocity;
            float relativeSpeed = relativeVel.magnitude;

            float timeToCollision = -(Vector3.Dot(relativePos, relativeVel))
                                    / (relativeSpeed * relativeSpeed);
            float distance = relativePos.magnitude;
            float minSeparation = distance - (relativeSpeed * timeToCollision);
            if (minSeparation > 2 * collisionRadius)
                continue;

            if (timeToCollision > 0.0f && timeToCollision < shortestTime) {
                shortestTime = timeToCollision;
                firstTarget = target;
                firstMinSeparation = minSeparation;
                firstDistance = distance;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }
        }
        npc.GetComponent<Renderer>().material.color = Color.red;
        if (firstTarget == null)
            return steering;
        if (firstMinSeparation <= 0.0f || firstDistance < 2 * collisionRadius) {
            firstRelativePos = firstTarget.position - npc.position; //This should be already assigned, so it is useless
            npc.GetComponent<Renderer>().material.color = Color.blue;
        } else {
            firstRelativePos += firstRelativeVel * shortestTime;
            npc.GetComponent<Renderer>().material.color = Color.green;
        }

        firstRelativePos.Normalize();
        steering.linear = -firstRelativePos * npc.MaxAccel;
        if (visibleRays)
            drawRays(npc.position,steering.linear,Color.red);
        return steering;
    }

}
