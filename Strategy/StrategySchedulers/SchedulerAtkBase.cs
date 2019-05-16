using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SchedulerAtkBase : SchedulerStrategy
{


    /* Comprobar si las unidades en territorio rival que tengan la estrategia ATKBASE son mas fuertes que las enemigas a cierto rango(20, por ejemplo).

    Si sí son más fuertes, o como mínimo no son mucho más débiles, entonces habrá que reagruparse + atacar
    Si son mucho más débiles que las enemigas, se les da la orden GoTo agresivo a un área(upFront/downFront) 
    delante de la base enemiga #-> DefenderZona*/
    HashSet<AgentUnit> regr = new HashSet<AgentUnit>(); // Para saber las unidades que se estan reagrupando
    HashSet<AgentUnit> atking = new HashSet<AgentUnit>(); // para saber las que estan atacando
    HashSet<AgentUnit> heal = new HashSet<AgentUnit>();
    // Esos sets son necesarios para no darle la orden GoTo a una unidad que ya la este siguiendo, pero sí hacerlo cuando esa unidad pasa
    // de reagruparse a atacar

    override
    public void ApplyStrategy()
    {
        if (usableUnits.Count > 0)
        {
			HashSet<AgentUnit> alliesAtk = new HashSet<AgentUnit>(Info.GetUnitsFactionArea(Info.GetWaypoint("base", enemyFaction), 45, allyFaction).Where(unit => unit.strategy == StrategyT.ATK_BASE));
            HashSet<AgentUnit> enemiesDef = Info.GetUnitsFactionArea(enemyBase, 25, Util.OppositeFaction(allyFaction));
            alliesAtk.UnionWith(enemiesDef);

            HashSet<AgentUnit> regrouped = new HashSet<AgentUnit>(Info.GetUnitsFactionArea(Info.GetWaypoint("base", enemyFaction), 35, allyFaction).Where(unit => unit.strategy == StrategyT.ATK_BASE));

			HashSet<AgentUnit> regrAtk = new HashSet<AgentUnit> (regrouped);
			regrAtk.UnionWith (enemiesDef);

            bool strong = Info.MilitaryAdvantage(alliesAtk, allyFaction) >= 0.85;
			bool reunitedStrong = Info.MilitaryAdvantage(regrAtk, allyFaction) >= 0.85;

			if ((strong && regrouped.Count >= usableUnits.Count * 0.8) || reunitedStrong)
            {
               // Debug.Log("Somos mas FUERTES asi que vamos a atacar");
                foreach (AgentUnit unit in usableUnits)
                {
                    if (atking.Contains(unit) == false || (!unit.HasTask<GoTo>() && Util.HorizontalDist(unit.position, Info.GetWaypoint("base", enemyFaction)) >= 15)) 
                    {
                        AddGroup(unit, "atking");

                        //   Debug.Log("Dandole a " + unit + " la orden de MOVERSE A LA BASE ENEMIGA");
                        unit.SetTask(new GoTo(unit, Info.GetWaypoint("base", enemyFaction), (bool success) =>
                        {
                          //  Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                            unit.SetTask(new DefendZone(unit, Info.GetWaypoint("base", enemyFaction), 15, (_) => { }));
                        }));
                    }
                }
            }
            else
            {
               // Debug.Log("Somos mas DEBILES asi que vamos a esperar");
                foreach (AgentUnit unit in usableUnits)
                {
                    if (strong == false && unit.militar.health < unit.militar.maxHealth && (heal.Contains(unit) == false || !unit.HasTask<RestoreHealth>()))
                    {
						bool winning = false;

						if (unit.HasTask<Attack>()) {
							Attack task = (Attack)unit.GetTask ();
							AgentUnit targetEnemy = task.GetTargetEnemy ();
							if (targetEnemy.militar.health <= unit.militar.health)
								winning = true;
						}
						if (winning == false) { // Estara a false siempre que no estemos peleando, o estemos peleando pero no llevemos ventaja
							AddGroup(unit, "heal");
							// Debug.Log("Añadadida unidad a heal");
							unit.SetTask(new RestoreHealth(unit, (bool success) => { }));
						}         
                    }
                    else if ((strong == true || unit.militar.health == unit.militar.maxHealth) && (regr.Contains(unit) == false || !unit.HasTask<GoTo>() && Util.HorizontalDist(unit.position, Info.GetWaypoint("front", enemyFaction)) >= 15))
                    {
                        AddGroup(unit, "regr");

                      //  Debug.Log("Dandole a " + unit + " la orden de MOVERSE AL FRONT");
                        unit.SetTask(new GoTo(unit, Info.GetWaypoint("front", enemyFaction), (bool success) =>
                        {
                       //     Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                            unit.SetTask(new DefendZone(unit, Info.GetWaypoint("front", enemyFaction), 15, (_) => { }));
                        }));
                    }
                }
            }
        }
    }

    override
    public void Reset() // Para limpiar las unidades de las listas cada vez que haya un gran cambio
    {
        base.Reset();
        regr.Clear();
        atking.Clear();
    }

    void AddGroup(AgentUnit unit, string group)
    {
        Debug.Assert(group == "heal" || group == "regr" || group == "atking");

        if (group == "heal")
        {
            regr.Remove(unit);
            atking.Remove(unit);
            heal.Add(unit);
        }
        else if (group == "regr")
        {
            regr.Add(unit);
            atking.Remove(unit);
            heal.Remove(unit);
        }
        else
        {
            regr.Remove(unit);
            atking.Add(unit);
            heal.Remove(unit);
        }
    }

	new
	public void RemoveUnit(AgentUnit unit){
		base.RemoveUnit (unit);
		heal.Remove (unit);
		regr.Remove (unit);
		atking.Remove (unit);
	}
}
