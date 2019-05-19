using UnityEngine;
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

    public string currentUnitName;

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
                Debug.Log("Too late pathfinding");
            }
            else {
                instance.init = Time.time;
                request.unit.SetColor(Color.black);
                currentUnitName = request.unit.name;
                request.unit.transform.localScale = new Vector3(3f, 3f, 3f);

                Debug.Log(Time.frameCount + " " + request.unit.name);
                StartCoroutine(pathfinding.FindPath(request.start, request.end, request.cost, request.faction, FinishedProcessingPath));
            }
            if (request.unit != null) repetitions[request.unit]--;

            init2 = Time.time;
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success) {
        request.unit.transform.localScale = new Vector3(1f, 1f, 1f);
        request.unit.SetColor(Color.red);
        currentUnitName = "";

        request.callback(path, success);
        searchingPath = false;
        ProcessNext();
    }


}