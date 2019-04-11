using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MilitaryResourcesAllocator {

    Dictionary<Strategy, float> weights; //let's better call it priority or interest, weight is another thing
    Dictionary<Strategy, Color> strategyColor = new Dictionary<Strategy, Color>() {
            { Strategy.ATK_BASE, Color.magenta},
            { Strategy.ATK_HALF, Color.red},
            { Strategy.DEF_BASE, Color.green},
            { Strategy.DEF_HALF,Color.cyan}
        };

    public void AllocateResources() {
        weights = new Dictionary<Strategy, float>() {
            { Strategy.ATK_BASE, 0.5f},
            { Strategy.ATK_HALF, 0.5f},
            { Strategy.DEF_BASE, 0.8f},
            { Strategy.DEF_HALF, 0.2f}
        };
        int nTotalAvailableUnits = Map.unitList.Count;

        foreach (Strategy strategy in weights.Keys.ToList()) {
            if (weights[strategy] < 0.2) {
                weights.Remove(strategy);
            }
            //weight.Value *= weighting
        }

        //Normalize
        float sum = weights.Sum(w => w.Value);
        foreach (Strategy strategy in weights.Keys.ToList()) {
            weights[strategy] /= sum;

        }

        //Map to number of units
        //We ignore rounding of float, we just assign till there are available
        Dictionary<Strategy, int> nUnitsAllocToStrategy = new Dictionary<Strategy, int>();
        foreach (Strategy strategy in weights.Keys.ToList()) {
            Debug.Log("Strategy " + strategy.ToString() + " units " + weights[strategy] * nTotalAvailableUnits);
            nUnitsAllocToStrategy[strategy] = (int) (weights[strategy] * nTotalAvailableUnits);
        }
        Debug.Assert(nUnitsAllocToStrategy.Sum(w => w.Value) <= nTotalAvailableUnits);

        Dictionary<Strategy, HashSet<AgentUnit>> unitsAssignedToStrategy = new Dictionary<Strategy, HashSet<AgentUnit>>();
        HashSet<AgentUnit> availableUnits = new HashSet<AgentUnit>(Map.unitList);

        foreach (Strategy strategy in weights.Keys.ToList()) {
            unitsAssignedToStrategy[strategy] = new HashSet<AgentUnit>();
            //Get list of units with "strategy assigned"
            List<AgentUnit> unitsWithStrategy = availableUnits.Where(unit => unit.strategy == strategy).ToList();
            foreach (AgentUnit unit in unitsWithStrategy) {
                if (nUnitsAllocToStrategy[strategy] <= 0) {
                    break;
                }
                if (true) { //&& blabla
                    unitsAssignedToStrategy[strategy].Add(unit);
                    availableUnits.Remove(unit);
                    nUnitsAllocToStrategy[strategy]--;
                }
            }
        }

        //For each strategy 
        //Foreach unit get how many units have assigned strategy (from previous iteration or from units_strategy?)
        //Sort units by their maximun "affinity" so in every iteration we assign a unit to the required strategy to which
        //it is more affine

        //Asign units randomly
        HashSet<AgentUnit> assignedUnits = new HashSet<AgentUnit>();
        foreach (AgentUnit unit in availableUnits) {
            foreach (Strategy strategy in weights.Keys.ToList()) {
                if (nUnitsAllocToStrategy[strategy] > 0) {
                    unitsAssignedToStrategy[strategy].Add(unit);
                    assignedUnits.Add(unit);
                    nUnitsAllocToStrategy[strategy]--;
                    break;
                }
            }
        }
        availableUnits.ExceptWith(assignedUnits);

        //There might be still available units due to rounding errors, they will be assigned to the most weighted strategy
        Strategy mostImportantStrategy = weights.FirstOrDefault(strategy => strategy.Value == weights.Values.Max()).Key;
        unitsAssignedToStrategy[mostImportantStrategy].UnionWith(availableUnits);
        availableUnits.Clear();
        Console.Log("Most important Strategy: " + mostImportantStrategy.ToString());


        foreach (Strategy strategy in weights.Keys.ToList()) {
            Console.Log("Strategy: "+strategy.ToString()+" = "+ unitsAssignedToStrategy[strategy].Count + " units");

            foreach (AgentUnit unit in unitsAssignedToStrategy[strategy]) {
                unit.GetComponent<Renderer>().material.color = strategyColor[strategy];
            }
        }
    }


    public void SetWeights(Dictionary<Strategy, float> weights) {
        this.weights = weights;
    }
}
