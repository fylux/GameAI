﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPoint : Body {

    HashSet<AgentUnit> units;

    private float nextHealingTime = 0.0f;
    public float period = 0.1f;

    new
    void Start () {
        base.Start();
        units = new HashSet<AgentUnit>();
    }

    new
    void Update() {
        base.Update();
        if (Time.time > nextHealingTime) {
            nextHealingTime += period;

            units.IntersectWith(Map.unitList); //Remove units that may died
            foreach (AgentUnit unit in units) {
                if (unit.militar.health < unit.militar.maxHealth) {
                    unit.militar.health += 1;
                    Console.Log("Unit " + unit.name + " restored health");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        AgentUnit unit = other.GetComponent<AgentUnit>();
        if (unit != null) {
            units.Add(unit);
        }
    }

    private void OnTriggerExit(Collider other) {
        AgentUnit unit = other.GetComponent<AgentUnit>();
        if (unit != null) {
            units.Remove(unit);
        }
    }
}
