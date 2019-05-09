using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	[SerializeField]
	Text generationButton;

	[SerializeField]
	Text minimapButton;

	int modeGen = 0;

	public static UnitT unitToGenerate = UnitT.MELEE;

	public void ChangeGeneration(){
		modeGen++;
		modeGen = modeGen % 4;

		unitToGenerate =(UnitT)modeGen;
		generationButton.text = "NEXT -> " + unitToGenerate;
	}

	public void ChangeMinimap(){
		if (string.Equals (minimapButton.text, "General"))
			minimapButton.text = "Cluster";
		else
			minimapButton.text = "General";

		// Change influence map
	}
}
