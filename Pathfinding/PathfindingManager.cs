using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

struct PathRequest {
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Action<Vector3[], bool> callback;

    public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        this.pathStart = pathStart;
        this.pathEnd = pathEnd;
        this.callback = callback;
    }
}

public class PathfindingManager : MonoBehaviour {

    static PathfindingManager instance;
    Pathfinding pathfinding;

    Queue<PathRequest> requestQueue = new Queue<PathRequest>();
    PathRequest currentRequest;

    bool searchingPath;

    void Awake() {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.requestQueue.Enqueue(newRequest);
        instance.ProcessNext();
    }

    void ProcessNext() {
        if (!searchingPath && requestQueue.Count > 0) {
            searchingPath = true;
            currentRequest = requestQueue.Dequeue();
            StartCoroutine(pathfinding.FindPath(currentRequest.pathStart, currentRequest.pathEnd));
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        currentRequest.callback(path, success);
        searchingPath = false;
        ProcessNext();
    }


}
