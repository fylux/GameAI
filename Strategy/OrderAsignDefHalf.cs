using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAsignDefHalf : OrderAsign {


    
    override
    public void ApplyStrategy()
    {
        foreach (AgentUnit unit in usableUnits)
        {
            Debug.Log("El waypoint del allyBase es " + info.waypoints["allyBase"]); // ¿NOT SET?
            List<Body> healPts;
            if (unit.militar.health <= unit.militar.MaxLife * 0.3 && (healPts = info.GetHealingPoints(Map.NodeFromPosition(unit.position), 60)).Count > 0)
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
            else if (info.AreaMilitaryAdvantage(info.waypoints["allyBase"], 25, faction) > 1.2f) // ¿Agrandar el area con varios niveles?
            {
                // Todas las unidades usables reciben la orden de defender la zona de delante de la base
            }
            else
            {
                // Todas las unidades usables reciben la orden de defender la zona de la base
            }
        }

    }
}
