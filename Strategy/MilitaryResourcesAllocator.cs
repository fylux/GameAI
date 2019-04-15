using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MilitaryResourcesAllocator {

    Dictionary<StrategyT, float> weights; //let's better call it priority or interest, weight is another thing
    Dictionary<StrategyT, Color> strategyColor = new Dictionary<StrategyT, Color>() {
            { StrategyT.ATK_BASE, Color.magenta},
            { StrategyT.ATK_HALF, Color.red},
            { StrategyT.DEF_BASE, Color.green},
            { StrategyT.DEF_HALF,Color.cyan}
        };

    InfoManager infoManager;

    public MilitaryResourcesAllocator(InfoManager infoManager) {
        this.infoManager = infoManager;

        weights = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, 0.5f},
            { StrategyT.ATK_HALF, 0.5f},
            { StrategyT.DEF_BASE, 0.8f},
            { StrategyT.DEF_HALF, 0.2f}
        };
    }

    public void AllocateResources() {
        int nTotalAvailableUnits = Map.unitList.Count;
        HashSet<AgentUnit> availableUnits = new HashSet<AgentUnit>(Map.unitList);

        foreach (StrategyT strategy in weights.Keys.ToList()) {
            if (weights[strategy] < 0.2) {
                weights.Remove(strategy);
            }
            //weight.Value *= weighting
        }

        //Normalize
        float sum = weights.Sum(w => w.Value);
        foreach (StrategyT strategy in weights.Keys.ToList()) {
            weights[strategy] /= sum;

        }

        //Map to number of units
        //We ignore rounding of float, we just assign till there are available
        // Dictionary<StrategyT, int> nUnitsAllocToStrategy= weights.ToDictionary(w => w.Key, w => (int)w.Value * nTotalAvailableUnits);
        Dictionary<StrategyT, int> nUnitsAllocToStrategy = new Dictionary<StrategyT, int>();
        foreach (StrategyT strategy in weights.Keys.ToList()) {
            Debug.Log("StrategyT " + strategy.ToString() + " units " + weights[strategy] * nTotalAvailableUnits);
            nUnitsAllocToStrategy[strategy] = (int)(weights[strategy] * nTotalAvailableUnits);
        }
        Debug.Assert(nUnitsAllocToStrategy.Sum(w => w.Value) <= nTotalAvailableUnits);

        Dictionary<StrategyT, HashSet<AgentUnit>> unitsAssignedToStrategy = new Dictionary<StrategyT, HashSet<AgentUnit>>();


        foreach (StrategyT strategy in weights.Keys.ToList()) {
            //Get list of units with "strategy assigned"
            List<AgentUnit> unitsWithStrategy = availableUnits.Where(unit => unit.strategy == strategy).ToList();

            unitsAssignedToStrategy[strategy] = new HashSet<AgentUnit>(unitsWithStrategy.Take(nUnitsAllocToStrategy[strategy]));
            nUnitsAllocToStrategy[strategy] -= unitsAssignedToStrategy[strategy].Count();
            availableUnits.ExceptWith(unitsAssignedToStrategy[strategy]);
        }

        //For each strategy 
        //Sort units by their maximun "affinity" so in every iteration we assign a unit to the required strategy to which
        //it is more affine
        Dictionary<AgentUnit, Dictionary<StrategyT, float>> unitAffinityToStrategy;
        unitAffinityToStrategy = availableUnits.ToDictionary(unit => unit, unit => infoManager.GetStrategyPriority(unit));

        while (availableUnits.Count > 0 && nUnitsAllocToStrategy.Keys.Any(nUnits => nUnits > 0)) {
            List<StrategyT> strategiesLeft = weights.Keys.Where(strategy => nUnitsAllocToStrategy[strategy] > 0).ToList();

            foreach (StrategyT strategy in strategiesLeft) {
                AgentUnit selectedUnit = unitAffinityToStrategy.OrderBy(unit => unit.Value[strategy]).Last().Key;
                unitsAssignedToStrategy[strategy].Add(selectedUnit);
                availableUnits.Remove(selectedUnit);
                nUnitsAllocToStrategy[strategy]--;
                unitAffinityToStrategy.Remove(selectedUnit);
            }
        }

        //Asign units randomly
        /*HashSet<AgentUnit> assignedUnits = new HashSet<AgentUnit>();
        foreach (AgentUnit unit in availableUnits) {
            foreach (StrategyT strategy in weights.Keys.ToList()) {
                if (nUnitsAllocToStrategy[strategy] > 0) {
                    unitsAssignedToStrategy[strategy].Add(unit);
                    assignedUnits.Add(unit);
                    nUnitsAllocToStrategy[strategy]--;
                    break;
                }
            }
        }
        availableUnits.ExceptWith(assignedUnits);*/

        //There might be still available units due to rounding errors, they will be assigned to the most weighted strategy
        StrategyT mostImportantStrategy = weights.FirstOrDefault(strategy => strategy.Value == weights.Values.Max()).Key;
        unitsAssignedToStrategy[mostImportantStrategy].UnionWith(availableUnits);
        availableUnits.Clear();
        Console.Log("Most important Strategy: " + mostImportantStrategy.ToString());


        foreach (StrategyT strategy in weights.Keys.ToList()) {
            Console.Log("Strategy: " + strategy.ToString() + " = " + unitsAssignedToStrategy[strategy].Count + " units");

            foreach (AgentUnit unit in unitsAssignedToStrategy[strategy]) {
                unit.GetComponent<Renderer>().material.color = strategyColor[strategy];
            }
        }
    }


    public void SetWeights(Dictionary<StrategyT, float> weights) {
        this.weights = weights;
    }
}
