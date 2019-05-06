using System;
using UnityEngine;

public class Attack : Task {

    AgentUnit targetEnemy;
    bool killedEnemy = true;
    Follow follow;

	public Attack(AgentUnit agent, AgentUnit targetEnemy, Action<bool> callback) : base(agent,callback) {
        this.targetEnemy = targetEnemy;
        follow = new Follow(agent, targetEnemy, (_) => { });
    }


    public override Steering Apply() {
        Steering st = new Steering();

        if (IsFinished()) {
            callback(true);
            return st;
        }


        if (follow.IsInRange()) {
            if (Time.fixedTime - timeStamp > 1) {
                agent.militar.Attack(targetEnemy);
                timeStamp = Time.fixedTime;
            }
        }
        else {
            st = follow.Apply();
        }

        return st;
    }


    //Cuando la unidad enemiga muere devolver true
    protected override bool IsFinished() {
        return targetEnemy == null || targetEnemy.militar.IsDead();
    }

    override
    public void Terminate() {
        follow.Terminate();
    }

    override
    public string ToString() {
        return "Attack ->" + targetEnemy.name;
    }

	public AgentUnit GetTargetEnemy(){
		return targetEnemy;
	}
}
