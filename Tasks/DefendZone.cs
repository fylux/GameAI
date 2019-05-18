using System;
using System.Linq;
using UnityEngine;

public class DefendZone : HostileTask {

    Vector3 center;
    float rangeRadius;
    AgentUnit targetEnemy;
    public Attack attack;
    GoTo goTo;
    const float followRangeExtra = 1f;
    bool returning;

    public DefendZone(AgentUnit agent, Vector3 center, float rangeRadius, Action<bool> callback) : base(agent,callback) {
        Debug.Assert(followRangeExtra >= 1f);
        this.center = center;
        this.rangeRadius = rangeRadius;
        targetEnemy = null;
        attack = null;
        goTo = new GoTo(agent, center, Mathf.Infinity, rangeRadius / 3, false, (_) => {
            returning = false;
            goTo.SetVisiblePath(false);
            agent.RequestStopMoving();
        }); //The offset should be smaller than the distance when it is consider far
        goTo.SetVisiblePath(false);
        returning = false;
    }

    //If you are attacked by a different unit you will start figthing with it unless that you are already figthing or very close to your target
    override
    public void ReceiveAttack(AgentUnit enemy) {
        if (targetEnemy == null) {
            AttackEnemy(enemy);
            return;
        }
        //It would be uncommon that targetEnemy is null unless that the range of the enemy is greater than the range of defend zone
        bool inRange = Util.HorizontalDist(targetEnemy.position, center) /* + Attack range*/ > rangeRadius;
        bool targetNear = Util.HorizontalDist(targetEnemy.position, agent.position) < 3f /*+ AttackRange*/;
        if (inRange && !targetNear) {
            AttackEnemy(enemy);
        }
    }

    void AttackEnemy(AgentUnit newEnemy) {
        if (attack != null) {
            attack.Terminate();
            attack = null;
        }

        if (newEnemy != null) {
            Debug.Log("Found enemy " + newEnemy.name + " distance " + Util.HorizontalDist(newEnemy.position, agent.position) +" by "+agent.name);
            targetEnemy = newEnemy;
            returning = false;
            goTo.SetVisiblePath(false);
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
        if (attack == null || Util.HorizontalDist(targetEnemy.position, center) > rangeRadius + agent.militar.attackRange + followRangeExtra) {
            AgentUnit closerEnemy = Info.GetUnitsFactionArea(center, rangeRadius + agent.militar.attackRange, Util.OppositeFaction(agent.faction))
                                            .OrderBy(enemy => Util.HorizontalDist(agent.position, enemy.position))
                                            .FirstOrDefault();


            AttackEnemy(closerEnemy);

            //if it does not find any target that it returns back to the center

            if (closerEnemy == null && !returning && Util.HorizontalDist(agent.position, center) > rangeRadius / 2 ) { 
                goTo.SetNewTarget(center);
                goTo.SetVisiblePath(true);
                returning = true;
            }

        }

        if (attack != null) st = attack.Apply();
        else if (returning) st = goTo.Apply();

        return st;
    }

    override
    protected bool IsFinished() {
        return false;
    }

    override
    public void Terminate() {
        if (attack != null) attack.Terminate();
        if (goTo != null) goTo.Terminate();
    }

    override
    public string ToString() {
        return "DefendZone -> " + center +" "+attack == null ? ",attacking" : "";
    }
}
