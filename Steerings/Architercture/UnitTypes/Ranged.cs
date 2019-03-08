﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : AgentNPC {

   Dictionary<NodeT, float> cost = new Dictionary<NodeT, float>() {
            { NodeT.ROAD, 1 },
            { NodeT.GRASS, 1.75f },
            { NodeT.FOREST, 2.5f },
            { NodeT.WATER, Mathf.Infinity},
            { NodeT.MOUNTAIN, Mathf.Infinity}
    };

    // Los atributos de los Ranged son con los que hemos estado trabajando, los default
}
