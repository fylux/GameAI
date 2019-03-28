using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO Llamar a este metodo Pursue y hacer una función que devuelva si esta en rango para hace ataque, defensa, etc.

public class Attack : BaseTask {

    AgentUnit target;

    bool killedEnemy = true;
    Pursuo pursuo;
    float timeLastAttack;

	public Attack(AgentUnit agent, AgentUnit target, Action<bool> callback) : base(agent,callback) {
        this.target = target;
        pursuo = new Pursuo(agent, target, (_) => { });
        timeLastAttack = Time.fixedTime;
    }


    public override Steering Apply() {
        if (IsFinished())
            callback(true);

        Steering st = pursuo.Apply();

        if (pursuo.IsInRange()) {
            if (Time.fixedTime - timeLastAttack > 1) {
                agent.Attack(target);
                timeLastAttack = Time.fixedTime;
            }
        }

        return st;
    }


    protected override bool IsFinished() {
        return false;
    }
}
