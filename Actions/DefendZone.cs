using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefendZone : Task {

    Vector3 center;
    float rangeRadius;
    float timeLastAttack;
    AgentUnit targetEnemy;
    Attack attack;

    public DefendZone(AgentUnit agent, Vector3 center, float rangeRadius, Action<bool> callback) : base(agent,callback) {
        this.center = center;
        this.rangeRadius = rangeRadius;
        timeLastAttack = Time.fixedTime;
        targetEnemy = null;
        attack = null;
    }

    //If you are attacked by a different unit you will start figthing with it unless that you are already figthing or very close to your target
    public void ReceiveAttack(AgentUnit enemy) {
        //It would be uncommon that targetEnemy is null unless that the range of the enemy is greater than the range of defend zone
        bool inRange = Util.HorizontalDistance(targetEnemy.position, center) /* + Attack range*/ > rangeRadius;
        bool targetNear = Util.HorizontalDistance(targetEnemy.position, agent.position) < 3f /*+ AttackRange*/;
        if (targetEnemy == null || (inRange && !targetNear)) {
            targetEnemy = enemy;
            if (attack != null) {
                attack.Terminate();
                attack = null;
            }
            attack = new Attack(agent, targetEnemy, (_) => { });
        }
    }

    public override Steering Apply() {
        if (IsFinished())
            callback(true);

        Steering st = new Steering();

        //Comprobar si se ha matado a la unidad
        if (attack == null || Util.HorizontalDistance(targetEnemy.position, center) /* + Attack range*/ > rangeRadius) {
            targetEnemy = null;
            if (attack != null) {
                attack.Terminate();
                attack = null;
            }

            AgentUnit closerEnemy = Physics.OverlapSphere(center, rangeRadius /*+AttackRange*/)
                                            .Select(coll => coll.GetComponent<AgentUnit>())
                                            .Where(unit => unit != null && unit.faction != agent.faction)
                                            .OrderBy(enemy => Util.HorizontalDistance(agent.position, enemy.position))
                                            .FirstOrDefault();
            if (closerEnemy != null) {
                Debug.Log("Found enemy "+closerEnemy.name);
                targetEnemy = closerEnemy;
                attack = new Attack(agent, targetEnemy, (_) => { });
            }

            //It would be nice if it does not find any target that it returns back to the center
        }

        if (attack != null) st = attack.Apply();

        return st;
    }

    override
    protected bool IsFinished() {
        return false;
    }

    override
    public void Terminate() {
        if (attack != null) attack.Terminate();
    }
}
