using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour {

    public Select select;


    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit") && other.GetComponent<AgentUnit>().faction == Faction.A) {
            select.AddUnit(other.GetComponent<AgentUnit>());
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit") && other.GetComponent<AgentUnit>().faction == Faction.A) {
            select.RemoveUnit(other.GetComponent<AgentUnit>());
        }
    }
}
