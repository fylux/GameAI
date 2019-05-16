using UnityEngine;

public class SelectBoxCPU : SelectBox {

	override
    protected void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit") && other.GetComponent<AgentUnit>().faction == Faction.A) {
            select.AddUnit(other.GetComponent<AgentUnit>());
        }
    }

	override
	protected void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Unit") && other.GetComponent<AgentUnit>().faction == Faction.A) {
            select.RemoveUnit(other.GetComponent<AgentUnit>());
        }
    }
}
