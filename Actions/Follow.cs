using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Considerar variacion basada en pursue sin pathfinding en vez de GoTo
public class Follow : Task {

    AgentUnit target;
    Vector3 lastTargetPosition;
    GoTo goTo;
    bool inRange;
    float timeStamp;

	public Follow(AgentUnit agent, AgentUnit target, Action<bool> callback) : base(agent,callback) {
        this.target = target;
        this.lastTargetPosition = target.position;
        this.goTo = new GoTo(agent, GetFutureTargetPosition(), (_) => {});
        inRange = IsNearEnough();
        timeStamp = Time.fixedTime;
    }

    bool ReconsiderPath() {
        if (Util.HorizontalDist(lastTargetPosition, GetFutureTargetPosition()) > 0.2) {
            goTo.SetNewTarget(GetFutureTargetPosition());
            lastTargetPosition = GetFutureTargetPosition();
            return true;
        }
        return false;
    }

    Vector3 GetFutureTargetPosition() {
        float lookAhead = Util.HorizontalDist(agent.position, target.position) / 2f ;
        Vector3 futurePosition = target.position + target.velocity * lookAhead;
        //Only predict if it is not too close and the prediction is a walkable place
        if (Util.HorizontalDist(agent.position, target.position) > 4f && Map.NodeFromPosition(futurePosition).isWalkable())
            return futurePosition;
        else
            return target.position;
    }

    public override Steering Apply() {
        Steering st = new Steering();
        if (IsFinished()) {
            callback(true);
            return st;
        }

        float realDistance = Util.HorizontalDist(agent.position, target.position);

        // If has reached range or fixed time reconsider path
        if ( (!inRange && IsNearEnough()) || Time.fixedTime - timeStamp > 2) {
            timeStamp = Time.fixedTime;
            bool changed_path = ReconsiderPath();
            //If the path has not changed and we are on range
            if (!changed_path && IsNearEnough()) {
                //Debug.Log("Enemy in attack range");
                inRange = true;
                goTo.FinishPath();
                agent.RequestStopMoving();
            }
        }
        //If the enemy it goes out of range
        else if (inRange && realDistance > agent.attackRange * 1.1) {
            //Debug.Log("Enemy goes out of range");
            inRange = false;
            ReconsiderPath();
        }

        if (!inRange)
            st = goTo.Apply();
        return st;
    }


    public bool IsInRange() {
        return inRange;
    }

    private bool IsNearEnough() {
        float distanceToTarget = Util.HorizontalDist(agent.position, lastTargetPosition);
        return distanceToTarget < agent.attackRange * 0.9;
    }

    //Should I consider then the unit that we are following died?
    protected override bool IsFinished() {
        return target == null;
    }

    override
    public void Terminate() {
        goTo.Terminate();
    }
}
