using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour {
    
    [SerializeField]
    private LayerMask layerMask;

    private GameObject unitSelected = null;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1")) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                unitSelected = hit.transform.gameObject;

            if (unitSelected != null)
                Debug.Log(unitSelected.name);
        }
    }
}
