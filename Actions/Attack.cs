using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BaseTask {

    AgentUnit target;
    Vector3 lastTargetPosition;

    bool killedEnemy = true;
    GoTo goTo;
    bool inRange;

    float timeLastAttack;

	public Attack(AgentUnit agent, AgentUnit target, Action<bool> callback) : base(agent,callback) {
        this.target = target;
        this.lastTargetPosition = target.position;
        timeLastAttack = Time.fixedTime;

        this.goTo = new GoTo(agent, target.position, (_) => {});
        inRange = false;
    }

    bool ReconsiderPath() {
        if (Util.HorizontalDistance(lastTargetPosition, target.position) > 0.2) {
            goTo.SetNewTarget(target.position);
            lastTargetPosition = target.position;
            return true;
        }
        return false;
    }

    public override Steering Apply() {
        if (IsFinished())
            callback(true);

        float distanceToTarget = Util.HorizontalDistance(agent.position, lastTargetPosition);
        float realDistance = Util.HorizontalDistance(agent.position, target.position);

        // If has reached range or fixed time reconsider path
        if ( (!inRange && distanceToTarget < 1.4f) || Time.fixedTime % 20 == 0) {
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
        else if (inRange && realDistance > 1.6f) {
            Debug.Log("Enemy goes out of range");
            inRange = false;
            ReconsiderPath();
        }


        if (!inRange) return goTo.Apply();
        else {
            if (Time.fixedTime - timeLastAttack > 1) {
                agent.Attack(target);
                timeLastAttack = Time.fixedTime;
            }
        }

        return new Steering();
    }


    protected override bool IsFinished() {
        return false;
    }
}
