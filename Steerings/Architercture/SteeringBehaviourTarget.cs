using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SeekT {
    MILLINGTON, REYNOLDS
};

public abstract class SteeringBehaviourTarget : SteeringBehaviour {

    [SerializeField]
    protected Body target;

}
