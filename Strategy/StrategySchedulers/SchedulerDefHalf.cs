using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SchedulerDefHalf : SchedulerStrategy {

    const int minimunHealth = 5; //If it is lower the unit will try to go to a healing point

    public float GetMilitaryBalanceCluster(HashSet<AgentUnit> cluster) {
        return Info.MilitaryAdvantageArea(Info.GetClusterCenter(cluster), 10f, allyFaction);
    }

    public Dictionary<HashSet<AgentUnit>, int> DistributeClusters(Dictionary<HashSet<AgentUnit>, float> clusters, int nUnits) {
        //Normalize
        float sum = clusters.Sum(c => c.Value);
        foreach (var cluster in clusters.Keys.ToList()) {
            clusters[cluster] /= sum;

        }

        //Distribute
        Dictionary<HashSet<AgentUnit>, int> nUnitsToCluster = clusters.ToDictionary(c => c.Key, c => Mathf.FloorToInt(c.Value * nUnits));

        //Assign based on rounding error
        int nRemainingUnits = nUnits - nUnitsToCluster.Sum(c => c.Value);
        var clusteryAllocResidual = clusters.OrderByDescending(c => (c.Value * nRemainingUnits) - Mathf.FloorToInt(c.Value * nRemainingUnits))
                                        .Select(c => c.Key)
                                        .Take(nRemainingUnits);
        foreach (var cluster in clusteryAllocResidual) {
            nUnitsToCluster[cluster]++;
        }

        return nUnitsToCluster;
    }

    override
    public void ApplyStrategy()
    {
        //Units with low l1evel of health should try to go to a healing point
        var damagedAllies = usableUnits.Where(unit => unit.militar.health < minimunHealth && !unit.HasTask<RestoreHealth>());
        foreach (var ally in damagedAllies) {
			bool winning = false;

			if (ally.HasTask<Attack>()) {
				Attack task = (Attack)ally.GetTask ();
				AgentUnit targetEnemy = task.GetTargetEnemy ();
				if (targetEnemy.militar.health <= ally.militar.health)
					winning = true;
			}
			if (winning == false)
				ally.SetTask(new RestoreHealth(ally, (_) => { }));
        }

        var remainingUnits = new HashSet<AgentUnit>(usableUnits.Where(unit => !unit.HasTask<RestoreHealth>()));

        var clusters = Info.GetClusters(enemyFaction, allyFaction);
        var clustersByAdvantage = clusters.ToDictionary(c => c, c => GetMilitaryBalanceCluster(c)).OrderByDescending(c => c.Value);
        var unitsAssignedToClusters = new Dictionary<HashSet<AgentUnit>, HashSet<AgentUnit>>();

        //Foreach cluster sorted by the advantage that we have
        foreach (HashSet<AgentUnit> cluster in clustersByAdvantage.Select(c => c.Key)) {

            var unitsCluster = new HashSet<AgentUnit>(cluster);
            var center = Info.GetClusterCenter(cluster);
            var closestAllies = remainingUnits.OrderBy(unit => Util.HorizontalDist(center, unit.position));

            //Assign the closest allies to that cluster
            foreach (AgentUnit closestAlly in closestAllies) {
                unitsCluster.Add(closestAlly);
                //Till the balance between the cluster and the assign allies is positive
				if (Info.MilitaryAdvantage(unitsCluster, allyFaction) > 1) {
					Debug.Log ("Enviamos a la unidad " + closestAlly.name + " a atacar al cluster que tiene " + unitsCluster.Count + ", con una ventaja aliada de " + Info.MilitaryAdvantage(unitsCluster, allyFaction));
                    var alliesToCluster = new HashSet<AgentUnit>(unitsCluster.Where(unit => unit.faction == allyFaction));
                    unitsAssignedToClusters.Add(cluster, alliesToCluster);
                    remainingUnits.ExceptWith(alliesToCluster);
                    break;
                }
            }
        }

        //Take the units that haven't been assigned and distribute them between clusters
        var priorityClusters = unitsAssignedToClusters.ToDictionary(c => c.Key, c => Info.MilitaryAdvantage(c.Key, enemyFaction));
        var remainingUnitsToCluster = DistributeClusters(priorityClusters, remainingUnits.Count);

        //Foreach cluster sorted by the advantage that we have
        foreach (HashSet<AgentUnit> cluster in remainingUnitsToCluster.Select(c => c.Key)) {
            var center = Info.GetClusterCenter(cluster);
            int nAssignedUnits = remainingUnitsToCluster[cluster];
            var closestAllies = remainingUnits.OrderBy(unit => Util.HorizontalDist(center, unit.position)).Take(nAssignedUnits);

            //Assign the corresponding number of closest allies to that cluster
            unitsAssignedToClusters[cluster].UnionWith(closestAllies);
            remainingUnits.ExceptWith(unitsAssignedToClusters[cluster]);
        }


        //Asign tasks to units in the selected clusters
        foreach (var cluster in unitsAssignedToClusters) {
            foreach (AgentUnit ally in cluster.Value) {
                if (ally.HasTask<Attack>()) continue;

                var enemiesByDistance = cluster.Key.OrderBy(unit => Util.HorizontalDist(ally.position, unit.position));
                var closestEnemy = enemiesByDistance.First();
                var distanceToEnemy = Util.HorizontalDist(ally.position, closestEnemy.position);

                foreach (var unitType in ally.GetPreferredEnemies()) {
                    var closestEnemyOfType = enemiesByDistance.Where(u => u.GetUnitType() == unitType).FirstOrDefault();
				//	Debug.Log ("El enemigo más cercano del tipo " + unitType + " es " + closestEnemyOfType);

					if (closestEnemyOfType != null && Util.HorizontalDist (ally.position, closestEnemyOfType.position) < distanceToEnemy + 4) {
						ally.SetTask (new Attack (ally, closestEnemyOfType, (_) => {
							//If you kill an enemy reconsider assignations
							ApplyStrategy ();
						}));
						break;
					} //else
					//	Debug.Log ("Pero estaba muy lejos del enemigo más cercano :(");
                }


            }
        }


        //Remaining units go to defend the bridge
		var alliesToDefendBridge = remainingUnits.Where(unit => !unit.HasTask<GoTo>() && !unit.HasTask<DefendZone>());

		Vector3 destiny = Info.GetWaypoint ("mid", allyFaction);

		float midInfl = Info.GetAreaInfluence(enemyFaction, Info.GetWaypoint ("mid", allyFaction), 10);
		float frontInfl = Info.GetAreaInfluence(enemyFaction, Info.GetWaypoint ("mid", allyFaction), 10);

	//	Debug.Log ("Las influencias son: mid -> " + midInfl + ", front -> " + frontInfl);
		string dest = "mid";
		if (midInfl > 0.5) { // TODO Numero tentativo a cambios
			if (frontInfl > 0.5) {
				destiny = allyBase;
				dest = "base";
			} else {
				destiny = Info.GetWaypoint ("front", allyFaction);
				dest = "front";
			}
		}

	//	Debug.Log ("El destino al que deben ir es " + dest);

        foreach (var ally in alliesToDefendBridge) {
            Debug.Assert(!ally.HasTask<DefendZone>());
            ally.SetTask(new GoTo(ally, destiny, Mathf.Infinity, 1.3f, false, (bool success) => {
                ally.SetTask(new DefendZone(ally, ally.position, 6f, (_) => {
                }));
            }));
        }
    }
}
