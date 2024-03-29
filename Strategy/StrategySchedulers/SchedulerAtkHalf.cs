﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SchedulerAtkHalf : SchedulerStrategy {

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

		//Units with low level of health should try to go to a healing point
		var damagedAllies = usableUnits.Where(unit => unit.militar.health < minimunHealth && !(unit.GetTask() is RestoreHealth));
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

		var remainingUnits = new HashSet<AgentUnit>(usableUnits.Where(unit => !(unit.GetTask() is RestoreHealth)));

		var clusters = Info.GetClusters(enemyFaction, enemyFaction);

		var selectedCluster = clusters.ToDictionary(c => c, c => GetMilitaryBalanceCluster(c)).OrderByDescending(c => c.Value).FirstOrDefault().Key;
        if (selectedCluster != null)
        {
            HashSet<AgentUnit> totalUnits = new HashSet<AgentUnit>(remainingUnits);
            totalUnits.UnionWith(selectedCluster);

            if (!(Info.MilitaryAdvantage(totalUnits, allyFaction) < 1))
            {
                //var unitsAssignedToCluster = new HashSet<AgentUnit>(); -> Es remainingUnits
                var center = Info.GetClusterCenter(selectedCluster);

                foreach (AgentUnit ally in remainingUnits)
                {
                    if (ally.GetTask() is Attack) continue;

                    var enemiesByDistance = selectedCluster.OrderBy(unit => Util.HorizontalDist(ally.position, unit.position));
                    AgentUnit closestEnemy = enemiesByDistance.First();
                    var distanceToEnemy = Util.HorizontalDist(ally.position, closestEnemy.position);
                    
                    foreach (var unitType in ally.GetPreferredEnemies())
                    {
                        var closestEnemyOfType = enemiesByDistance.Where(u => u.GetUnitType() == unitType).FirstOrDefault();
                        
                        if (closestEnemyOfType != null && Util.HorizontalDist(ally.position, closestEnemyOfType.position) < distanceToEnemy + 4)
                        {
//                            Debug.Log(ally + " closest "+closestEnemy);
    
                            ally.SetTask(new Attack(ally, closestEnemyOfType, (_) => {
                                //If you kill an enemy reconsider assignations
                                ApplyStrategy();
                            }));
                            break;
                        }
                        
                    }
                    
                }
            }
        }


        //var unitsCluster = new HashSet<AgentUnit>(selectedCluster);

        /*var closestAllies = remainingUnits.OrderBy(unit => Util.HorizontalDist(center, unit.position));

		//Assign the closest allies to that cluster
		foreach (AgentUnit closestAlly in closestAllies) {
			unitsCluster.Add(closestAlly);
			//Till the balance between the cluster and the assign allies is positive
			if (Info.MilitaryAdvantage(unitsCluster, allyFaction) > 1) {
				var alliesToCluster = new HashSet<AgentUnit>(unitsCluster.Where(unit => unit.faction == allyFaction));
				unitsAssignedToCluster.Add(alliesToCluster);
				remainingUnits.ExceptWith(alliesToCluster);
				break;
			}
		}*/

        //Take the units that haven't been assigned and distribute them between clusters
        /*
		var priorityClusters = unitsAssignedToClusters.ToDictionary(c => c.Key, c => Info.MilitaryAdvantage(c.Key, enemyFaction));
		var remainingUnitsToCluster = DistributeClusters(priorityClusters, remainingUnits.Count);*/

        //Foreach cluster sorted by the advantage that we have
        //foreach (HashSet<AgentUnit> cluster in remainingUnitsToCluster.Select(c => c.Key)) {
        /*	int nAssignedUnits = remainingUnitsToCluster[cluster];
			var closestAllies = remainingUnits.OrderBy(unit => Util.HorizontalDist(center, unit.position)).Take(nAssignedUnits);

			//Assign the corresponding number of closest allies to that cluster
			unitsAssignedToClusters[cluster].UnionWith(closestAllies);
			remainingUnits.ExceptWith(unitsAssignedToClusters[cluster]);
		//}*/


        //Asign tasks to units in the selected clusters
        //foreach (var cluster in unitsAssignedToClusters) {

        //}


        //Remaining units go to defend the bridge
        var alliesToDefendBridge = remainingUnits.Where(unit => !unit.HasTask<GoTo, DefendZone, Attack>());

		float midInfl = Info.GetAreaInfluence(enemyFaction, Info.GetWaypoint ("mid", enemyFaction), 10);
		float frontInfl = Info.GetAreaInfluence(enemyFaction, Info.GetWaypoint ("front", enemyFaction), 10);

		//	Debug.Log ("Las influencias son: mid -> " + midInfl + ", front -> " + frontInfl);
		string dest = "front";
		Faction destFact = enemyFaction;

		if (frontInfl > 0.5) { // TODO Numero tentativo a cambios
			if (midInfl > 0.5) {
				destFact = allyFaction;
			} else {
				dest = "mid";
			}
		}

		foreach (var ally in alliesToDefendBridge) {
			ally.SetTask(new GoTo(ally, Info.GetWaypoint(dest, destFact), Mathf.Infinity, 1.3f, true, (bool success) => {
				ally.SetTask(new DefendZone(ally, ally.position, 6f, (_) => {
				}));
			}));
		}
	}
}
