using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MilitaryResourcesAllocator : MonoBehaviour {

    /* Dictionary<StrategyT, HashSet<AgentUnit>> strategyUnits = new Dictionary<StrategyT, HashSet<AgentUnit>>()
     {
         { StrategyT.ATK_BASE, new HashSet<AgentUnit>() },
         { StrategyT.ATK_HALF, new HashSet<AgentUnit>() },
         { StrategyT.DEF_BASE, new HashSet<AgentUnit>() },
         { StrategyT.DEF_HALF, new HashSet<AgentUnit>() }
     };*/

    Dictionary<StrategyT, float> importanceWeigth;
    Dictionary<StrategyT, float> priority;
    Dictionary<StrategyT, float> offensiveWeight = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, 0.2f},
            { StrategyT.ATK_HALF, 0.5f},
            { StrategyT.DEF_BASE, -0.5f},
            { StrategyT.DEF_HALF, -0.2f}
        };
    float offensiveFactor;

    Dictionary<StrategyT, Color> strategyColor = new Dictionary<StrategyT, Color>() {
            { StrategyT.ATK_BASE, Color.magenta},
            { StrategyT.ATK_HALF, Color.red},
            { StrategyT.DEF_BASE, Color.green},
            { StrategyT.DEF_HALF,Color.cyan}
        };

    InfoManager info;
    public Faction faction;

    void Start() {
        info = GetComponent<InfoManager>();

        priority = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, 0.5f},
            { StrategyT.ATK_HALF, 0.5f},
            { StrategyT.DEF_BASE, 0.8f},
            { StrategyT.DEF_HALF, 0.2f}
        };

        importanceWeigth = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, 1},
            { StrategyT.ATK_HALF, 1},
            { StrategyT.DEF_BASE, 1},
            { StrategyT.DEF_HALF, 1}
        };
    }


    public void AllocateResources() {
        //  ClearUnitSets(); // Vaciamos los sets que contienen las unidades de cada estrategia
        HashSet<AgentUnit> availableUnits = new HashSet<AgentUnit>(Map.unitList.Where(unit => unit.faction == faction));
        int nTotalAvailableUnits = availableUnits.Count;
        Debug.Log("Total units " + nTotalAvailableUnits);

        weigthStrategies();
        normalizeStrategies();
        pruneStrategies();
        normalizeStrategies();

        if (priority.Count == 0) {
            Debug.Log("No strategy has enough importance");
        }

        Dictionary<StrategyT, HashSet<AgentUnit>> unitsAssignedToStrategy = new Dictionary<StrategyT, HashSet<AgentUnit>>();

        //Map to number of units
        //If there are remaining units due to rounding errors are assigned to the most important strategy
        Dictionary<StrategyT, int> nUnitsAllocToStrategy = priority.ToDictionary(w => w.Key, w => Mathf.FloorToInt(w.Value * nTotalAvailableUnits));

        StrategyT mostImportantStrategy = priority.OrderBy(strategy => strategy.Value).Last().Key;
        Debug.Log("most important:" + mostImportantStrategy);
        nUnitsAllocToStrategy[mostImportantStrategy] += nTotalAvailableUnits - nUnitsAllocToStrategy.Sum(w => w.Value);
        Debug.Assert(nUnitsAllocToStrategy.Sum(w => w.Value) == nTotalAvailableUnits);

        foreach (var z in nUnitsAllocToStrategy) {
            Debug.Log(z.Key + " " + z.Value + "; weight: " + priority[z.Key]);
        }

        Dictionary<AgentUnit, Dictionary<StrategyT, float>> strategyAffinity = availableUnits.ToDictionary(u => u, u => info.GetStrategyPriority(u, faction));

        //Assign units with the same strategy
        foreach (StrategyT strategy in priority.Keys) {
            var selectedUnits = availableUnits.Where(u => u.strategy == strategy)
                                                .OrderBy(u => strategyAffinity[u][strategy])
                                                .Take(nUnitsAllocToStrategy[strategy]);
            unitsAssignedToStrategy[strategy] = new HashSet<AgentUnit>(selectedUnits);
            availableUnits.ExceptWith(unitsAssignedToStrategy[strategy]);

            foreach (var unit in unitsAssignedToStrategy[strategy]) {
                Debug.Log("Unit " + unit.name + " selected same strategy " + strategy);
            }

            nUnitsAllocToStrategy[strategy] -= unitsAssignedToStrategy[strategy].Count();
        }

        Debug.Assert(nUnitsAllocToStrategy.Sum(w => w.Value) == availableUnits.Count);

        //Assign units to strategies based on affinity
        while (priority.Keys.Any(s => nUnitsAllocToStrategy[s] > 0)) {
            var remainingStrategies = new HashSet<StrategyT>(priority.Keys.Where(s => nUnitsAllocToStrategy[s] > 0));
            var mostAffineUnit = strategyAffinity.OrderBy(unit => unit.Value.Where(s => remainingStrategies.Contains(s.Key)).Min(s => s.Value)).First().Key;
            var strategy = strategyAffinity[mostAffineUnit].Where(s => remainingStrategies.Contains(s.Key)).OrderBy(s => s.Value).First().Key;

            Debug.Log("Most affine unit for " + strategy + " is " + mostAffineUnit.name + ", affinity: " + strategyAffinity[mostAffineUnit][strategy]);

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
                unit.GetComponent<Renderer>().material.color = strategyColor[strategy];
                unit.strategy = strategy;
            }
        }
    }

    public void weigthStrategies() {
        foreach (StrategyT strategy in priority.Keys.ToList()) {
            priority[strategy] *= importanceWeigth[strategy] + offensiveWeight[strategy] * offensiveFactor;
        }
    }

    public void normalizeStrategies() {
        float sum = priority.Sum(w => w.Value);
        foreach (StrategyT strategy in priority.Keys.ToList()) {
            priority[strategy] /= sum;
        }
    }
    
    //Here we can remove strategies that do not have enough importance or do not fulfill the requirements to be considered
    public void pruneStrategies() {
        foreach (StrategyT strategy in priority.Keys.ToList()) {
            if (priority[strategy] < 0.2) {
                priority.Remove(strategy);
            }
        }
    }


    /* void ClearUnitSets()
     {
         foreach (KeyValuePair<StrategyT,HashSet<AgentUnit>> tuple in strategyUnits)
         {
             tuple.Value.Clear();
         }
     }

     void CheckUnitsStrategy()
     {
         Debug.Log("Unidades --> ");
         Debug.Log("Defbase tiene " + strategyUnits[StrategyT.DEF_BASE].Count);
         Debug.Log("Defhalf tiene " + strategyUnits[StrategyT.DEF_HALF].Count);
         Debug.Log("Atkhalf tiene " + strategyUnits[StrategyT.ATK_HALF].Count);
         Debug.Log("Atkbase tiene " + strategyUnits[StrategyT.ATK_BASE].Count);


         foreach (KeyValuePair<StrategyT, HashSet<AgentUnit>> tuple in strategyUnits)
         {
             foreach (AgentUnit unit in tuple.Value)
             {
                 Debug.Log("La estrategia " + tuple.Key + " tiene a la unidad " + unit);
             }

         }
     }
     */
    public void SetPriority(Dictionary<StrategyT, float> priority) {
        this.priority = priority;
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