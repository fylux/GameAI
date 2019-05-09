using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationManager {

    StrategyManager strategyManager;

    Dictionary<UnitT, int> idealDist = new Dictionary<UnitT, int>()
    {
        { UnitT.MELEE, 8 },
        { UnitT.RANGED, 5 },
        { UnitT.SCOUT, 4 },
        { UnitT.ARTIL, 3 }
    };

    Dictionary<UnitT, int> realDist;

    public GenerationManager(StrategyManager strat)
    {
        strategyManager = strat;
    }

	public UnitT GetMostImportantUnit()
    {
        UpdateRealDist();
        Debug.Log("La distribucion a la que deberiamos llegar es " + realDist[UnitT.MELEE] + "M, " + realDist[UnitT.RANGED] + "R, " + realDist[UnitT.SCOUT] + "S, " + realDist[UnitT.ARTIL] + "A");

        HashSet<AgentUnit> allies = Map.GetAllies(strategyManager.faction);

        Dictionary<UnitT, int> alliesControled = new Dictionary<UnitT, int>()
        {
            { UnitT.MELEE, 0 },
            { UnitT.RANGED, 0 },
            { UnitT.SCOUT, 0 },
            { UnitT.ARTIL, 0 }
        };

        foreach (AgentUnit unit in allies)
        {
            alliesControled[unit.GetUnitType()]++;
        }

        Debug.Log("La distribucion que tenemos actualmente es " + alliesControled[UnitT.MELEE] + "M, " + alliesControled[UnitT.RANGED] + "R, " + alliesControled[UnitT.SCOUT] + "S, " + alliesControled[UnitT.ARTIL] + "A");

        Dictionary<UnitT, float> actualDistribution = new Dictionary<UnitT, float>()
        {
            { UnitT.MELEE, Divide(alliesControled[UnitT.MELEE],realDist[UnitT.MELEE]) },
            { UnitT.RANGED, Divide(alliesControled[UnitT.RANGED],realDist[UnitT.RANGED]) },
            { UnitT.SCOUT, Divide(alliesControled[UnitT.SCOUT],realDist[UnitT.SCOUT]) },
            { UnitT.ARTIL, Divide(alliesControled[UnitT.ARTIL],realDist[UnitT.ARTIL]) }
        };

        Debug.Log("Los porcentajes son " + actualDistribution[UnitT.MELEE] + "M, " + actualDistribution[UnitT.RANGED] + "R, " + actualDistribution[UnitT.SCOUT] + "S, " + actualDistribution[UnitT.ARTIL] + "A");

        UnitT mostValuable = UnitT.MELEE; // Hay que inicializarlo a la fuerza
        float smallestPercent = 1;

        foreach (KeyValuePair<UnitT,float> tuple in actualDistribution)
        {
            if (tuple.Value < smallestPercent)
            {
                mostValuable = tuple.Key;
                smallestPercent = tuple.Value;
            }
        }

        Debug.Log("La unidad elegida para crearse es " + mostValuable);

        return mostValuable;

    }

    void UpdateRealDist()
    {
        Dictionary<UnitT, int> modifications = new Dictionary<UnitT, int>()
        {
            { UnitT.MELEE, 0 },
            { UnitT.RANGED, 0 },
            { UnitT.SCOUT, 0 },
            { UnitT.ARTIL, 0 }
        };

        realDist = new Dictionary<UnitT, int>(idealDist);

        HashSet<AgentUnit> enemies = Map.GetEnemies(strategyManager.faction);


        StrategyLayer stratL = strategyManager.GetStrategyLayer();
        StrategyT mostPriority = StrategyT.DEF_BASE; // Hay que inicializarlo a la fuerza
        float mostValue = 0;

        foreach (KeyValuePair<StrategyT,float> tuple in stratL.priority)
            if (tuple.Value > mostValue)
                mostPriority = tuple.Key;

        if (mostPriority != StrategyT.ATK_BASE && mostPriority != StrategyT.DEF_BASE)
        {
            modifications[UnitT.ARTIL] -= 2;
            modifications[UnitT.RANGED] += 2;
            Debug.Log("Como no estamos atacando ni defendiendo base principalmente, -2A, +2R");
        }

        int numberScouts = 0;
        int numberRangeds = 0;

        foreach (AgentUnit enemy in enemies)
        {
            if (enemy is Scout)
                numberScouts++;
            else if (enemy is Ranged)
                numberRangeds++;
        }

        if (numberScouts >= 5)
        {
            modifications[UnitT.ARTIL] += 2;
            modifications[UnitT.RANGED] -= 2;
            Debug.Log("Como hay muchos scouts enemigos, -2R, +2A");
        }

        numberRangeds -= 5;
        numberRangeds = Mathf.Clamp(numberRangeds, -4, 8);
        modifications[UnitT.SCOUT] += numberRangeds;
        modifications[UnitT.MELEE] -= numberRangeds;
        Debug.Log("Debido al numero de Rangeds rivales, " + numberRangeds + "S, " + (-numberRangeds) + "M");

        foreach (UnitT unit in modifications.Keys)
        {
            realDist[unit] += modifications[unit];
        }
    }

    float Divide(int A, int B)
    {
        if (B == 0) return 1;

        return (float)A / B;
    }
}
