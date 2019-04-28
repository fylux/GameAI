using System;
using System.Linq;
using UnityEngine;

public class DefendZone : HostileTask {

    Vector3 center;
    float rangeRadius;
    AgentUnit targetEnemy;
    public Attack attack;

    public DefendZone(AgentUnit agent, Vector3 center, float rangeRadius, Action<bool> callback) : base(agent,callback) {
        this.center = center;
        this.rangeRadius = rangeRadius;
        targetEnemy = null;
        attack = null;
    }

    //If you are attacked by a different unit you will start figthing with it unless that you are already figthing or very close to your target
    override
    public void ReceiveAttack(AgentUnit enemy) {
        //It would be uncommon that targetEnemy is null unless that the range of the enemy is greater than the range of defend zone
        bool inRange = Util.HorizontalDistance(targetEnemy.position, center) /* + Attack range*/ > rangeRadius;
        bool targetNear = Util.HorizontalDistance(targetEnemy.position, agent.position) < 3f /*+ AttackRange*/;
        if (targetEnemy == null || (inRange && !targetNear)) {
            AttackEnemy(enemy);
        }
    }

    void AttackEnemy(AgentUnit newEnemy) {
        if (attack != null) {
            attack.Terminate();
            attack = null;
        }

        if (newEnemy != null) {
            Debug.Log("Found enemy " + newEnemy.name);
            targetEnemy = newEnemy;
            attack = new Attack(agent, targetEnemy, (_) => {
                attack.Terminate();
                attack = null;
            });
        }
    }

    public void SetCenter(Vector3 center) {
        this.center = center;
    }

    public override Steering Apply() {
        if (IsFinished())
            callback(true);

        Steering st = new Steering();

        //Comprobar si se ha matado a la unidad
        if (attack == null || Util.HorizontalDistance(targetEnemy.position, center) > rangeRadius + agent.attackRange) {
            AgentUnit closerEnemy = Physics.OverlapSphere(center, rangeRadius + agent.attackRange)
                                            .Select(coll => coll.GetComponent<AgentUnit>())
                                            .Where(unit => unit != null && unit.faction != agent.faction)
                                            .OrderBy(enemy => Util.HorizontalDistance(agent.position, enemy.position))
                                            .FirstOrDefault();

            AttackEnemy(closerEnemy);

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
