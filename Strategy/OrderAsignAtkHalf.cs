using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAsignAtkHalf : OrderAsign {

    private void Start()
    {
        usableUnits = Map.unitList;
    }

    private void Update()
    {
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log("APLICANDO ESTRATEGIA --> ORDENES");
            ApplyStrategy();
        }
    }


    override
    public void ApplyStrategy()
    {
        foreach (AgentUnit unit in usableUnits)
        {
            Debug.Log("El waypoint del allyBase es " + info.waypoints["allyBase"]); // ¿NOT SET?
           /* List<Body> healPts;
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
            }*/
            if (info.AreaMilitaryAdvantage(info.waypoints["allyBase"], 25, faction) > 1.2f) // ¿Agrandar el area con varios niveles?
            {
                // Todas las unidades usables reciben la orden de defender la zona de delante de la base
                Node dest;
                if (faction == Faction.A)
                    dest = info.waypoints["upFront"]; // El cruce de caminos delante de la base
                else
                    dest = info.waypoints["downFront"]; // El mismo cruce pero de la otra base

                Debug.Log("Asignada a la unidad " + unit + " la orden Defender zona con destino Front " + dest);
            }
            else
            {
                Node dest;
                if (faction == Faction.A)
                    dest = info.waypoints["allyBase"];
                else
                    dest = info.waypoints["enemyBase"];

                Debug.Log("Asignada a la unidad " + unit + " la orden Defender zona con destino base " + dest);
                // Todas las unidades usables reciben la orden de defender la zona de la base
            }
        }

    }
}
