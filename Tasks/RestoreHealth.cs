﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Considerar variacion basada en pursue sin pathfinding en vez de GoTo
public class RestoreHealth : Task {

    GoTo goTo;
    DefendZone defendZone;
    Vector3 healingPoint;

	public RestoreHealth(AgentUnit agent, Action<bool> callback) : base(agent,callback) {
        healingPoint = Info.GetClosestHealingPoint(agent.position, 150f).position;

		this.goTo = new GoTo(agent, healingPoint, Mathf.Infinity, 0, true, (bool success) => {
            goTo.Terminate();
            goTo = null;
            defendZone = new DefendZone(agent, agent.position, 2f, (_) => {

            });
        });
    }



    public override Steering Apply() {
        Steering st = new Steering();

        if (IsFinished()) {
            callback(true);
            return st;
        }

        if (goTo != null) {
            st = goTo.Apply();
          //  Debug.Log("Aplicando goto");
        }
        else if (defendZone != null) {
            st = defendZone.Apply();
        }

        
        return st;
    }


    //Should I consider then the unit that we are following died?
    protected override bool IsFinished() {
        return agent.militar.health == agent.militar.maxHealth;
    }

    override
    public void Terminate() {
        if (goTo != null) goTo.Terminate();
        if (defendZone != null) defendZone.Terminate();
    }

    override
    public string ToString() {
        return "RestoreHealth ->" + healingPoint;
    }
}
