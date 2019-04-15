using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAsignDefHalf : OrderAsign {

	override
    public void ApplyStrategy()
    {
        foreach (AgentUnit unit in usableUnits)
        {
            if (unit.militar.health <= unit.militar.MaxLife * 0.3)
            {
                // Comprobar los puntos de curación en un rango (AÑADIRLO A LAS CONDICIONES DEL IF)
                // En caso de haber alguno, comprueba si en algún punto del camino la influencia supera cierto umbral
                // Si NO lo supera, darle la orden GoTo hacia el punto de curación (o alguna orden enfocada a irse, curarse, y volver
                // Si SI lo supera, se ignorará esta orden
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
