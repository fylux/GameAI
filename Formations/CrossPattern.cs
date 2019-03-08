using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossPattern : FormationPattern {

    [SerializeField]
    float characterRadius;

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
        float xval = 1;
        float zval = 1;
        if (slotIndex < 2)
            xval = -1;
        if (slotIndex == 1 || slotIndex == 3)
            zval = -1;

        Location location = new Location();

        location.position.x = 2 * characterRadius * xval;
        location.position.z = 2 * characterRadius * zval;

        location.orientation = leader.orientation;

       // Debug.Log(slotIndex + " -> " + location.position); // Devuelve siempre el (1.0, 0.0, 0.0)

        return location;
    }

    public override bool SupportsSlots(int slotCount)
    {
        return slotCount <= numOfSlots;
    }
}
