using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class MilitarComponent : UnitComponent {

    public int maxHealth;
    public int health;
    public int attack;
    public float attackRange;
    public int defense;
    public float attackSpeed;

    public MilitarComponent() {
        health = maxHealth;
    }

    public void Attack(AgentUnit unit) {
        float damage;
        if (Random.Range(0, 100) > 99f) {
            damage = attack * 5;
        } else {
            damage = attack * Random.Range(0.8f, 1.2f) * AgentUnit.atkTable[(int)agent.GetUnitType(), (int)unit.GetUnitType()];
        }

        if (Map.projectile != null) {
            var newProjectile = GameObject.Instantiate(Map.projectile);
            newProjectile.transform.position = agent.position;
            newProjectile.GetComponent<Projectile>().SetTarget(unit);
        }
        else {
            Debug.Log("Falta el prefab del projectil para atacar");
        }


        unit.militar.ReceiveAttack(agent, (int)Mathf.Round(damage));
    }

    public float ReceiveAttack(AgentUnit enemy, int amount) {
        int damage = Mathf.Max(0, amount - defense);
        Console.Log("Unit caused " + damage + " damage");
        health -= damage;
        agent.StartCoroutine(BlinkMesh());
        if (IsDead()) {
            Console.Log("Unit died");
            //Update list of units removing this one
            agent.StartCoroutine(DestroyUnit());
           
        }
        else if (agent.HasTask<HostileTask>()) { //To change the target if needed
            ((HostileTask)agent.GetTask()).ReceiveAttack(enemy);
        }
        //Request to update selection text
        return damage;
    }

    public bool IsDead() {
        return health <= 0;
    }

    public IEnumerator BlinkMesh() {
        yield return new WaitForSeconds(0.2f);
        agent.SetRenderer(false);

        yield return new WaitForSeconds(0.15f);
        agent.SetRenderer(true);

        yield return new WaitForSeconds(0.15f);
        agent.SetRenderer(false);

        yield return new WaitForSeconds(0.15f);
        agent.SetRenderer(true);
    }

    public IEnumerator DestroyUnit() {
        agent.ResetTask();
        Map.unitList.Remove(agent);
		agent.stratManager.RemoveUnitFromSchedulers(agent);
        
        yield return new WaitForSeconds(0.5f);
        GameObject.DestroyImmediate(agent.gameObject);
    }
}
