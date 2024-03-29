﻿using System;
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
            follow.Apply(); //Keep detecting if enemy moves
            //agent.RequestStopMoving();
            st += Face.GetSteering(targetEnemy.position, agent, agent.interiorAngle, agent.exteriorAngle, 0.1f, false);


            if ((Time.fixedTime - timeStamp) * agent.militar.attackSpeed > 1) {
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
        if (follow != null) follow.Terminate();
    }

    override
    public string ToString() {
        if (targetEnemy == null)
            return "Killed enemy";
         else if (follow.goTo.finished) {
            return "Attack->" + targetEnemy.name + " GoTo finished";

        } else {
            return "Attack->" + targetEnemy.name + " inrang " + follow.IsInRange() + " processing "+follow.goTo.processing;

        }

    }

	public AgentUnit GetTargetEnemy(){
		return targetEnemy;
	}
}
