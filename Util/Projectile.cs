﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    AgentUnit target;
    float endTime;

    [SerializeField]
    float velocity = 5f;

    public void SetTarget(AgentUnit target) {
        velocity = Mathf.Max(3f, Util.HorizontalDist(transform.position, target.position) / 0.7f);
        this.target = target;

        transform.LookAt(target.position);
        transform.Rotate(new Vector3(90, 180, 0));
        endTime = Time.fixedTime + Util.HorizontalDist(transform.position, target.position) / velocity;
    }

	void Update () {
        transform.LookAt(target.position);
        transform.Rotate(new Vector3(90, 180, 0));

        transform.position += -transform.up * velocity * Time.deltaTime;

        if (Time.fixedTime > endTime) {
            GameObject.Destroy(gameObject);
        }
	}

}
