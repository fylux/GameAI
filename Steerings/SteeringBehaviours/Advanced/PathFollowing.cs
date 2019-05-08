using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum FollowT {
    STAY, BACK, LOOP
}

public class PathFollowing : SteeringBehaviour {

    [SerializeField]
    float arrivalRadius = 0.4f;

    public Vector3[] path;
    int currentPoint;
    int direction = 1;
    FollowT type = FollowT.STAY;

    new
    void Start() {
        base.Start();
        currentPoint = 0;
    }

    public void SetPath(Vector3[] path) {
        this.path = path;
        currentPoint = 0;
    }

    override
    public Steering GetSteering() {
        if (path == null || currentPoint >= path.Length) {
            if (path == null)
                Debug.LogError("Path is invalid: Null");
            if (currentPoint >= path.Length)
                Debug.LogError("Path is invalid: Out of bounds");
            return new Steering();
        }
        float occupied = Physics.Raycast(path[currentPoint], path[currentPoint] + Vector3.up, 2f, Map.unitsMask) ? 1f : 0;
        float arrivalRadius2 = arrivalRadius + occupied;
        if (occupied > 0f) {
            Debug.Log("ocuppied");
        }

        float distance = Util.HorizontalDist(path[currentPoint], npc.position);
        if (distance < arrivalRadius2) {
            if (type == FollowT.STAY) {
                currentPoint = Mathf.Min(currentPoint + 1, path.Length - 1); //When it reaches the last stays on it
                if (currentPoint == path.Length - 1)
                    return Arrive.GetSteering(path[currentPoint], npc, 1f,maxAccel /*50*/);
            }
            else if (type == FollowT.BACK) {
                currentPoint += direction;
                //Needs to be run twice, since the currentPoint will remain the same the first time
                if (currentPoint >= path.Length || currentPoint < 0) {
                    direction = 1 - direction;
                    currentPoint += direction;
                }
            }
            else if (type == FollowT.LOOP) {
                //When it reaches the last it starts from the begging
                currentPoint++;
                if (currentPoint >= path.Length)
                    currentPoint = 0;
            }
        }

		return Seek.GetSteering(path[currentPoint], npc, maxAccel /*50*/, visibleRays);
    }

    /*public static Steering getSteering(Vector3[] path, ref int currentPoint, float arrivalRadius, Agent npc, float maxAccel, bool visibleRays) {
        float distance = Util.HorizontalDistance(path[currentPoint], npc.position);
        if (distance < arrivalRadius) {
            currentPoint = Mathf.Min(currentPoint + 1, path.Length - 1); //When it reaches the last stays on it 
        }

        return Seek.GetSteering(path[currentPoint], npc, maxAccel, visibleRays);
    }*/

    void OnDrawGizmos() {
        var models = new HashSet<GameObject>(npc.transform.GetComponentsInChildren<Transform>().Select(t => t.gameObject));
        models.Add(npc.gameObject);
        if (!(models.Any(model => UnityEditor.Selection.Contains(model)) || (npc is AgentUnit && ((AgentUnit)npc).selectCircle != null)))
            return;
        if (!visibleRays || path == null || currentPoint >= path.Length )
            return;
        Gizmos.color = Color.black;
        Debug.DrawLine(npc.position, path[currentPoint] + Vector3.up, Color.yellow);
        for (int i = currentPoint; i < path.Length; ++i) {

            path[i] = new Vector3(path[i].x, 0f, path[i].z);

            if (i + 1 < path.Length)
                Debug.DrawLine(path[i] + Vector3.up, path[i+1] + Vector3.up, Color.white);

            Gizmos.DrawSphere(path[i] + Vector3.up, 0.23f);
        }
    }

}
