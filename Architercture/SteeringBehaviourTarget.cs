using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SteeringBehaviourTarget : SteeringBehaviour {

    [SerializeField]
    protected Agent target;

    public void Init(Agent target)
    {
        this.target = target;
    }

}
