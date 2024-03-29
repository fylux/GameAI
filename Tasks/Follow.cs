﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Considerar variacion basada en pursue sin pathfinding en vez de GoTo
public class Follow : Task {

    AgentUnit target;
    //Vector3 lastTargetPosition;
    public GoTo goTo;
    bool inRange;

	public Follow(AgentUnit agent, AgentUnit target, Action<bool> callback) : base(agent,callback) {
        this.target = target;
        //this.lastTargetPosition = target.position;
        Debug.Assert(Map.NodeFromPosition(target.position).isWalkable());
        this.goTo = new GoTo(agent, target.position/*GetFutureTargetPosition()*/, (_) => {
            if (IsNearEnough()) SetInRange();
            else ReconsiderPath();
        });
        inRange = IsNearEnough();
    }

    void ReconsiderPath() {
        goTo.SetNewTarget(target.position);
       // lastTargetPosition = target.position;
    }

    /*Vector3 GetFutureTargetPosition() {
        float lookAhead = Mathf.Clamp(Util.HorizontalDist(agent.position, target.position) / 2f, 0, 3);
        Vector3 futurePosition = Map.Clamp(target.position + target.velocity * lookAhead);
        //Only predict if it is not too close and the prediction is a walkable place
        if (Util.HorizontalDist(agent.position, target.position) > 4f && Map.NodeFromPosition(futurePosition).isWalkable())
            return futurePosition;
        else
            return target.position;
    }*/

    public override Steering Apply() {
        Steering st = new Steering();
        if (IsFinished()) {
            callback(true);
            return st;
        }

        // If has reached range or fixed time reconsider path
        if (!inRange) {
            if (IsNearEnough()) {
               SetInRange();
            }
            else if (Time.fixedTime - timeStamp > 2) {
                timeStamp = Time.fixedTime;
                ReconsiderPath();
            }
        }
        //If the enemy it goes out of range
        else if (inRange && IsFarEnough()) {
            inRange = false;
            ReconsiderPath();
        }



        if (!inRange) {
            st = goTo.Apply();
        }
        return st;
    }


    public bool IsInRange() {
        return inRange;
    }

    void SetInRange() {
        inRange = true;
        goTo.FinishPath();
        agent.RequestStopMoving();
    }

    private bool IsNearEnough() {
        float distanceToTarget = Util.HorizontalDist(agent.position, target.position);
        return distanceToTarget < agent.militar.attackRange * 0.9;
    }

    private bool IsFarEnough() {
        float realDistance = Util.HorizontalDist(agent.position, target.position);
        return realDistance > agent.militar.attackRange * 1.1;
    }

   /* private bool IsNearEnoughLastPosition() {
        float distanceToTarget = Util.HorizontalDist(agent.position, lastTargetPosition);
        return distanceToTarget < agent.militar.attackRange * 0.9;
    }*/

    //Should I consider then the unit that we are following died?
    protected override bool IsFinished() {
        return target == null;
    }

    override
    public void Terminate() {
        if (goTo != null) goTo.Terminate();
    }
}
