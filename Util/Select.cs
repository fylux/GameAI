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

    [SerializeField]
    GameObject prefabSelectCircle;

    GameObject selectBox = null;
    Vector3 upper_left, lower_right;

    public HashSet<AgentUnit> selectedUnits = new HashSet<AgentUnit>();

    [SerializeField]
    Text selectionText;

    //MilitaryResourcesAllocator militaryResourceAllocator = new MilitaryResourcesAllocator();

    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetButtonDown("Fire1")) {
            //militaryResourceAllocator.AllocateResources();

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer)) {
                FinishSelection();
                AddUnit(hit.transform.gameObject.GetComponent<AgentUnit>());
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

        if (Input.GetButtonUp("Fire2") && selectedUnits.Count > 0) {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer)) {
                Console.Log(selectedUnits.Count + " Units going to attack target");
                foreach (AgentUnit unit in selectedUnits) {
                    unit.AttackEnemy(hit.transform.GetComponent<AgentUnit>());
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer)) {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cube.transform.position = hit.point;
                cube.transform.localScale = new Vector3(0.4f, 0.1f, 0.4f);

                Console.Log(selectedUnits.Count + " Units moving to target");
                foreach (AgentUnit unit in selectedUnits) {
                    /*if (unit.faction == Faction.A)*/
                        unit.SetTarget(hit.point);
                }
            }
        }
    }

    public void AddUnit(AgentUnit unit, bool updateText = true) {
        selectedUnits.Add(unit);
        if (unit.SelectCircle == null) {
            unit.SelectCircle = Instantiate(prefabSelectCircle, unit.transform);
            unit.SelectCircle.transform.position = new Vector3(unit.transform.position.x, 0.1f, unit.transform.position.z);
        }

        if (updateText)
            UpdateSelectionText();
    }

    public void RemoveUnit(AgentUnit unit, bool updateText = true) {
        selectedUnits.Remove(unit);

        Destroy(unit.SelectCircle);
        unit.SelectCircle = null;

        if (updateText)
            UpdateSelectionText();
    }

    public void UpdateSelectionText() {
        selectionText.text = "";
        foreach (AgentUnit unit in selectedUnits) {
            string prefix;
            if (unit is Melee)          prefix = "[M]";
            else if (unit is Ranged)    prefix = "[R]";
            else if (unit is Artillery) prefix = "[A]";
            else if (unit is Scout)     prefix = "[S]";
            else                        prefix = "[U]";
            selectionText.text += prefix +" " +unit.name + " " + unit.militar.health + "/" + unit.militar.maxHealth + "\n";
        }
    }

    void FinishSelection() {
        foreach (AgentUnit unit in selectedUnits) {
            Destroy(unit.SelectCircle);
            unit.SelectCircle = null;
        }
        selectedUnits.Clear();
        UpdateSelectionText();
    }
}
