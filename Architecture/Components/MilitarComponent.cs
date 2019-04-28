﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MilitarComponent : UnitComponent {

    public int maxHealth = 10;
    public int health = 10;
    public int attack = 6;
    public int defense = 3;


    public MilitarComponent() {
        health = maxHealth;
    }

    public void Attack(AgentUnit unit) {
        float damage;
        if (Random.Range(0, 100) > 99f) {
            damage = attack * 5;
        } else {
            damage = attack * Random.Range(0.8f, 1.2f) /** factorTable*/;
        }
        unit.militar.ReceiveAttack(agent, (int)Mathf.Round(damage));
    }

    public float ReceiveAttack(AgentUnit enemy, int amount) {
        int damage = Mathf.Max(0, amount - defense);
        Console.Log("Unit caused " + damage + " damage");
        health -= damage;
        if (IsDead()) {
            Console.Log("Unit died");
            agent.gameObject.SetActive(false);

            //Update list of units removing this one
            GameObject.Destroy(agent.gameObject);
        }
        else if (agent.GetTask() is HostileTask) { //To change the target if needed
            ((HostileTask)agent.GetTask()).ReceiveAttack(enemy);
        }
        //Request to update selection text
        return damage;
    }

    public bool IsDead() {
        return health <= 0;
    }
}
