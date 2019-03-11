using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : AgentNPC {

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
    }

    
}
