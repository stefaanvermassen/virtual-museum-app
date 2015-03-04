using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DrawController : MonoBehaviour {

    public const int DRAWING_TOOL = 0;
    public const int MOVING_TOOL = 1;
    public const int ROTATING_TOOL = 2;

    public const int ERASE_TOOL = 3;
    public const int SCALE_TOOL = 4;
    public const int PLACE_OBJECT_TOOL = 5;
    public const int PLACE_ART_TOOL = 6;

    public GameObject toDraw;
    public Museum currentMuseum;
    public float cameraSpeed = 10;
    public float edgeRatio = 0.05f;
    public int tool = 0;
    public Texture2D debugTexture;

    private bool dragging = false;
    private Vector3 dragPoint = Vector3.zero;
    private Vector3 dragAnchor = Vector3.zero;

    private LayerMask groundLayerMask;

	void Start () {
        groundLayerMask = (1 << LayerMask.NameToLayer("Ground"));
	}

    public void SetTool(int tool) {
        this.tool = tool;
    }

    public bool IsPointerBusy() {
        foreach (Touch touch in Input.touches) {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return true;
            if (touch.phase == TouchPhase.Ended) return true;
        }
        return EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject();
    }
	
    void Update() {
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
            case ERASE_TOOL:
                DrawUpdate(true);
                break;
            case SCALE_TOOL:
                ScaleUpdate();
                break;
            case PLACE_OBJECT_TOOL:
                PlaceObjectUpdate();
                break;
            case PLACE_ART_TOOL:
                PlaceArtUpdate();
                break;
        }
	}

    void DrawUpdate(bool erase = false) {
        var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
        var dir = Camera.main.transform.forward;
        var origin = mouse3D;
        Debug.DrawRay(origin, dir * 100, Color.white, 0.1f);
        if (Input.GetMouseButton(0) && !IsPointerBusy()) {
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,Mathf.Infinity , groundLayerMask)) {
                var hitPosition = new Vector3(Mathf.Floor(info.point.x + 0.5f), 0.5f, Mathf.Floor(info.point.z + 0.5f));
                if (erase) currentMuseum.RemoveTile((int)Mathf.Floor(info.point.x + 0.5f), 0, (int)Mathf.Floor(info.point.z + 0.5f));
                else currentMuseum.SetTile((int)Mathf.Floor(info.point.x + 0.5f), 0, (int)Mathf.Floor(info.point.z + 0.5f), 0, 0, 0);
            }
        }
    }

    void MoveUpdate() {
        var dir = Camera.main.transform.forward;
        var origin = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        if (Input.GetMouseButtonDown(0) && !IsPointerBusy()) {
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,Mathf.Infinity , groundLayerMask)) {
                dragging = true;
                dragAnchor = info.point;
                dragPoint = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
        if (dragging) {
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,Mathf.Infinity , groundLayerMask)) {
                Camera.main.transform.Translate((info.point - dragAnchor).normalized * cameraSpeed * Time.deltaTime, Space.World);
            }
            
        }
    }

    void RotateUpdate() {
        if (Input.GetMouseButtonDown(0) && !IsPointerBusy()) {
            var dir = Camera.main.transform.forward;
            var origin = Camera.main.transform.position;
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,Mathf.Infinity , groundLayerMask)) {
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
            Camera.main.transform.Translate(new Vector3(diff, 0, 0) * cameraSpeed * Time.deltaTime);
            Camera.main.transform.LookAt(dragAnchor);
        }
    }

    void ScaleUpdate() {
        if (Input.GetMouseButtonDown(0) && !IsPointerBusy()) {
            var dir = Camera.main.transform.forward;
            var origin = Camera.main.transform.position;
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,Mathf.Infinity , groundLayerMask)) {
                dragging = true;
                dragPoint = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
        if (dragging) {
            var diff = (dragPoint.y - Input.mousePosition.y)/Display.main.renderingHeight * cameraSpeed * Time.deltaTime;
            Camera.main.orthographicSize += diff;
        }
    }

    void PlaceObjectUpdate() {
        if (Input.GetMouseButtonDown(0) && !IsPointerBusy()) {
            var dir = Camera.main.transform.forward;
            var origin = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,Mathf.Infinity , groundLayerMask)){
                Debug.Log("layer " + info.collider.gameObject.layer);
                currentMuseum.AddObject(Resources.Load<GameObject>("texmonkey"), info.point + new Vector3(0,0.5f,0));
            }
        }
    }

    void PlaceArtUpdate() {
        var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
        var dir = Camera.main.transform.forward;
        var origin = mouse3D;
        if (Input.GetMouseButtonDown(0) && !IsPointerBusy()) {
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,Mathf.Infinity , groundLayerMask)) {
                dragging = true;
                dragPoint = new Vector3(Mathf.Floor(info.point.x + 0.5f), 0.5f, Mathf.Floor(info.point.z + 0.5f));
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
        if (dragging) {
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,Mathf.Infinity , groundLayerMask)) {
                var point = new Vector3(Mathf.Floor(info.point.x + 0.5f), 0.5f, Mathf.Floor(info.point.z + 0.5f));
                var diff = (point - dragPoint).normalized;
                var angle = -(Mathf.Atan2(diff.z, diff.x) + Mathf.PI/2) / Mathf.PI * 180;
                if (angle < 0) angle = 360 + angle;
                var orientation = (int)angle / 90;
                currentMuseum.AddArt((int)dragPoint.x, (int)dragPoint.y, (int)dragPoint.z, orientation, debugTexture);
            }
        }
    }
}
