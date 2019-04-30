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

        HashSet<AgentUnit> alliesAtk = info.GetUnitsFactionArea(enemyBase, 45, faction);
        HashSet<AgentUnit> enemiesDef = info.GetUnitsFactionArea(enemyBase, 25, Util.EnemyFactionOf(faction));
        alliesAtk.UnionWith(enemiesDef);
        if (info.MilitaryAdvantage(alliesAtk,faction) >= 0.85)
        {
            RegroupAndAttack();
        }
        else
        {
            Debug.Log("Somos muy debiles");
        }

    }

    public void RegroupAndAttack()
    {
        Debug.Log("Regroup and attack");
    }
}
