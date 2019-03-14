using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationManager : MonoBehaviour {
    public FormationPattern pattern;
    List<SlotAssignment> slotAssignments;
    Location driftOffset;

//    Vector3 lastLeaderPosition;

    void Awake() {
        slotAssignments = new List<SlotAssignment>();
     //   lastLeaderPosition = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("NPC")){
                AddCharacter(go.GetComponent<AgentUnit>());
            }
        }

        if (slotAssignments.Count != 0)
            UpdateSlots();
    }

    public void UpdateSlotAssignments() {
        for (int i = 0; i < slotAssignments.Count; i++){
            slotAssignments[i].slotIndex = i;
        }
        driftOffset = pattern.GetDriftOffset(slotAssignments);
    }

    public bool AddCharacter(AgentUnit character)
    {
        int occupiedSlots = slotAssignments.Count;
        if (!pattern.SupportsSlots(occupiedSlots + 1) || character == pattern.leader)
        {
            return false;
        }
        SlotAssignment sa = new SlotAssignment();
        sa.character = character;
        slotAssignments.Add(sa);
        UpdateSlotAssignments();
        return true;
    }

    public void RemoveCharacter(Agent agent)
    {
        int index = slotAssignments.FindIndex(x => x.character.Equals(agent));
        slotAssignments.RemoveAt(index);
        UpdateSlotAssignments();
    }

    public void UpdateSlots()
    {
        Agent leader = pattern.leader;
        Vector3 anchor = leader.position;
        float orientation = leader.orientation;
        foreach (SlotAssignment sa in slotAssignments)
        {
            AgentUnit character = sa.character.GetComponent<AgentUnit>();

            if (character != leader) // Tiene sentido que el lider no se mueva
            {
                Vector3 slotPos = pattern.GetSlotLocation(sa.slotIndex).position;
                Vector3 relPos = anchor + leader.transform.TransformDirection(slotPos);
                //    Debug.Log(sa.slotIndex + ". SlotPos: " + slotPos + ", relPos: " + relPos);
                   Location charDrift = new Location(relPos, orientation);
                //   charDrift.position += driftOffset.position;
                Debug.Log("driftOffset.position = " + driftOffset.position);
                  // charDrift.orientation += driftOffset.orientation; //Podria ser *

                   character.SetFormation(charDrift.position, orientation); //Queremos que miren a donde mire el lider
            }
        }
    }
}
