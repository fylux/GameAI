using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MilitaryResourcesAllocator {

    /* Dictionary<StrategyT, HashSet<AgentUnit>> strategyUnits = new Dictionary<StrategyT, HashSet<AgentUnit>>()
     {
         { StrategyT.ATK_BASE, new HashSet<AgentUnit>() },
         { StrategyT.ATK_HALF, new HashSet<AgentUnit>() },
         { StrategyT.DEF_BASE, new HashSet<AgentUnit>() },
         { StrategyT.DEF_HALF, new HashSet<AgentUnit>() }
     };*/

    Dictionary<StrategyT, float> importanceWeigth;
    public Dictionary<StrategyT, float> priority; //TESTGGG cambiar la visibilidad a privado
    Dictionary<StrategyT, float> offensiveWeight = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, 0.2f},
            { StrategyT.ATK_HALF, 0.5f},
            { StrategyT.DEF_BASE, -0.5f},
            { StrategyT.DEF_HALF, -0.2f}
        };
    float offensiveFactor;

    public static Dictionary<StrategyT, Color> strategyColor = new Dictionary<StrategyT, Color>() {
            { StrategyT.ATK_BASE, Color.magenta},
            { StrategyT.ATK_HALF, Color.red},
            { StrategyT.DEF_BASE, Color.green},
            { StrategyT.DEF_HALF,Color.cyan}
        };

    public Faction faction;

    public MilitaryResourcesAllocator(Faction faction, float atkbase, float defbase, float atkhalf, float defhalf) {
        this.faction = faction;

        priority = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, atkbase},
            { StrategyT.ATK_HALF, atkhalf},
            { StrategyT.DEF_BASE, defbase},
            { StrategyT.DEF_HALF, defhalf}
        };

        importanceWeigth = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, 1},
            { StrategyT.ATK_HALF, 1},
            { StrategyT.DEF_BASE, 1},
            { StrategyT.DEF_HALF, 1}
        };
    }


    public Dictionary<StrategyT, HashSet<AgentUnit>> AllocateResources() {
        //  ClearUnitSets(); // Vaciamos los sets que contienen las unidades de cada estrategia
        HashSet<AgentUnit> availableUnits = new HashSet<AgentUnit>(Map.unitList.Where(unit => unit.faction == faction));

        int nTotalAvailableUnits = availableUnits.Count;

        //All strategies must have a set, even if it is empty
        Dictionary<StrategyT, HashSet<AgentUnit>> unitsAssignedToStrategy = priority.Keys.ToDictionary(strategy => strategy, _ => new HashSet<AgentUnit>());

   //     Debug.Log("Total units " + availableUnits.Count);

        WeigthStrategies();
        NormalizeStrategies();
        PruneStrategies();

        if (priority.Count == 0) {
            Debug.Log("No strategy has enough importance. Giving all importance to DEF_BASE");
            unitsAssignedToStrategy[StrategyT.DEF_BASE] = new HashSet<AgentUnit>(availableUnits);

            foreach (AgentUnit unit in unitsAssignedToStrategy[StrategyT.DEF_BASE]) {
                //unit.hat.GetComponent<Renderer>().material.color = strategyColor[StrategyT.DEF_BASE];
                if (unit.strategy != StrategyT.DEF_BASE) {
                    unit.ResetTask();
                }
                unit.strategy = StrategyT.DEF_BASE;
            }
            return unitsAssignedToStrategy;
        }

        NormalizeStrategies();


        //Map to number of units
        //If there are remaining units due to rounding errors are assigned to the most important strategy
        foreach (var z in priority.Keys) {
            Debug.Log(Time.frameCount + " z: " + priority[z]);
        }
        Dictionary<StrategyT, int> nUnitsAllocToStrategy = priority.ToDictionary(w => w.Key, w => Mathf.FloorToInt(w.Value * nTotalAvailableUnits));

        //Asign remaining units to the most important strategy
        /*StrategyT mostImportantStrategy = priority.OrderBy(strategy => strategy.Value).Last().Key;
        nUnitsAllocToStrategy[mostImportantStrategy] += nTotalAvailableUnits - nUnitsAllocToStrategy.Sum(w => w.Value);
        Debug.Assert(nUnitsAllocToStrategy.Sum(w => w.Value) == nTotalAvailableUnits);*/


        //Asign remaining units to the strategies with biggest rounding error
        Debug.Log(Time.frameCount +" count "+nUnitsAllocToStrategy.Count);
        Debug.Log(Time.frameCount + " sum " + nUnitsAllocToStrategy.Sum(w => w.Value));
		
        int nRemainingUnits = nTotalAvailableUnits - nUnitsAllocToStrategy.Sum(w => w.Value);

        var strategiesByAllocResidual = priority.OrderByDescending(s => (s.Value * nTotalAvailableUnits) - Mathf.FloorToInt(s.Value * nTotalAvailableUnits))
                                        .Select(s => s.Key)
                                        .Take(nRemainingUnits);
        foreach (StrategyT strategy in strategiesByAllocResidual) {
            nUnitsAllocToStrategy[strategy]++;
        }

        Dictionary<AgentUnit, Dictionary<StrategyT, float>> strategyAffinity = availableUnits.ToDictionary(u => u, u => Info.GetStrategyPriority(u, faction));

        //Assign units with the same strategy
        foreach (StrategyT strategy in priority.Keys) {
            var selectedUnits = availableUnits.Where(u => u.strategy == strategy)
                                                .OrderBy(u => strategyAffinity[u][strategy])
                                                .Take(nUnitsAllocToStrategy[strategy]);

            unitsAssignedToStrategy[strategy] = new HashSet<AgentUnit>(selectedUnits);
            foreach (var unit in unitsAssignedToStrategy[strategy]) {
                availableUnits.Remove(unit);
                strategyAffinity.Remove(unit);
                nUnitsAllocToStrategy[strategy]--;
            }
        }

        Debug.Log("Available units " + availableUnits.Count);
        if (nUnitsAllocToStrategy.Sum(w => w.Value) != availableUnits.Count) {
            Debug.Log(nUnitsAllocToStrategy.Sum(w => w.Value) +" "+ availableUnits.Count);
        }
        Debug.Assert(nUnitsAllocToStrategy.Sum(w => w.Value) == availableUnits.Count);

        //Assign units to strategies based on affinity
        while (priority.Keys.Any(s => nUnitsAllocToStrategy[s] > 0)) {
            var remainingStrategies = new HashSet<StrategyT>(priority.Keys.Where(s => nUnitsAllocToStrategy[s] > 0));

            var mostAffineUnit = strategyAffinity.OrderBy(unit => unit.Value.Where(s => remainingStrategies.Contains(s.Key)).Min(s => s.Value)).First().Key;

            var strategy = strategyAffinity[mostAffineUnit].Where(s => remainingStrategies.Contains(s.Key)).OrderBy(s => s.Value).First().Key;

            //    Debug.Log("Most affine unit for " + strategy + " is " + mostAffineUnit.name + ", affinity: " + strategyAffinity[mostAffineUnit][strategy]);

            unitsAssignedToStrategy[strategy].Add(mostAffineUnit);
            strategyAffinity.Remove(mostAffineUnit);
            availableUnits.Remove(mostAffineUnit);
            nUnitsAllocToStrategy[strategy]--;
            /*
            //ROUND ROBIN
            foreach (StrategyT strategy in weights.Keys.Where(s => nUnitsAllocToStrategy[s] > 0)) {
                AgentUnit mostAffineUnit = strategyAffinity.OrderBy(unit => unit.Value[strategy]).First().Key;
                Debug.Log("Most affine unit for " + strategy + " is " + mostAffineUnit.name + ", affinity: " + strategyAffinity[mostAffineUnit][strategy]);

                unitsAssignedToStrategy[strategy].Add(mostAffineUnit);
                strategyAffinity.Remove(mostAffineUnit);
                availableUnits.Remove(mostAffineUnit);
                nUnitsAllocToStrategy[strategy]--;
            }*/
        }


        //Asign units randomly
        /*HashSet<AgentUnit> assignedUnits = new HashSet<AgentUnit>();
        foreach (AgentUnit unit in availableUnits) {
            foreach (StrategyT strategy in weights.Keys) {
                if (nUnitsAllocToStrategy[strategy] > 0) {
                    unitsAssignedToStrategy[strategy].Add(unit);
                    assignedUnits.Add(unit);
                    nUnitsAllocToStrategy[strategy]--;
                    break;
                }
            }
        }
        availableUnits.ExceptWith(assignedUnits);*/

        Debug.Assert(availableUnits.Count == 0);

        foreach (StrategyT strategy in priority.Keys) {
            Console.Log("Strategy: " + strategy.ToString() + " = " + unitsAssignedToStrategy[strategy].Count + " units");

            foreach (AgentUnit unit in unitsAssignedToStrategy[strategy]) {
                unit.hat.GetComponent<Renderer>().material.color = strategyColor[strategy];
                if (unit.strategy != strategy) {
                    unit.ResetTask();
                }
                unit.strategy = strategy;

            }
        }

        return unitsAssignedToStrategy;
    }

    public void WeigthStrategies() {
        foreach (StrategyT strategy in priority.Keys.ToList()) {
            priority[strategy] *= importanceWeigth[strategy];
            priority[strategy] += offensiveWeight[strategy] * offensiveFactor;
        }
    }

    public void NormalizeStrategies() {
        float sum = priority.Sum(w => w.Value);
        //Debug.Assert(sum > 0f);
        if (!(sum > 0)) sum = 1f;
        Debug.Log("normalize sum " + sum);
        foreach (StrategyT strategy in priority.Keys.ToList()) {
            priority[strategy] /= sum;
        }
    }

    //Here we can remove strategies that do not have enough importance or do not fulfill the requirements to be considered
    public void PruneStrategies() {
        foreach (StrategyT strategy in priority.Keys.ToList()) {
            if (priority[strategy] < 0.2) {
                priority.Remove(strategy);
            }
        }
    }

    public void SetPriority(Dictionary<StrategyT, float> priority) {
        this.priority = new Dictionary<StrategyT, float>(priority);
    }

    public void SetImportanceWeights(Dictionary<StrategyT, float> weights) {
        this.importanceWeigth = weights;
    }

    public void SetFaction(Faction faction) {
        this.faction = faction;
    }

    public void SetOffensiveFactor(float offensiveFactor) {
        this.offensiveFactor = offensiveFactor;
    }
}