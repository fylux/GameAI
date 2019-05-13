using System;
using System.Linq;
using UnityEngine;

public class Patrol : HostileTask {

	Vector3 center;
	float rangeRadius;
	AgentUnit targetEnemy;
	FollowPath followPath;
    public Attack attack;
    const float followRangeExtra = 1f;

	public Patrol(AgentUnit agent, Vector3[] path, float rangeRadius, Action<bool> callback) : base(agent,callback) {
		Debug.Assert(followRangeExtra >= 1f);
		this.rangeRadius = rangeRadius;
		targetEnemy = null;
		attack = null;
        followPath = new FollowPath(agent, path, FollowT.LOOP, (_) => {});
	}

	//If you are attacked by a different unit you will start figthing with it unless that you are already figthing or very close to your target
	override
	public void ReceiveAttack(AgentUnit enemy) {
		//It would be uncommon that targetEnemy is null unless that the range of the enemy is greater than the range of defend zone
		bool inRange = Util.HorizontalDist(targetEnemy.position, center) /* + Attack range*/ > rangeRadius;
		bool targetNear = Util.HorizontalDist(targetEnemy.position, agent.position) < 3f /*+ AttackRange*/;
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
			Debug.Log("Found enemy " + newEnemy.name + " distance " + Util.HorizontalDist(newEnemy.position, agent.position) +" by "+agent.name);
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
		if (attack == null || Util.HorizontalDist(targetEnemy.position, center) > rangeRadius + agent.attackRange + followRangeExtra) {
			AgentUnit closerEnemy = Info.GetUnitsFactionArea(center, rangeRadius + agent.attackRange, Util.OppositeFaction(agent.faction))
				.OrderBy(enemy => Util.HorizontalDist(agent.position, enemy.position))
				.FirstOrDefault();

			AttackEnemy(closerEnemy);
		}

		if (attack != null) st = attack.Apply();
		else st = followPath.Apply();

		return st;
	}

	override
	protected bool IsFinished() {
		return false;
	}

	override
	public void Terminate() {
		if (attack != null) attack.Terminate();
        if (followPath != null) followPath.Terminate();
	}

	override
	public String ToString() {
		return "DefendZone -> " + center;
	}
}
