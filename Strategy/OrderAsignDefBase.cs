using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderAsignDefBase : OrderAsign {

    

    override
    public void ApplyStrategy()
    {
        Vector3 allyBase = InfoManager.instance.waypoints["allyBase"].worldPosition;

        foreach (AgentUnit unit in usableUnits)
        {
            if (Util.HorizontalDistance(allyBase, unit.position) > 15) // El 15 es un numero pendiente de ajuste
            {
                if (!(unit.GetTask() is GoTo))
                {
                    Debug.Log("Dandole a " + unit + " la orden de MOVERSE A LA BASE");
                    unit.SetTask(new GoTo(unit, allyBase, (bool success) =>
                    {
                        Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                        unit.SetTask(new DefendZone(unit, allyBase, 15, (_) => { }));
                    }));
                }
            }
            else if (!(unit.GetTask() is DefendZone))
            {
                Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                unit.SetTask(new DefendZone(unit, allyBase, 15, (_) => { }));
            }
        }
    }
}
