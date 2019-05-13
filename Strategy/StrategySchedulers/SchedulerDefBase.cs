using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Tal como está este scheduler basta con llamarlo simplemente cada vez que se llame a la capa 2
public class SchedulerDefBase : SchedulerStrategy
{
    private bool InBase(Vector3 position) {
        return Util.HorizontalDist(allyBase, position) < 15;
    }

    override
    public void ApplyStrategy()
    {
        var unitsOutsideBase = usableUnits.Where(unit => !InBase(unit.position) && !unit.HasTask<GoTo>());
        foreach (AgentUnit unit in unitsOutsideBase) {
            Debug.Log("Dandole a " + unit + " la orden de MOVERSE A LA BASE");
			unit.SetTask(new GoTo(unit, allyBase, Mathf.Infinity, 10, false, (bool success) =>
            {
                Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
                unit.SetTask(new DefendZone(unit, allyBase, 15, (_) => { }));
            }));
        }
        var unitsInsideBase = usableUnits.Where(unit => InBase(unit.position) && !unit.HasTask<DefendZone>());
        foreach (AgentUnit unit in unitsInsideBase)
        {
            Debug.Log("Dandole a " + unit + " la orden de DEFENDER LA ZONA");
            unit.SetTask(new DefendZone(unit, allyBase, 15, (_) => { }));
        }
    }

    override
    public void Reset()
    {
    }
}
