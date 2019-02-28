using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour {

    [SerializeField]
    public LayerMask unitLayer;

    [SerializeField]
    LayerMask terrainLayer;

    [SerializeField]
    GameObject prefabSelectBox;

    private GameObject unitSelected = null;
    GameObject selectBox = null;
    Vector3 upper_left, lower_right;

    public HashSet<GameObject> selectedUnits = new HashSet<GameObject>();


    void Update() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetButtonDown("Fire1")) {
            Debug.Log(unitLayer.value);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, unitLayer)) {
                unitSelected = hit.transform.gameObject;
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer)) {
                upper_left = hit.point + new Vector3(0, 1f, 0);
                selectBox = Instantiate(prefabSelectBox, upper_left, Quaternion.identity);
                selectBox.GetComponent<SelectBox>().select = this;
            }
        }
        if (Input.GetButton("Fire1") && selectBox != null) {
            Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayer);
            lower_right = hit.point + new Vector3(0, 1f, 0);
            Vector3 z = lower_right - upper_left;
            selectBox.transform.localScale = new Vector3(Mathf.Abs(z.x), 1, Mathf.Abs(z.z)) / 10f;
            selectBox.transform.position = (upper_left + lower_right) / 2f;
        }
        if (Input.GetButtonUp("Fire1")) {
            selectBox.GetComponent<SelectBox>().Finish();
            Destroy(selectBox);
        }
    }
}
