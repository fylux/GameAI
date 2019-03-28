using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendZone : BaseTask {

    Vector3 target;
    float rangeRadius;
    float timeLastAttack;

    public DefendZone(AgentUnit agent, Vector3 target, float rangeRadius, Action<bool> callback) : base(agent,callback) {
        this.target = target;
        this.rangeRadius = rangeRadius;
        timeLastAttack = Time.fixedTime;
    }


    public override Steering Apply() {
        if (IsFinished())
            callback(true);


        //Problems
        /*If you select one enemy in the next iteration you may change. If you take an
         enemy you should keep fighting him till he goes out of range*/
        /*It should pursue the enemy as long as is in the zone*/

        List<AgentUnit> unitList = new List<AgentUnit>();
        Collider[] hits = Physics.OverlapSphere(agent.position, rangeRadius);
        foreach (Collider coll in hits) {
            AgentUnit unit = coll.GetComponent<AgentUnit>();
            //Get any enemy in the range
            if (unit != null && unit.faction != agent.faction) {
                //unitList.Add(unit);
                if (Time.fixedTime - timeLastAttack > 1) {
                    agent.Attack(unit);
                    timeLastAttack = Time.fixedTime;
                }
                break;
            }
        }

        return new Steering();
    }


    protected override bool IsFinished() {
        return false;
    }
}
