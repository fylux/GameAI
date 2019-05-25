using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour {

	[SerializeField]
	MainBase downBase, upBase;

	[SerializeField]
	GameObject blue, red;

	bool victory = false;

	void Update () {
		if (downBase.health <= 0 && !victory) {
			red.SetActive (true);
			victory = true;
		}
		else if (upBase.health <= 0 && !victory) {
			blue.SetActive (true);
			victory = true;
		}		
	}
}
