using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Select : MonoBehaviour {

    [SerializeField]
    public LayerMask unitLayer;

    [SerializeField]
    LayerMask terrainLayer;

    [SerializeField]
    GameObject prefabSelectBox;

    GameObject selectBox = null;
    Vector3 upper_left, lower_right;

    public HashSet<GameObject> selectedUnits = new HashSet<GameObject>();

    [SerializeField]
    Text selectionText;


    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetButtonDown("Fire1")) {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer)) {
                FinishSelection();
                AddUnit(hit.transform.gameObject);
                selectedUnit = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer)) {
                FinishSelection();

                upper_left = hit.point + new Vector3(0, 1f, 0);
                selectBox = Instantiate(prefabSelectBox, upper_left, Quaternion.identity);
                selectBox.GetComponent<SelectBox>().select = this;
            }
        }

        //Resize selectBox
        if (Input.GetButton("Fire1") && selectBox != null) {
            Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer);
            lower_right = hit.point + new Vector3(0, 1f, 0);
            Vector3 z = lower_right - upper_left;
            selectBox.transform.localScale = new Vector3(Mathf.Abs(z.x), 1, Mathf.Abs(z.z)) / 10f;
            selectBox.transform.position = (upper_left + lower_right) / 2f;
        }

        //Finish SelectBox
        if (Input.GetButtonUp("Fire1") && selectBox != null) {
            Destroy(selectBox);
        }

        /*if (Input.GetButtonUp("Fire2") && selectedUnits.Count == 1) {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer)) {
                selectedUnit.GetComponent<AgentNPC>().SetTarget(hit.point);
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cube.transform.position = hit.point;
                cube.transform.localScale = new Vector3(0.4f, 0.1f, 0.4f);
            }
        }*/

        if (Input.GetButtonUp("Fire2") && selectedUnits.Count > 0) {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer)) {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cube.transform.position = hit.point;
                cube.transform.localScale = new Vector3(0.4f, 0.1f, 0.4f);

                foreach (GameObject unit in selectedUnits) {
                    unit.GetComponent<AgentUnit>().SetTarget(hit.point);
                }
                
            }
        }
    }

    public void AddUnit(GameObject unit) {
        selectedUnits.Add(unit);
        unit.GetComponent<Renderer>().material.color = Color.blue;

        UpdateSelectionText();
    }

    public void RemoveUnit(GameObject unit) {
        selectedUnits.Remove(unit);
        unit.GetComponent<Renderer>().material.color = Color.red;

        UpdateSelectionText();
    }

    public void UpdateSelectionText() {
        selectionText.text = "";
        foreach (GameObject unit in selectedUnits) {
            selectionText.text += unit.name + "\n";
        }
    }

    void FinishSelection() {
        foreach (GameObject unit in selectedUnits)
            unit.GetComponent<Renderer>().material.color = Color.red;

        selectedUnits.Clear();
        UpdateSelectionText();
    }
}
