using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MilitaryResourcesAllocator : MonoBehaviour {

    Dictionary<StrategyT, float> weights; //let's better call it priority or interest, weight is another thing
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

        weights = new Dictionary<StrategyT, float>() {
            { StrategyT.ATK_BASE, 0.5f},
            { StrategyT.ATK_HALF, 0.5f},
            { StrategyT.DEF_BASE, 0.8f},
            { StrategyT.DEF_HALF, 0.2f}
        };
    }

    public void AllocateResources() {
        HashSet<AgentUnit> availableUnits = new HashSet<AgentUnit>(Map.unitList.Where(unit => unit.faction == faction));
        int nTotalAvailableUnits = availableUnits.Count;
        Debug.Log("Total units " + nTotalAvailableUnits);
        /*foreach (StrategyT strategy in weights.Keys.ToList()) {
            if (weights[strategy] < 0.2) {
                weights.Remove(strategy);
            }
            //weight.Value *= weighting
        }*/

        if (weights.Count == 0) {
            Debug.LogError("No strategy has enough importance");
        }

        //Normalize
        float sum = weights.Sum(w => w.Value);
        foreach (StrategyT strategy in weights.Keys.ToList()) {
            weights[strategy] /= sum;
        }

        Dictionary<StrategyT, HashSet<AgentUnit>> unitsAssignedToStrategy = new Dictionary<StrategyT, HashSet<AgentUnit>>();

        //Map to number of units
        //If there are remaining units due to rounding errors are assigned to the most important strategy
        Dictionary<StrategyT, int> nUnitsAllocToStrategy = weights.ToDictionary(w => w.Key, w => Mathf.FloorToInt(w.Value * nTotalAvailableUnits));

        StrategyT mostImportantStrategy = weights.OrderBy(strategy => strategy.Value).Last().Key;
        Debug.Log("most important:" + mostImportantStrategy);
        nUnitsAllocToStrategy[mostImportantStrategy] += nTotalAvailableUnits - nUnitsAllocToStrategy.Sum(w => w.Value);
        Debug.Assert(nUnitsAllocToStrategy.Sum(w => w.Value) == nTotalAvailableUnits);

        foreach (var z in nUnitsAllocToStrategy) {
            Debug.Log(z.Key + " " + z.Value + "; weight: " + weights[z.Key]);
        }

        //Assign units with the same strategy
        foreach (StrategyT strategy in weights.Keys) {
            var selectedUnits = availableUnits.Where(unit => unit.strategy == strategy).Take(nUnitsAllocToStrategy[strategy]);
            unitsAssignedToStrategy[strategy] = new HashSet<AgentUnit>(selectedUnits);
            availableUnits.ExceptWith(unitsAssignedToStrategy[strategy]);
            nUnitsAllocToStrategy[strategy] -= unitsAssignedToStrategy[strategy].Count();
        }

        Debug.Assert(nUnitsAllocToStrategy.Sum(w => w.Value) == availableUnits.Count);

        //For each strategy 
        //Foreach unit get how many units have assigned strategy (from previous iteration or from units_strategy?)
        //Sort units by their maximun "affinity" so in every iteration we assign a unit to the required strategy to which
        //it is more affine

        Dictionary<AgentUnit, Dictionary<StrategyT, float>> strategyAffinity = availableUnits.ToDictionary(u => u, u => info.GetStrategyPriority(u, faction));
        while (weights.Keys.Any(s => nUnitsAllocToStrategy[s] > 0)) {
            foreach (StrategyT strategy in weights.Keys.Where(s => nUnitsAllocToStrategy[s] > 0)) {
                AgentUnit mostAffineUnit = strategyAffinity.OrderBy(unit => unit.Value[strategy]).Last().Key;

                unitsAssignedToStrategy[strategy].Add(mostAffineUnit);
                strategyAffinity.Remove(mostAffineUnit);
                availableUnits.Remove(mostAffineUnit);
                nUnitsAllocToStrategy[strategy]--;
            }
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

        foreach (StrategyT strategy in weights.Keys) {
            Console.Log("Strategy: " + strategy.ToString() + " = " + unitsAssignedToStrategy[strategy].Count + " units");

            foreach (AgentUnit unit in unitsAssignedToStrategy[strategy]) {
                unit.GetComponent<Renderer>().material.color = strategyColor[strategy];
            }
        }
    }


    public void SetWeights(Dictionary<StrategyT, float> weights) {
        this.weights = weights;
    }

    public void SetFaction(Faction faction) {
        this.faction = faction;
    }
}