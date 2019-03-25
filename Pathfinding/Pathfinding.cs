using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public enum DistanceT {
    EUCLIDEAN, MANHATTAN, CHEBYSHEV
};

public class Pathfinding {

    protected Map map;

    public Pathfinding() {
        map = GameObject.Find("Terrain").GetComponent<Map>();
    }

}
