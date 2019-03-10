using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquarePattern : FormationPattern {

    [SerializeField]
    int columns = 3;

    private void Awake()
    {
        numOfSlots = 4;
    }

    public int CalculateNumberOfSlots(SlotAssignment[] assignments)
    {
        int filledSlots = 0;

        foreach (SlotAssignment sa in assignments)
        {
            if (sa.slotIndex > filledSlots)
                filledSlots = sa.slotIndex;
        }

        return filledSlots + 1;
    }

    public override Location GetDriftOffset(List<SlotAssignment> assignments)
    {
        Location centerOfMass = new Location();

        foreach (SlotAssignment sa in assignments)
        {
            Location location = GetSlotLocation(sa.slotIndex);
            centerOfMass.position += location.position;
            centerOfMass.orientation += location.orientation;
        }

        int numberOfAssignments = assignments.Count;
        centerOfMass.position /= numberOfAssignments;
        centerOfMass.orientation /= numberOfAssignments;
        Debug.Log("Centro de masas: " + centerOfMass.position);
        return centerOfMass;
    }

    public override Location GetSlotLocation(int slotIndex)
    {
        slotIndex++;

        Location location = new Location();

        location.position.x = (slotIndex % columns) * characterRadius * 2;
        location.position.z = (slotIndex / columns) * characterRadius * -2;

        location.orientation = leader.orientation;

        return location;
    }

    public override bool SupportsSlots(int slotCount)
    {
        return true;
    }
}
