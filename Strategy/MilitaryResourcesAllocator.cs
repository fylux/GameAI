using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MilitaryResourcesAllocator {

    Dictionary<Strategy, float> weights; //let's better call it priority or interest, weight is another thing


    public void AllocateResources() {
 
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
        /*foreach (Strategy strategy in weights.Keys.ToList()) {
            n_units_alloc[strategy] = weights[strategy] * nTotalAvailableUnits;
        }*/

        HashSet<AgentUnit> used_units = new HashSet<AgentUnit>();
        //Dictionary units_strategy
        /*foreach (Strategy strategy in weights.Keys.ToList()) {
            List<AgentUnit> unitsWithStrategy = new List<AgentUnit>();//Get list of units with "strategy assigned"
            foreach (AgentUnit unit in unitsWithStrategy) {
                if (n_units_alloc[strategy] < 0) {
                    break;
                }
                if (!used_units.Contains(unit) ) { //&& blabla
                    units_strategy[strategy].Add(unit);
                    n_units_alloc[strategy]--;
                }
                
            }
        }*/

        //Now we have left the units that will have to change their strategy

        //For each strategy 
             //Foreach unit get how many units have assigned strategy (from previous iteration or from units_strategy?)
        
        //Sort units by their maximun "affinity" so in every iteration we assign a unit to the required strategy to which
        //it is more affine

    }



    public void SetWeights(Dictionary<Strategy, float> weights) {
        this.weights = weights;
    }
}
