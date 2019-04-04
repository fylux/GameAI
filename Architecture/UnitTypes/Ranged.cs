using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : AgentUnit {

    protected new void Start()
    {
        base.Start();
        // Los atributos de los Ranged son con los que hemos estado trabajando, los default

        cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.75f },
            { NodeT.FOREST, 2.5f },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
        };

        atk = new Dictionary<UnitT, float>() { //Coste por defecto, para casos de prueba
            { UnitT.MELEE, 1 },
            { UnitT.RANGED, 0.8f },
            { UnitT.SCOUT, 1.5f },
            { UnitT.ARTIL, 0.5f }
        };
    }

    
}
