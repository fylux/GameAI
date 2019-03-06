﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationManager : MonoBehaviour {
    public FormationPattern pattern;
    List<SlotAssignment> slotAssignments;
    Location driftOffset;

    void Awake() {
        slotAssignments = new List<SlotAssignment>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("A ver si esto funciona");
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("NPC")){
                Debug.Log("Checking...");
                AddCharacter(go.GetComponent<AgentNPC>());
            }
        }
    }

    public void UpdateSlotAssignments() {
        for (int i = 0; i < slotAssignments.Count; i++){
            slotAssignments[i].slotIndex = i;
        }
        driftOffset = pattern.GetDriftOffset(slotAssignments);
    }

    public bool AddCharacter(Agent character)
    {
        Debug.Log("Añadiendo personaje " + character);
        int occupiedSlots = slotAssignments.Count;
        if (!pattern.SupportsSlots(occupiedSlots + 1))
        {
            Debug.Log("Accion no soportada");
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
        Debug.Log("Updating");
        Agent leader = pattern.leader;
        Vector3 anchor = leader.position;
        float orientation = leader.orientation;
        foreach (SlotAssignment sa in slotAssignments)
        {
            Vector3 slotPos = pattern.GetSlotLocation(sa.slotIndex).position;
            Vector3 relPos = anchor + leader.transform.TransformDirection(slotPos);
            Location charDrift = new Location(relPos, orientation);
            charDrift.position += driftOffset.position;
            charDrift.orientation += driftOffset.orientation; //Podria ser *
            Character character = sa.character.GetComponent<Character>();
            character.SetTarget(charDrift);
        }
    }
}
