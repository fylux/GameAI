﻿using System.Collections.Generic;
using UnityEngine;

public enum StrategyT {
	DEF_BASE, DEF_HALF, ATK_BASE, ATK_HALF
};

public class StrategyLayer {

	bool dbg = false;

	public Dictionary<StrategyT, float> priority = new Dictionary<StrategyT, float>() { { StrategyT.DEF_BASE, 0 },
		{ StrategyT.DEF_HALF, 0 },
		{ StrategyT.ATK_HALF, 0 },
		{ StrategyT.ATK_BASE, 0 } };

	Dictionary<string, List<Node>> waypointArea;


	static public string chosenWaypoint = "mid";
	Faction allyFaction, enemyFaction;

	public StrategyLayer(Faction allyFaction) {
		this.allyFaction = allyFaction;
		this.enemyFaction = Util.OppositeFaction(allyFaction);
		// Dado que, más adelante, tendremos que consultar la influencia en el área alrededor de estos waypoints a cada segundo, vamos a tener guardados
		// los nodos y así nos ahorramos repetir el trabajo
		waypointArea = new Dictionary<string, List<Node>>() { { "mid", Info.GetNodesInArea(Info.GetWaypoint("mid", allyFaction), 5) },
			{ "top", Info.GetNodesInArea(Info.GetWaypoint("top", allyFaction), 5) },
			{ "bottom", Info.GetNodesInArea(Info.GetWaypoint("mid", allyFaction), 5) }};
	}

	public bool Apply() {
		if (dbg) Debug.Log("Starting apply");
		bool changed = false;

		Dictionary<StrategyT, float> newPriority = ComputePriority();

		foreach (StrategyT strategy in newPriority.Keys) {
			if (dbg) Debug.Log("El valor de la estrategia " + strategy + " es de " + newPriority[strategy]);
			//	Debug.Log ("Antes, el valor de la estrategia era de " + priority [strategy]);
			if (Mathf.Abs(newPriority[strategy] - priority[strategy]) >= 0.15) //TODO: Decidir valor real. ¿Lo hacemos así o pedimos cambio estable?
			{
				priority = newPriority;
				changed = true;
			}
		}
		return changed;
	}

	public Dictionary<StrategyT, float> GetPriority() {
		return priority;
	}

	Dictionary<StrategyT, float> ComputePriority() {
		float defbase = Mathf.Clamp(PriorityDefbase(), 0, 1);
		float defhalf = Mathf.Clamp(PriorityDefhalf(), 0, 1);
		float atkhalf = 1 - (Mathf.Max(defbase, defhalf));

		// *********** Bonus de controlar territorio aliado **********
		float terr = Info.GetTerritoryInfluence(allyFaction, enemyFaction);
		if (!(terr >= 0.001)) {
			atkhalf += 0.2f;
//			Debug.Log ("BONUS por dominación de territorio!");
			atkhalf = Mathf.Min (1, atkhalf);
		} else {
			defhalf += 0.2f;
			defhalf = Mathf.Min (1, defhalf);
		}
			
//		Debug.Log("La influencia enemiga en el territorio aliado es de " + terr);
		// ***********************************************************


		return new Dictionary<StrategyT, float>(){
			{ StrategyT.DEF_BASE, defbase },
			{ StrategyT.DEF_HALF, defhalf },
			{ StrategyT.ATK_HALF, atkhalf },
			{ StrategyT.ATK_BASE, Mathf.Clamp(PriorityAtkbase(), 0, 1) }
		};
	}

