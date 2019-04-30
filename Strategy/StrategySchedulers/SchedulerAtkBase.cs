using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchedulerAtkBase : SchedulerStrategy
{


    /* Comprobar si las unidades en territorio rival que tengan la estrategia ATKBASE son mas fuertes que las enemigas a cierto rango(20, por ejemplo).

    Si sí son más fuertes, o como mínimo no son mucho más débiles, entonces habrá que reagruparse + atacar
    Si son mucho más débiles que las enemigas, se les da la orden GoTo agresivo a un área(upFront/downFront) 
    delante de la base enemiga #-> DefenderZona*/

    override
    public void ApplyStrategy()
    {
        if (usableUnits.Count > 0)
        {
            HashSet<AgentUnit> alliesAtk = info.GetUnitsFactionArea(enemyBase, 45, faction);
            HashSet<AgentUnit> enemiesDef = info.GetUnitsFactionArea(enemyBase, 25, Util.EnemyFactionOf(faction));
            alliesAtk.UnionWith(enemiesDef);
            if (info.MilitaryAdvantage(alliesAtk, faction) >= 0.85)
            {
                RegroupAndAttack();
            }
            else
            {
                Debug.Log("Vamos a ir avanzando que somos flojitos");
                foreach (AgentUnit unit in usableUnits)
                {
                    if (!(unit.GetTask() is GoTo))
                    {
                        Debug.Log("Dandole a " + unit + " la orden de MOVERSE AL FRONT");
                        unit.SetTask(new GoTo(unit, info.waypoints["enemyFront"].worldPosition, (bool success) =>
                        {
                            Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                            unit.SetTask(new DefendZone(unit, info.waypoints["enemyFront"].worldPosition, 15, (_) => { }));
                        }));
                    }
                }
            }
        }
    }

    public void RegroupAndAttack()
    {
        Debug.Log("Regroup and attack");
    }
}
