using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEconomy : MonoBehaviour {

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
    int gold, goldPerSecond;


    int modeGen = 0;

	public UnitT unitToGenerate = UnitT.MELEE;

	// Use this for initialization
	void Start () {
		units = new Dictionary<UnitT, GameObject>(){
			{ UnitT.MELEE, melee },
			{ UnitT.RANGED, ranged },
			{ UnitT.SCOUT, scout },
			{ UnitT.ARTIL, artillery } };
	}

	// Update is called once per frame
	void Update () {
		if (Time.frameCount % 30 == 0 && goldGeneration) {
			gold+= goldPerSecond;
		}
		goldDisplay.text = "Gold: [" + gold + "]";
	}

	public void GenerateUnit(){
		if (Map.GetAllies(faction).Count < Map.maxUnits && gold >= 50){
			gold -= 50;
			GameObject created = GameObject.Instantiate(units[unitToGenerate], (Info.GetWaypoint("recruit", faction) + new Vector3(0,0.75f,0)), Quaternion.identity) as GameObject;
			AgentUnit newUnit = created.GetComponent<AgentUnit>();
			Map.unitList.Add (newUnit);
			Debug.Log ("Generada una unidad de " + unitToGenerate);
		}
	}


	public UnitT ChangeGeneration(){
		modeGen++;
		modeGen = modeGen % 4;

		unitToGenerate =(UnitT)modeGen;

		return unitToGenerate;
	}
}
