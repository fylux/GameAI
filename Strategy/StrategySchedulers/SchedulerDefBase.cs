using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SchedulerDefBase : SchedulerStrategy
{

    private bool InBase(Vector3 position) {
        return Util.HorizontalDist(allyBase, position) < 15;
    }

    override
    public void ApplyStrategy()
    {
        Vector3 r = Info.GetWaypoint("base", allyFaction);
        Vector3[] z = new Vector3[] { r + new Vector3(-4, 0, -4), r + new Vector3(4, 0, -4), r + new Vector3(4, 0, 4), r+ new Vector3(-4, 0, 4) };
        

        var unitsOutsideBase = usableUnits.Where(unit => !InBase(unit.position) && !unit.HasTask<GoTo>());
        foreach (AgentUnit unit in unitsOutsideBase) {
            Debug.Log("Dandole a " + unit + " la orden de MOVERSE A LA BASE");
			unit.SetTask(new GoTo(unit, allyBase, Mathf.Infinity, 10, false, (bool success) => {
                Debug.Log("Dandole a " + unit + " la orden de PATRULLAR");
                //unit.SetTask(new DefendZone(unit, allyBase, 15, (_) => { }));
                unit.SetTask(new Patrol(unit, z, 15f, (_) => { }));
            }));
        }
        var unitsInsideBase = usableUnits.Where(unit => InBase(unit.position) && !unit.HasTask<Patrol>());
        foreach (AgentUnit unit in unitsInsideBase) {
            Debug.Log("Dandole a " + unit + " la orden de PATRULLAR");

            // unit.SetTask(new DefendZone(unit, allyBase, 15, (_) => { }));
            unit.SetTask(new Patrol(unit, z, 15f, (_) => { }));
        }
    }
}
