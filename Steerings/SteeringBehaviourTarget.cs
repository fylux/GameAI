using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SeekT {
    MILLINGTON, REYNOLDS
};

public abstract class SteeringBehaviourTarget : SteeringBehaviour {

    [SerializeField]
    protected SeekT seekT = SeekT.REYNOLDS;

    [SerializeField]
    protected Body target;

}
