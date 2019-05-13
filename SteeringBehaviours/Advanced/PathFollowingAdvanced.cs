using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PathFollowingAdvanced : PathFollowing {

    float extraRadius = 0f;
    float timeStamp = 0f;
    bool near = true;

    override
    public Steering GetSteering() {
        if (path == null || currentPoint >= path.Length) {
            if (path == null)
                Debug.LogError("Path is invalid: Null");
            if (currentPoint >= path.Length)
                Debug.LogError("Path is invalid: Out of bounds");
            return new Steering();
        }

        UpdateExtraRadius();
        float arrivalRadius2 = arrivalRadius + extraRadius;

        float distance = Util.HorizontalDist(path[currentPoint], npc.position);
        if (distance < arrivalRadius2) {
            if (currentPoint != Mathf.Min(currentPoint + 1, path.Length - 1)) {
                extraRadius = 0;
                near = false;
            }
            currentPoint = Mathf.Min(currentPoint + 1, path.Length - 1); //When it reaches the last stays on it
            if (currentPoint == path.Length - 2)
                return Arrive.GetSteering(path[currentPoint], npc, 1f, maxAccel /*50*/);
        }

		return Seek.GetSteering(path[currentPoint], npc, maxAccel /*50*/, visibleRays);
    }

    void UpdateExtraRadius() {
        float distance = Util.HorizontalDist(path[currentPoint], npc.position);
        if (Time.fixedTime - timeStamp > 1 && distance < 3f) {
            timeStamp = Time.fixedTime;
            if (near) extraRadius += 0.85f;
            else near = true;
        }
        if (npc.velocity.magnitude < 0.3f) {
            extraRadius = 0;
            near = false;
        }
    }

    void OnDrawGizmos() {
        var models = new HashSet<GameObject>(npc.transform.GetComponentsInChildren<Transform>().Select(t => t.gameObject));
        models.Add(npc.gameObject);
        if (!(models.Any(model => UnityEditor.Selection.Contains(model)) || (npc is AgentUnit && ((AgentUnit)npc).selectCircle != null)))
            return;
        if (!visibleRays || path == null || currentPoint >= path.Length)
            return;

        Gizmos.color = Color.black;
        Debug.DrawLine(npc.position, path[currentPoint] + Vector3.up, Color.yellow);
        for (int i = currentPoint; i < path.Length; ++i) {
            path[i] = new Vector3(path[i].x, 0f, path[i].z);

            if (i + 1 < path.Length)
                Debug.DrawLine(path[i] + Vector3.up, path[i + 1] + Vector3.up, Color.white);

            Gizmos.DrawSphere(path[i] + Vector3.up, 0.23f);
        }

        float arrivalRadius2 = arrivalRadius + extraRadius;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(path[currentPoint] + Vector3.up, arrivalRadius2);
    }

}
