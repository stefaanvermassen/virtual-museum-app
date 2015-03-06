using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DrawController : MonoBehaviour {

    public enum Tools : int {
        Drawing,
        Moving,
        Rotating,
        Erasing,
        Scaling,
        PlacingObject,
        PlacingArt
    }

    public GameObject toDraw;
    public Museum currentMuseum;
    public float cameraSpeed = 10;
    public float edgeRatio = 0.05f;
    public Tools tool = Tools.Drawing;
    public Texture2D debugTexture;

    private Vector3 dragPoint = Vector3.zero;
    private Vector3 dragAnchor = Vector3.zero;

    private bool dragging = false;
    private Vector3 centerPointWorld = Vector3.zero;
    private Vector3 anchorPointScreen = Vector3.zero;
    private Vector3 anchorPointWorld = Vector3.zero;

    private LayerMask groundLayerMask;

	void Start () {
        groundLayerMask = (1 << LayerMask.NameToLayer("Ground"));
	}

    public void SetTool(int tool) {
        this.tool = (Tools)tool;
    }

    public bool IsPointerBusy() {
        foreach (Touch touch in Input.touches) {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return true;
            if (touch.phase == TouchPhase.Ended) return true;
        }
        return EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject();
    }
	
    void Update() {
        var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
        var dragPointScreen = Vector3.zero;
        var dragPointWorld = Vector3.zero;
        Debug.DrawRay(mouse3D, Camera.main.transform.forward * 100, Color.white, 0.1f);
        if (Input.GetMouseButtonDown(0) && !IsPointerBusy()) {
            RaycastHit mouseInfo;
            if (Physics.Raycast(mouse3D, Camera.main.transform.forward, out mouseInfo, Mathf.Infinity, groundLayerMask)) {
                dragging = true;
                anchorPointScreen = mouse2D;
                anchorPointWorld = mouseInfo.point;
            }
            RaycastHit centerInfo;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out centerInfo, Mathf.Infinity, groundLayerMask)) {
                centerPointWorld = centerInfo.point;
            }
        }
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
        if (dragging) {
            RaycastHit mouseInfo;
            if (Physics.Raycast(mouse3D, Camera.main.transform.forward, out mouseInfo, Mathf.Infinity, groundLayerMask)) {
                dragPointScreen = mouse2D;
                dragPointWorld = mouseInfo.point;
            }
            switch (tool) {
                case Tools.Drawing:
                    Draw(dragPointWorld);
                    break;
                case Tools.Moving:
                    Move(dragPointWorld, anchorPointWorld);
                    break;
                case Tools.Rotating:
                    Rotate(dragPointScreen, anchorPointScreen, centerPointWorld);
                    break;
                case Tools.Erasing:
                    Erase(dragPointWorld);
                    break;
                case Tools.Scaling:
                    Scale(dragPointScreen, anchorPointScreen);
                    break;
                case Tools.PlacingObject:
                    PlaceObject(dragPointWorld, anchorPointWorld);
                    break;
                case Tools.PlacingArt:
                    PlaceArt(dragPointWorld, anchorPointWorld);
                    break;
            }
        }
    }

    void Draw(Vector3 dragPointWorld) {
        currentMuseum.SetTile((int)Mathf.Floor(dragPointWorld.x + 0.5f), 0, (int)Mathf.Floor(dragPointWorld.z + 0.5f), 0, 0, 0);
    }

    void Erase(Vector3 dragPointWorld) {
        currentMuseum.RemoveTile((int)Mathf.Floor(dragPointWorld.x + 0.5f), 0, (int)Mathf.Floor(dragPointWorld.z + 0.5f));
    }

    void Move(Vector3 dragPointWorld, Vector3 anchorPointWorld) {
        Camera.main.transform.Translate((dragPointWorld - anchorPointWorld).normalized * cameraSpeed * Time.deltaTime, Space.World);
    }

    void Rotate(Vector3 dragPointScreen, Vector3 anchorPointScreen, Vector3 centerPointWorld) {
        var diff = (dragPointScreen.x - anchorPointScreen.x) / Display.main.renderingWidth;
        Camera.main.transform.Translate(new Vector3(diff, 0, 0) * cameraSpeed * Time.deltaTime);
        Camera.main.transform.LookAt(centerPointWorld);
    }

    void Scale(Vector3 dragPointScreen, Vector3 anchorPointScreen) {
        var diff = (anchorPointScreen.x - dragPointScreen.x) / Display.main.renderingHeight * cameraSpeed * Time.deltaTime;
        Camera.main.orthographicSize += diff;
    }

    void PlaceObject(Vector3 dragPointWorld, Vector3 anchorPointWorld) {
        var diff = (dragPointWorld - anchorPointWorld).normalized;
        var angle = -(Mathf.Atan2(diff.z, diff.x) + Mathf.PI / 2) / Mathf.PI * 180;
        currentMuseum.AddObject(Resources.Load<GameObject>("texmonkey"), anchorPointWorld + new Vector3(0, 0.5f, 0), new Vector3(0, angle, 0));
    }

    void PlaceArt(Vector3 dragPointWorld, Vector3 anchorPointWorld) {
        var diff = (dragPointWorld - anchorPointWorld).normalized;
        var angle = -(Mathf.Atan2(diff.z, diff.x) + Mathf.PI / 2) / Mathf.PI * 180;
        if (angle < 0) angle = 360 + angle;
        var orientation = (int)angle / 90;
        currentMuseum.AddArt((int)Mathf.Floor(anchorPointWorld.x + 0.5f), 0, (int)Mathf.Floor(anchorPointWorld.z + 0.5f), orientation, debugTexture);
    }
}
