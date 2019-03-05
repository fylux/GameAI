using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour {

    public Select select;


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit")) {
            select.AddUnit(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit")) {
            select.RemoveUnit(other.gameObject);
        }
    }
}
