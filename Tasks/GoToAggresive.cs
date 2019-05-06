using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoToAggresive : HostileTask {

    Vector3 target;
    DefendZone defendZone;
    GoTo goTo;
    bool fighting;

    public GoToAggresive(AgentUnit agent, Vector3 target, float rangeRadius, Action<bool> callback) : base(agent,callback) {
        this.target = target;
        this.defendZone = new DefendZone(agent, agent.position, rangeRadius, (_) => { });
        this.fighting = false;
        this.goTo = new GoTo(agent, target, (bool result) => {
            goTo.Terminate();
            goTo = null;
        });
    }

    //If you are attacked by a different unit you will start figthing with it unless that you are already figthing or very close to your target
    override
    public void ReceiveAttack(AgentUnit enemy) {
        defendZone.ReceiveAttack(enemy);
    }

    public override Steering Apply() {
        Steering st = new Steering();

        if (IsFinished()) {
            callback(true);
            return st;
        }
            

        if (defendZone.attack != null) {
            st = defendZone.Apply();
            fighting = true;
        }
        else {
            defendZone.Apply();
            if (fighting) {
                goTo.SetNewTarget(target);
            }
            st = goTo.Apply();
            fighting = false;
        }

        //Attack any units that it sees in range
        defendZone.SetCenter(agent.position);

        return st;
    }

    override
    protected bool IsFinished() {
        return goTo == null;
    }

    override
    public void Terminate() {
        if (defendZone != null) defendZone.Terminate();
        if (goTo != null) goTo.Terminate();
    }
}
