using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour {

    public Select select;


    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit")) {
            select.selectedUnits.Add(other.gameObject);
            other.gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }

    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit")) {
            select.selectedUnits.Remove(other.gameObject);
            other.gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    public void Finish() {
        foreach(GameObject unit in select.selectedUnits)
            unit.GetComponent<Renderer>().material.color = Color.red;

        select.selectedUnits.Clear();
    }
}
