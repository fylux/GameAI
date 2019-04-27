using System;
using UnityEngine;

public class Attack : Task {

    AgentUnit targetEnemy;
    bool killedEnemy = true;
    Follow follow;
    float timeLastAttack;

	public Attack(AgentUnit agent, AgentUnit targetEnemy, Action<bool> callback) : base(agent,callback) {
        this.targetEnemy = targetEnemy;
        follow = new Follow(agent, targetEnemy, (_) => { }, 2f /*AttackRange*/);
        timeLastAttack = Time.fixedTime;
    }


    public override Steering Apply() {
        if (IsFinished())
            callback(true);

        Steering st = follow.Apply();

        if (follow.IsInRange() && (Time.fixedTime - timeLastAttack > 1)) {
            agent.militar.Attack(targetEnemy);
            timeLastAttack = Time.fixedTime;
        }

        return st;
    }


    //Cuando la unidad enemiga muere devolver true
    protected override bool IsFinished() {
        return false;
    }

    override
    public void Terminate() {
        follow.Terminate();
    }
}
