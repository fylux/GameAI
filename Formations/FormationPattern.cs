using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FormationPattern: MonoBehaviour {
    public int numOfSlots;
    public Agent leader;


    public virtual Location GetSlotLocation(int slotIndex) {
        return new Location();
    }

    public bool SupportsSlots(int slotCount) {
        return slotCount <= numOfSlots;
    }

    public virtual Location GetDriftOffset(List<SlotAssignment> slotAssignments) {
        Location location = new Location();
        location.position = leader.position;
        location.orientation = leader.orientation;
        return location;
    }
}
