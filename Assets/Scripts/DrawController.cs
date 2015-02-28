using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DrawController : MonoBehaviour {

    public const int DRAWING_TOOL = 0;
    public const int MOVING_TOOL = 1;
    public const int ROTATING_TOOL = 2;

    public GameObject toDraw;
    public Museum currentMuseum;
    public float cameraSpeed = 10;
    public float edgeRatio = 0.05f;
    public int tool = 0;

    private bool dragging = false;
    private Vector3 dragPoint = Vector3.zero;
    private Vector3 dragAnchor = Vector3.zero;

	void Start () {
	
	}

    public void SetTool(int tool) {
        this.tool = tool;
    }
	
    void Update() {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        switch (tool) {
            case DRAWING_TOOL:
                DrawUpdate();
                break;
            case MOVING_TOOL:
                MoveUpdate();
                break;
            case ROTATING_TOOL:
                RotateUpdate();
                break;
        }
	}

    void DrawUpdate() {
        var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
        var dir = Camera.main.transform.forward;
        var origin = mouse3D;
        Debug.DrawRay(origin, dir * 100, Color.white, 0.1f);
        if (Input.GetMouseButton(0)) {
            Debug.Log("Press " + origin + " " + dir);
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info, 1 << 8)) {
                var hitPosition = new Vector3(Mathf.Floor(info.point.x + 0.5f), 0.5f, Mathf.Floor(info.point.z + 0.5f));
                Debug.Log(hitPosition + " " + info.point);
                currentMuseum.SetTile((int)Mathf.Floor(info.point.x + 0.5f), 0, (int)Mathf.Floor(info.point.z + 0.5f), 0, 0, 0);
            }
        }
    }

    void MoveUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            dragging = true;
            dragPoint = new Vector3(Input.mousePosition.x,0,Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
        if (dragging) {
            var diff = new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y) - dragPoint;
            Camera.main.transform.Translate(diff.normalized * diff.magnitude / Display.main.renderingHeight * cameraSpeed * Time.deltaTime, Space.World);
        }
    }

    void RotateUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
            var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
            var dir = Camera.main.transform.forward;
            var origin = mouse3D;
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info, 1 << 8)) {
                dragging = true;
                dragAnchor = info.point;
                dragPoint = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
        if (dragging) {
            var diff = (Input.mousePosition.x - dragPoint.x) / Display.main.renderingHeight ;
            Camera.main.transform.Rotate(0, diff, 0, Space.World);
        }
    }
}
