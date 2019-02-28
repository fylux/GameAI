﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Evade : SteeringBehaviourTarget {
    [SerializeField]
    private float maxPrediction;

    override
    public Steering GetSteering() {
        return GetSteering(target, npc, maxAccel, maxPrediction, visibleRays);
    }


    public static Steering GetSteering(Agent target, Agent npc, float maxAccel, float maxPrediction, bool visibleRays = false) {
        return -Pursue.GetSteering(target,npc,maxAccel,maxPrediction,visibleRays);
    }

}