	float PriorityDefbase() {

		if (dbg) Debug.Log("START DEFBASE");
		HashSet<AgentUnit> baseAllies = Info.UnitsNearBase(allyFaction, allyFaction, 25); //Cogemos los aliados cercanos a la base aliada
		baseAllies.UnionWith(Info.GetUnitsFactionArea(Info.GetWaypoint("base", allyFaction), 45, enemyFaction)); // Añadimos los enemigos en territorio aliado

		float result = Mathf.Clamp(Info.MilitaryAdvantage(baseAllies, enemyFaction) - 1, -0.4f, 0.4f);
		if (dbg) Debug.Log("Gracias a la ventaja de las fuerzas aliadas en la base aliada frente a las enemigas en territorio aliado, tenemos un peso actual de " + result);

		HashSet<AgentUnit> units = Map.unitList;
		result += Mathf.Clamp(Info.MilitaryAdvantage(units, enemyFaction) - 1, -0.2f, 0.2f);
		if (dbg) Debug.Log("Teniendo en cuenta todas las unidades vivas, ese peso es ahora " + result);

		HashSet<AgentUnit> unitsInOtherHalf = Info.UnitsNearBase(enemyFaction, allyFaction, 40); //Unidades de un bando en territorio del otro
		unitsInOtherHalf.UnionWith(Info.UnitsNearBase(allyFaction, enemyFaction, 40));
		float allyAdv = Info.MilitaryAdvantageArea(Info.GetWaypoint("base", enemyFaction), 40, allyFaction);
		float enemAdv = Info.MilitaryAdvantageArea(Info.GetWaypoint("base", allyFaction), 40, enemyFaction);

		result -= Mathf.Clamp(allyAdv - enemAdv, -0.4f, 0.4f);
		if (dbg) Debug.Log("Finalmente, comparando la ventaja atacante aliada con la enemiga, el peso de DEFBASE es de " + result);

		return result;
	}

	float PriorityDefhalf() {
		if (dbg) Debug.Log("START DEFHALF");
		HashSet<AgentUnit> near = Info.UnitsNearBase(allyFaction, enemyFaction, 25);
		HashSet<AgentUnit> mid = Info.UnitsNearBase(allyFaction, enemyFaction, 35);
		HashSet<AgentUnit> far = Info.UnitsNearBase(allyFaction, enemyFaction, 50);

		far.ExceptWith(mid);
		far.ExceptWith(near);

		mid.ExceptWith(near);

		float result = 0;

		foreach (AgentUnit unit in near) {
			result += ((float)1 / 5); 
		}
		foreach (AgentUnit unit in mid) {
			result += ((float)1 / 5) * 0.85f;
		}
		foreach (AgentUnit unit in far) {
			result += ((float)1 / 5) * 0.75f;
		}

		if (dbg) Debug.Log("La proximidad de unidades enemigas en territorio aliado contribuye a DEFHALF en " + result);

		if (dbg) Debug.Log("El peso dado a DEFHALF es de " + result);

		return result;

	}

	float PriorityUnitsNearWaypoint(string waypoint, float nearMult, float farMult) {
		HashSet<AgentUnit> near = Info.GetUnitsFactionArea(Info.waypoints[waypoint], 8, enemyFaction);
		HashSet<AgentUnit> far = Info.GetUnitsFactionArea(Info.waypoints[waypoint], 15, enemyFaction);

		far.ExceptWith(near);
		float result = (nearMult / Map.maxUnits) * near.Count + (farMult / Map.maxUnits) * far.Count;
		if (dbg) Debug.Log("El waypoint " + waypoint + " contribuye a DEFHALF en " + result + " debido a la cercanía de unidades enemigas");
		return result;
	}

	float PriorityAllyInfluenceWaypoint(string waypoint) {
		if (dbg) Debug.Log("Vamos a buscar el waypoint" + waypoint +" de "+allyFaction);
		float allyInfl = Info.GetNodesInfluence(allyFaction, waypointArea[waypoint]);
		float enemyInfl = Info.GetNodesInfluence(enemyFaction, waypointArea[waypoint]);

		if (dbg) Debug.Log("En el waypoint " + waypoint + " hay una influencia aliada de " + allyInfl + " y una influencia enemiga de " + enemyInfl);
		if (dbg) Debug.Log("Por tanto el waypoint " + waypoint + " contribuye al peso debido a influencias enemigas en -" + Mathf.Min(0.2f, Mathf.Max((enemyInfl - allyInfl), 0)));

		return Mathf.Min(0.2f, Mathf.Max((enemyInfl - allyInfl), 0)); // Preferimos que la influencia en nuestro lado sea nuestra
	}

