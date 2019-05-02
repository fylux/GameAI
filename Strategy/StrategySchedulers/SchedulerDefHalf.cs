using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchedulerDefHalf : SchedulerStrategy {
 
    override
    public void ApplyStrategy()
    {
        foreach (AgentUnit unit in usableUnits)
        {
           /* Debug.Log("El waypoint del allyBase es " + Info.GetWaypoint("base",allyFaction)); // ¿NOT SET?
            List<Body> healPts;
            if (unit.militar.health <= unit.militar.maxHealth * 0.3 && (healPts = Info.GetHealingPoints(Map.NodeFromPosition(unit.position), 60)).Count > 0)
            {
                foreach (Body hp in healPts)
                {
                    Debug.Log("La unidad " + unit + " tiene un healing point cercano: " + hp);
                }
                Body closerPoint = Util.GetCloserBody(healPts, Map.NodeFromPosition(unit.position));

                if (closerPoint != null)
                {
                    Debug.Log("Asignada a la unidad " + unit + " la orden GoTo con destino el healPoint" + closerPoint);
                }
            }
            else if (Info.AreaMilitaryAdvantage(Info.GetWaypoint("base", allyFaction), 25, allyFaction) > 1.2f) // ¿Agrandar el area con varios niveles?
            {
                // Todas las unidades usables reciben la orden de defender la zona de delante de la base
            }
            else
            {
                // Todas las unidades usables reciben la orden de defender la zona de la base
            }*/
        }

    }
}
