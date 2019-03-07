using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class FormationPattern: MonoBehaviour {
    public int numOfSlots;
    public Agent leader;


    public abstract Location GetSlotLocation(int slotIndex);

    public abstract bool SupportsSlots(int slotCount);

    public virtual Location GetDriftOffset(List<SlotAssignment> slotAssignments) {
        Location location = new Location();
        location.position = leader.position;
        location.orientation = leader.orientation;
        return location;
    }
}
