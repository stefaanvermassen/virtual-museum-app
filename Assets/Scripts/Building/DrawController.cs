using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Controller that handles all input and uses it to modify both the scene and the
/// museum when inside the BuildMuseum scene.
/// </summary>
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
    public Tools tool = Tools.Drawing;

    public int currentArt = 0;
    public int currentObject = 0;
    public int currentFloor = 0;
    public int currentWall = 0;
    public int currentCeiling = 0;

    private bool[] dragging = {false, false, false, false, false};
    private Vector3 centerPointWorld = Vector3.zero;
    private Vector3 anchorPointScreen = Vector3.zero;
    private Vector3 anchorPointWorld = Vector3.zero;
    private Vector3 lastDragPointScreen = Vector3.zero;
    private Vector3 cameraAnchor = Vector3.zero;

    private LayerMask groundLayerMask;

	void Start () {
        groundLayerMask = (1 << LayerMask.NameToLayer("Ground"));
	}

    /// <summary>
    /// Change the current tool by using the toolID. One can use the Tools enum and convert it to an int as well.
    /// </summary>
    /// <param name="tool">The int representation of a Tools enum value.</param>
    public void SetTool(int tool) {
        this.tool = (Tools)tool;
    }

    /// <summary>
    /// Change the current object by using the object's id.
    /// </summary>
    /// <param name="objectID"></param>
    public void SetCurrentObject(int objectID) {
        this.currentObject = objectID;
    }

    bool IsPointerBusy() {
        foreach (Touch touch in Input.touches) {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) return true;
            if (touch.phase == TouchPhase.Ended) return true;
        }
        return EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject();
    }

    Vector3 raycast(Vector3 origin, Vector3 direction, float maxDistance = Mathf.Infinity, int layermask = 1 << 8) {
        RaycastHit info;
        if (Physics.Raycast(origin, direction, out info, Mathf.Infinity, groundLayerMask)) {
            return info.point;
        }
        return Vector3.zero;
    }

    void Update() {
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        ToolUpdate(0, tool);
        MouseUpdate();
#endif
#if UNITY_IOS || UNITY_ANDROID
        if(Input.touchCount == 1)
            ToolUpdate(0, tool);
        else
            GestureUpdate();
#endif
    }
	
    void ToolUpdate(int mouseButton, Tools tool) {
        var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
        Debug.DrawRay(mouse3D, Camera.main.transform.forward * 100, Color.white, 0.1f);
        if (Input.GetMouseButtonDown(mouseButton) && !IsPointerBusy()) {
            cameraAnchor = Camera.main.transform.position;
            dragging[mouseButton] = true;
            anchorPointScreen = mouse2D;
            anchorPointWorld = raycast(mouse3D, Camera.main.transform.forward, Mathf.Infinity, groundLayerMask);
            lastDragPointScreen = anchorPointScreen;
            centerPointWorld = raycast(cameraAnchor, Camera.main.transform.forward, Mathf.Infinity, groundLayerMask);
        }
        if (Input.GetMouseButtonUp(mouseButton)) {
            dragging[mouseButton] = false;
        }
        if (dragging[mouseButton]) {
            var dragPointScreen = Vector3.zero;
            var dragPointWorld = Vector3.zero;
            dragPointScreen = mouse2D;
            dragPointWorld = raycast(mouse3D, Camera.main.transform.forward, Mathf.Infinity, groundLayerMask);
            var dragOffsetScreen = anchorPointScreen - dragPointScreen;
            var dragOffsetWorld = anchorPointWorld - dragPointWorld;
            var frameOffsetScreen = dragPointScreen - lastDragPointScreen;

            if      (tool == Tools.Drawing)         Draw(dragPointWorld);
            else if (tool == Tools.Moving)          Move(dragOffsetWorld);
            else if (tool == Tools.Rotating)        Rotate(centerPointWorld, new Vector3(-frameOffsetScreen.y / Display.main.renderingHeight * 180, frameOffsetScreen.x / Display.main.renderingWidth * 180, 0));
            else if (tool == Tools.Erasing)         Erase(dragPointWorld);
            else if (tool == Tools.Scaling)         Scale(Mathf.Pow(2, -frameOffsetScreen.y / Display.main.renderingHeight));
            else if (tool == Tools.PlacingObject)   PlaceObject(dragPointWorld, anchorPointWorld);
            else if (tool == Tools.PlacingArt)      PlaceArt(dragPointWorld, anchorPointWorld);

            lastDragPointScreen = dragPointScreen;
        }
    }

    void MouseUpdate() {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0) Scale(1.1f);
        if (scroll > 0) Scale(0.9f);
        ToolUpdate(1, Tools.Rotating);
        ToolUpdate(2, Tools.Moving);
    }

    void GestureUpdate() {
        if (Input.touchCount == 2) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            Scale(Mathf.Pow(2, deltaMagnitudeDiff*2 / Display.main.renderingHeight));
        }
    }

    void Draw(Vector3 dragPointWorld) {
        currentMuseum.SetTile(currentWall, currentFloor, currentCeiling, (int)Mathf.Floor(dragPointWorld.x + 0.5f), 0, (int)Mathf.Floor(dragPointWorld.z + 0.5f));
    }

    void Erase(Vector3 dragPointWorld) {
        currentMuseum.RemoveTile((int)Mathf.Floor(dragPointWorld.x + 0.5f), 0, (int)Mathf.Floor(dragPointWorld.z + 0.5f));
    }

    void Move(Vector3 movement) {
        Camera.main.transform.Translate(movement, Space.World);
    }

    void Rotate(Vector3 center, Vector3 angles) {
        Camera.main.transform.RotateAround(center, Camera.main.transform.right, angles.x);
        Camera.main.transform.RotateAround(center, new Vector3(0, 1, 0), angles.y);
        Camera.main.transform.RotateAround(center, new Vector3(0, 0, 1), angles.z);
    }

    void Scale(float factor) {
        Camera.main.orthographicSize *= factor;
    }

    void PlaceObject(Vector3 dragPointWorld, Vector3 anchorPointWorld) {
        var diff = (dragPointWorld - anchorPointWorld).normalized;
        var angle = -(Mathf.Atan2(diff.z, diff.x) - Mathf.PI / 2) / Mathf.PI * 180;
        currentMuseum.AddObject(currentObject, (int)Mathf.Floor(anchorPointWorld.x + 0.5f), 0, (int)Mathf.Floor(anchorPointWorld.z + 0.5f), angle);
    }

    void PlaceArt(Vector3 dragPointWorld, Vector3 anchorPointWorld) {
        var diff = (dragPointWorld - anchorPointWorld).normalized;
        var angle = -(Mathf.Atan2(diff.z, diff.x) + Mathf.PI/4) / Mathf.PI * 180;
        if (angle < 0) angle = 360 + angle;
        var orientation = (int)angle / 90;
        currentMuseum.AddArt(currentArt, (int)Mathf.Floor(anchorPointWorld.x + 0.5f), 0, (int)Mathf.Floor(anchorPointWorld.z + 0.5f), orientation);
    }
}
