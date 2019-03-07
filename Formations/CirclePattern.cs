using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePattern : FormationPattern {

    [SerializeField]
    float characterRadius;

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
        return centerOfMass;
    }

    public override Location GetSlotLocation(int slotIndex)
    {
        float angleAroundCircle = (float)slotIndex / numOfSlots * Mathf.PI * 2;
    //    Debug.Log("angleAroundCircle -> " + slotIndex + "/" + numOfSlots + " * 2 * PI = " + angleAroundCircle);

        float radius = 3f * (float)characterRadius / (Mathf.Sin((float)Mathf.PI / numOfSlots));
     //   Debug.Log("radius -> " + characterRadius + "/" + "Sin(PI / " + numOfSlots + ") = " + radius);
       // radius = 3.0f;
        Location location = new Location();
        
        location.position.x = radius * Mathf.Cos(angleAroundCircle);
     //   Debug.Log("location.position.x = " + radius + " * " + Mathf.Cos(angleAroundCircle) + " = " + location.position.x);
        location.position.z = radius * Mathf.Sin(angleAroundCircle);

        location.orientation = angleAroundCircle;

       // Debug.Log(slotIndex + " -> " + location.position); // Devuelve siempre el (1.0, 0.0, 0.0)

        return location;
    }

    public override bool SupportsSlots(int slotCount)
    {
        return true;
    }

    /* Esto seria para el cuadrado
    public Location GetSlotLocation(int slotIndex)
    {
        int membersPerRow = (int)Mathf.Ceil(Mathf.Sqrt(numOfSlots));
        int positionInsideRow = slotIndex % membersPerRow;
        int numberOfRow = slotIndex / membersPerRow;

        float separation = characterRadius * 0.75f;

        Location location = new Location();
        //Calcular a partir del lider la posicion de los demás. Seguramente sea operando con senos/cosenos. Si todo
        //falla, cambiar y hacer la del circulo
        
    }*/
}
