using System;
using System.Collections;
using System.Collections.Generic;
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


    public override Steering Apply() {
        if (IsFinished())
            callback(true);

        Steering st = new Steering();
        if (attack != null) st = attack.Apply();

        Debug.DrawLine(center, center + Vector3.right * rangeRadius, Color.blue);
        Debug.DrawLine(center, center + Vector3.forward * rangeRadius, Color.blue);
        Debug.DrawLine(center, center - Vector3.right * rangeRadius, Color.blue);
        Debug.DrawLine(center, center - Vector3.forward * rangeRadius, Color.blue);


        //Comprobar si se ha matado a la unidad
        if (attack == null || Util.HorizontalDistance(targetEnemy.position, center) > rangeRadius) {
            targetEnemy = null;
            if (attack != null) {
                attack.Terminate();
                attack = null;
            }

            Collider[] hits = Physics.OverlapSphere(center, rangeRadius);
            foreach (Collider coll in hits) {
                AgentUnit unit = coll.GetComponent<AgentUnit>();
     
                if (unit != null && unit.faction != agent.faction) {
                    Debug.Log("Found enemy");
                    targetEnemy = unit;
                    attack = new Attack(agent, targetEnemy, (_) => { });
                    break;
                }
            }
            //It would be nice if it does not find any target that it returns back to the center
        }

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
