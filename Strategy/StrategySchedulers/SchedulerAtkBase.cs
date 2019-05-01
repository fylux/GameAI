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
    // Esos sets son necesarios para no darle la orden GoTo a una unidad que ya la este siguiendo, pero sí hacerlo cuando esa unidad pasa
    // de reagruparse a atacar

    override
    public void ApplyStrategy()
    {
        if (usableUnits.Count > 0)
        {
            HashSet<AgentUnit> alliesAtk = Info.GetUnitsFactionArea(enemyBase, 45, allyFaction);
            HashSet<AgentUnit> enemiesDef = Info.GetUnitsFactionArea(enemyBase, 25, Util.OppositeFaction(allyFaction));
            alliesAtk.UnionWith(enemiesDef);

            HashSet<AgentUnit> regrouped = new HashSet<AgentUnit>(Info.GetUnitsFactionArea(Info.GetWaypoint("front", enemyFaction), 35, allyFaction).Where(unit => unit.strategy == StrategyT.ATK_BASE));

            if (Info.MilitaryAdvantage(alliesAtk, allyFaction) >= 0.85 && regrouped.Count >= usableUnits.Count * 0.8)
            {
                Debug.Log("Somos mas FUERTES asi que vamos a atacar");
                foreach (AgentUnit unit in usableUnits)
                {
                    if (atking.Contains(unit) == false || (!(unit.GetTask() is GoTo) && Util.HorizontalDist(unit.position, Info.GetWaypoint("base", enemyFaction).worldPosition) >= 15)) 
                    {
                        atking.Add(unit);
                        if (regr.Contains(unit))
                            regr.Remove(unit);

                        //   Debug.Log("Dandole a " + unit + " la orden de MOVERSE A LA BASE ENEMIGA");
                        unit.SetTask(new GoTo(unit, Info.GetWaypoint("base", enemyFaction).worldPosition, (bool success) =>
                        {
                          //  Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                            unit.SetTask(new DefendZone(unit, Info.GetWaypoint("base", enemyFaction).worldPosition, 15, (_) => { }));
                        }));
                    }
                }
            }
            else
            {
                Debug.Log("Somos mas DEBILES asi que vamos a esperar");
                foreach (AgentUnit unit in usableUnits)
                {
                    if (regr.Contains(unit) == false || !(unit.GetTask() is GoTo) && Util.HorizontalDist(unit.position, Info.GetWaypoint("base", enemyFaction).worldPosition) >= 15)
                    {
                        regr.Add(unit);
                        if (atking.Contains(unit))
                            atking.Remove(unit);

                      //  Debug.Log("Dandole a " + unit + " la orden de MOVERSE AL FRONT");
                        unit.SetTask(new GoTo(unit, Info.GetWaypoint("front", enemyFaction).worldPosition, (bool success) =>
                        {
                       //     Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                            unit.SetTask(new DefendZone(unit, Info.GetWaypoint("front", enemyFaction).worldPosition, 15, (_) => { }));
                        }));
                    }
                }
            }
        }
    }

    override
    public void Reset() // Para limpiar las unidades de las listas cada vez que haya un gran cambio
    {
        regr.Clear();
        atking.Clear();
    }
}
