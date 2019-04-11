using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAsignDefBase : OrderAsign {

	override
    public void ApplyStrategy()
    {
        foreach (AgentUnit unit in usableUnits)
        {
            List<Body> healPts;
            if (unit.militar.health <= unit.militar.MaxLife * 0.3 && (healPts = info.GetHealingPoints(Map.NodeFromPosition(unit.position), 20)).Count > 0)
            {
                foreach (Body hp in healPts)
                {
                    Debug.Log("La unidad " + unit + " tiene un healing point cercano: " + hp);
                }
                
                // En caso de haber alguno, comprueba si en algún punto del camino la influencia supera cierto umbral
                // Si NO lo supera, darle la orden GoTo hacia el punto de curación (o alguna orden enfocada a irse, curarse, y volver
                // Si SI lo supera, se ignorará esta orden
            }
            else if (info.AreaMilitaryAdvantage(Map.NodeFromPosition(info.allyBase.position), 25, faction) > 1.2f) // ¿Agrandar el area con varios niveles?
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
