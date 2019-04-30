using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchedulerDefBase : SchedulerStrategy
{

    

    override
    public void ApplyStrategy()
    {

        foreach (AgentUnit unit in usableUnits)
        {
            if (Util.HorizontalDistance(allyBase.worldPosition, unit.position) > 15) // El 15 es un numero pendiente de ajuste
            {
                if (!(unit.GetTask() is GoTo))
                {
                    Debug.Log("Dandole a " + unit + " la orden de MOVERSE A LA BASE");
                    unit.SetTask(new GoTo(unit, allyBase.worldPosition, (bool success) =>
                    {
                        Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                        unit.SetTask(new DefendZone(unit, allyBase.worldPosition, 15, (_) => { }));
                    }));
                }
            }
            else if (!(unit.GetTask() is DefendZone))
            {
                Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                unit.SetTask(new DefendZone(unit, allyBase.worldPosition, 15, (_) => { }));
            }
        }
    }
}
