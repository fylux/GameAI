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
    float rangeDistance;
    float timeStamp;

	public Follow(AgentUnit agent, AgentUnit target, Action<bool> callback, float rangeDistnace) : base(agent,callback) {
        this.target = target;
        this.lastTargetPosition = target.position;
        this.goTo = new GoTo(agent, GetFutureTargetPosition(), (_) => {});
        this.rangeDistance = rangeDistnace;
        inRange = false;
        timeStamp = Time.fixedTime;
    }

    bool ReconsiderPath() {
        if (Util.HorizontalDistance(lastTargetPosition, GetFutureTargetPosition()) > 0.2) {
            goTo.SetNewTarget(GetFutureTargetPosition());
            lastTargetPosition = GetFutureTargetPosition();
            return true;
        }
        return false;
    }

    Vector3 GetFutureTargetPosition() {
        float lookAhead = Util.HorizontalDistance(agent.position, target.position) / 2f ;
        Vector3 futurePosition = target.position + target.velocity * lookAhead;
        //Only predict if it is not too close and the prediction is a walkable place
        if (Util.HorizontalDistance(agent.position, target.position) > 4f && Map.NodeFromPosition(futurePosition).isWalkable())
            return futurePosition;
        else
            return target.position;
    }

    public override Steering Apply() {
        if (IsFinished())
            callback(true);

        float distanceToTarget = Util.HorizontalDistance(agent.position, lastTargetPosition);
        float realDistance = Util.HorizontalDistance(agent.position, target.position);

        // If has reached range or fixed time reconsider path
        if ( (!inRange && distanceToTarget < rangeDistance*0.9) || Time.fixedTime - timeStamp > 2) {
            timeStamp = Time.fixedTime;
            bool changed_path = ReconsiderPath();
            //If the path has not changed and we are on range
            if (!changed_path && distanceToTarget < 1.4f) {
                Debug.Log("Enemy in attack range");
                inRange = true;
                goTo.FinishPath();
                agent.RequestStopMoving();
            }
        }
        //If the enemy it goes out of range
        else if (inRange && realDistance > rangeDistance*1.1) {
            Debug.Log("Enemy goes out of range");
            inRange = false;
            ReconsiderPath();
        }

        if (!inRange)
            return goTo.Apply();
        else
            return new Steering();
    }


    public bool IsInRange() {
        return inRange;
    }

    protected override bool IsFinished() {
        return false;
    }

    override
    public void Terminate() {
        goTo.Terminate();
    }
}
