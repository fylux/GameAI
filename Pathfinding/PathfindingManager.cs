using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

struct PathRequest {
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Dictionary<NodeT, float> cost;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Dictionary<NodeT, float> cost, Action<Vector3[], bool> callback) {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.cost = cost;
        this.callback = callback;
    }
}

public class PathfindingManager : MonoBehaviour {

    static PathfindingManager instance;
    AStar pathfinding;

    Queue<PathRequest> requestQueue = new Queue<PathRequest>();
    PathRequest currentRequest;

    bool searchingPath;

    void Awake() {
        instance = this;
        pathfinding = GetComponent<AStar>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Dictionary<NodeT, float> cost, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, cost, callback);
        instance.requestQueue.Enqueue(newRequest);
        instance.ProcessNext();
    }

    void ProcessNext() {
        if (!searchingPath && requestQueue.Count > 0) {
            searchingPath = true;
            currentRequest = requestQueue.Dequeue();
            StartCoroutine(pathfinding.FindPath(currentRequest.pathStart, currentRequest.pathEnd, currentRequest.cost));
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        currentRequest.callback(path, success);
        searchingPath = false;
        ProcessNext();
    }


}
