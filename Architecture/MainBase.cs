using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBase : Body {

	public int health = 100;

	StrategyManager strat;

	Faction faction;

	new
	protected void Start(){
		base.Start ();
		strat = GetComponent<StrategyManager>();

		if (strat != null)
			faction = strat.faction;
		else
			faction = Faction.A; // Si no hay un strategyManager es porque es el jugador, por tanto Facción A
	}

	new
	protected void Update(){
		base.Update ();

		if (Time.frameCount % 60 == 0) {
			HashSet<AgentUnit> closeEnemies = Info.UnitsNearBase (faction, Util.OppositeFaction(faction), 15);
			HashSet<AgentUnit> closeAllies = Info.UnitsNearBase (faction, faction, 15);

			int dmg = Mathf.Max(closeEnemies.Count - closeAllies.Count,0);

			health -= dmg;
		}
	}
}
