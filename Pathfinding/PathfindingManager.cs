﻿using UnityEngine;
using System.Collections.Generic;
using System;

public struct PathRequest {
    public Vector3 start;
    public Vector3 end;
    public Dictionary<NodeT, float> cost;
    public Action<Vector3[], bool> callback;
    public Faction faction;
    public AgentNPC unit;

    public PathRequest(AgentNPC unit, Vector3 end, Faction faction, Action<Vector3[], bool> callback) {
        this.start = unit.position;
        this.end = end;
        this.cost = unit.Cost;
        this.callback = callback;
        this.faction = faction;
        this.unit = unit;
    }
}

public class PathfindingManager : MonoBehaviour {

    static PathfindingManager instance;
    AStar pathfinding;

    public Queue<PathRequest> requestQueue = new Queue<PathRequest>();
    PathRequest request;

    Dictionary<AgentNPC, int> repetitions = new Dictionary<AgentNPC, int>();

    public float init, init2;

    bool searchingPath;

    void Awake() {
        instance = this;
        pathfinding = new AStar();
    }


    public static void RequestPath(AgentNPC agent, Vector3 pathEnd, Faction faction, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(agent, pathEnd,  faction, callback);

        Debug.Assert(Map.NodeFromPosition(pathEnd).isWalkable());
        if (instance.repetitions.ContainsKey(agent)) {
            instance.repetitions[agent] += 1;
        }
        else {
            instance.repetitions[agent] = 1;
        }

        instance.requestQueue.Enqueue(newRequest);
        instance.ProcessNext();

        
    }

    void ProcessNext() {
        while (!searchingPath && requestQueue.Count > 0) {
            searchingPath = true;
            request = requestQueue.Dequeue();

            if (request.unit == null || instance.repetitions[request.unit] > 1) {
                request.callback(null, false);
                searchingPath = false;
                //Debug.Log("Too late pathfinding");
            }
            else {
                instance.init = Time.time;
                StartCoroutine(pathfinding.FindPath(request, FinishedProcessingPath));
            }
            if (request.unit != null) repetitions[request.unit]--;

            init2 = Time.time;
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        request.callback(path, success);
        searchingPath = false;
        ProcessNext();
    }


}