	float PriorityAtkhalf() {
		// Desde el centro de la base, un radio de 50 cubre todo el territorio relevante
		// 40 sería un "casi llegando a la base"
		// 25 sería que están en la misma base
		if (dbg) Debug.Log("START ATKHALF");
		HashSet<AgentUnit> near = Info.UnitsNearBase(enemyFaction, enemyFaction, 25);
		HashSet<AgentUnit> mid = Info.UnitsNearBase(enemyFaction, enemyFaction, 40);
		HashSet<AgentUnit> far = Info.UnitsNearBase(enemyFaction, enemyFaction, 50);

		far.ExceptWith(mid);
		far.ExceptWith(near);

		mid.ExceptWith(near);

		float result = 0;

		foreach (AgentUnit unit in near) {
			result += ((float)1 / Map.maxUnits); // ¿Deberiamos considerar el "peor caso" antes, o no tiene sentido?
		}
		foreach (AgentUnit unit in mid) {
			result += ((float)1 / Map.maxUnits) * 0.75f;
		}
		foreach (AgentUnit unit in far) {
			result += ((float)1 / Map.maxUnits) * 0.5f;
		}

		if (dbg) Debug.Log("La proximidad de unidades enemigas en su territorio contribuye a ATKHALF en " + result);

		// Para calcular la diferencia de fuerza en los waypoints, mismo sistema que en DEFHALF
		// float area = 20;
		float inflM = Mathf.Clamp(Info.MilitaryAdvantageArea(Info.GetWaypoint("base", enemyFaction), 45, allyFaction) - 1, -0.5f, 0.5f);
		result += inflM;

		float terr = Info.GetTerritoryInfluence(allyFaction, enemyFaction);
		if (!(terr >= 0))
		{
			result += 0.2f;
			Debug.Log("BONUS por dominación de territorio!");
		}
		else
			Debug.Log("La influencia enemiga en el territorio aliado es de " + terr);


		if (dbg) Debug.Log("El peso dado a ATKHALF es de " + result);

		return result;
	}

	float PriorityAtkbase() {
		if (dbg) Debug.Log("START ATKBASE");
		HashSet<AgentUnit> baseEnemies = Info.UnitsNearBase(enemyFaction, enemyFaction, 25); //Cogemos los enemigos cercanos a la base enemiga
		baseEnemies.UnionWith(Info.GetUnitsFactionArea(Info.GetWaypoint("base", enemyFaction), 45, allyFaction)); // Añadimos los aliados en territorio enemigo

		float result = Mathf.Clamp(Info.MilitaryAdvantage(baseEnemies, allyFaction) - 1, -0.4f, 0.4f);
		if (dbg) Debug.Log("Gracias a la ventaja de las fuerzas aliadas en territorio enemigo frente a las enemigas en la base enemigo, tenemos un peso actual de " + result);

		/*HashSet<AgentUnit> units = new HashSet<AgentUnit>(Map.GetAllies(allyFaction));
        units.UnionWith(Map.GetEnemies(allyFaction)); // Tenemos ahora un hashset con todas las unidades vivas*/
		HashSet<AgentUnit> units = Map.unitList;
		result += Mathf.Clamp(Info.MilitaryAdvantage(units, allyFaction) - 1, -0.2f, 0.2f);
		if (dbg) Debug.Log("Teniendo en cuenta todas las unidades vivas, ese peso es ahora " + result);

		HashSet<AgentUnit> unitsInOtherHalf = Info.UnitsNearBase(enemyFaction, allyFaction, 40); //Unidades de un bando en territorio del otro
		unitsInOtherHalf.UnionWith(Info.UnitsNearBase(allyFaction, enemyFaction, 40));
		float allyAdv = Info.MilitaryAdvantageArea(Info.GetWaypoint("base", enemyFaction), 40, allyFaction);
		float enemAdv = Info.MilitaryAdvantageArea(Info.GetWaypoint("base", allyFaction), 40, enemyFaction);

		result += Mathf.Clamp(allyAdv - enemAdv, -0.4f, 0.4f);
		if (dbg) Debug.Log("Finalmente, comparandola ventaja atacante aliada con la enemiga, el peso de ATKBASE es de " + result);

		return result;

	}
}
