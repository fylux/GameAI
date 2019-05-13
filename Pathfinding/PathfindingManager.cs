using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

struct PathRequest {
    public Vector3 start;
    public Vector3 end;
    public Dictionary<NodeT, float> cost;
    public Action<Vector3[], bool> callback;
    public Faction faction;

    public PathRequest(Vector3 start, Vector3 end, Dictionary<NodeT,float> cost, Faction faction, Action<Vector3[], bool> callback) {
        this.start = start;
        this.end = end;
        this.cost = cost;
        this.callback = callback;
        this.faction = faction;
    }
}

public class PathfindingManager : MonoBehaviour {

    static PathfindingManager instance;
    AStar pathfinding;

    Queue<PathRequest> requestQueue = new Queue<PathRequest>();
    PathRequest request;

    bool searchingPath;

    void Awake() {
        instance = this;
        pathfinding = new AStar();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Dictionary<NodeT, float> cost, Faction faction, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, cost,  faction, callback);
        instance.requestQueue.Enqueue(newRequest);
        instance.ProcessNext();
    }

    void ProcessNext() {
        if (!searchingPath && requestQueue.Count > 0) {
            searchingPath = true;
            request = requestQueue.Dequeue();
            StartCoroutine(pathfinding.FindPath(request.start, request.end, request.cost,  request.faction, FinishedProcessingPath));
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        request.callback(path, success);
        searchingPath = false;
        ProcessNext();
    }


}
