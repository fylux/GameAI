using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public enum DistanceT {
    EUCLIDEAN, MANHATTAN, CHEBYSHEV
};

public class Pathfinding : MonoBehaviour {

    protected Map grid;

    protected void Awake() {
        grid = GetComponent<Map>();
    }

}
