using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EconomyManager : MonoBehaviour {

	[SerializeField]
	StrategyManager stratManager;

	[SerializeField]
	GameObject melee, ranged, scout, artillery;

	Dictionary<UnitT,GameObject> units;

	[SerializeField]
	Faction faction;

	[SerializeField]
	Text goldDisplay;

	[SerializeField]
	bool goldGeneration;

    [SerializeField]
	int gold;

	GenerationManager generationManager;

	Dictionary <UnitT,float> priority = new Dictionary<UnitT, float>() {
		{ UnitT.MELEE, 0},
		{ UnitT.RANGED, 0},
		{ UnitT.SCOUT, 0},
		{ UnitT.ARTIL, 0}};

	// Use this for initialization
	void Start () {
		units = new Dictionary<UnitT, GameObject>(){
			{ UnitT.MELEE, melee },
			{ UnitT.RANGED, ranged },
			{ UnitT.SCOUT, scout },
			{ UnitT.ARTIL, artillery } };

        generationManager = new GenerationManager(stratManager);

    }
	
	// Update is called once per frame
	void Update () {
		if (Time.frameCount % 30 == 0 && goldGeneration) {
			gold+=20;

			if (Map.GetAllies (faction).Count < 20 && gold >= 50) {
				gold -= 50;
				GenerateUnit (generationManager.GetMostImportantUnit());
			}

			goldDisplay.text = "Gold: [" + gold + "]";
		}
	}

	void GenerateUnit(UnitT type){
		GameObject created = GameObject.Instantiate(units[type], (Info.GetWaypoint("base", faction) + new Vector3(0,0.5f,0)), Quaternion.identity) as GameObject;
		AgentUnit newUnit = created.GetComponent<AgentUnit>();
		Map.unitList.Add (newUnit);
		Debug.Log ("Generada una unidad de " + type);
		stratManager.forceStrats = true;
	}
}
