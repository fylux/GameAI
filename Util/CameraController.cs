using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private const int LevelArea = 30;

    private const int ScrollArea = 5;
    private const int ScrollSpeed = 19;

    private const int ZoomSpeed = 40;
    private const int ZoomMin = -25;
    private const int ZoomMax = 25;

    private Vector3 initialPosition;

    void Start() {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        // Init camera translation for this frame.
        var translation = Vector3.zero;

        // Move camera with arrow keys
        translation += new Vector3(-Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));

        // Move camera if mouse pointer reaches screen borders
        if (Input.mousePosition.x < ScrollArea) {
            translation += -Vector3.forward;
        }
        if (Input.mousePosition.x >= Screen.width - ScrollArea) {
            translation += Vector3.forward;
        }
        if (Input.mousePosition.y < ScrollArea) {
            translation += Vector3.right;
        }
        if (Input.mousePosition.y > Screen.height - ScrollArea) {
            translation += -Vector3.right;
        }

        translation *= ScrollSpeed * Time.deltaTime;


        // Zoom in or out
        var zoomDelta = Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.fixedDeltaTime;
        if (zoomDelta != 0) {
            translation -= Vector3.up * ZoomSpeed * zoomDelta;
        }

        // Keep camera within level and zoom area
        var desiredPosition = transform.position + translation - initialPosition;
        if (desiredPosition.x < -LevelArea || LevelArea+10 < desiredPosition.x) {
            translation.x = 0;
        }
        if (desiredPosition.y < ZoomMin || ZoomMax < desiredPosition.y) {
            translation.y = 0;
        }
        if (desiredPosition.z < -LevelArea || LevelArea < desiredPosition.z) {
            translation.z = 0;
        }

        // Finally move camera parallel to world axis
        transform.position += translation;
    }

}
