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

    public int currentArt = 1;
    public int currentObject = 0;
    public int currentFloor = 0;
    public int currentWall = 0;
    public int currentCeiling = 0;
	public int currentFrame = 0;

    private bool[] dragging = {false, false, false, false, false};
    private Vector3 centerPointWorld = Vector3.zero;
    private Vector3 anchorPointScreen = Vector3.zero;
    private Vector3 anchorPointWorld = Vector3.zero;
    private Vector3 anchorNormalWorld = Vector3.zero;
    private Vector3 lastDragPointScreen = Vector3.zero;
    private Vector3 cameraAnchor = Vector3.zero;

    private LayerMask groundLayerMask;
    private LayerMask wallLayerMask;

	void Start () {
        groundLayerMask = (1 << LayerMask.NameToLayer("Ground"));
        wallLayerMask = (1 << LayerMask.NameToLayer("Walls"));
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
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
                return true;
            }
            if (touch.phase == TouchPhase.Ended) {
                return true;
            }
        }
        return EventSystem.current.IsPointerOverGameObject(0) || EventSystem.current.IsPointerOverGameObject();
    }

    RaycastHit raycast(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask) {
        RaycastHit info = new RaycastHit();
        if (Physics.Raycast(origin, direction, out info, maxDistance, layerMask)) {
            return info;
        }
        return new RaycastHit();
    }

    void Update() {
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        ToolUpdate(0, tool);
        MouseUpdate();
#endif
#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount == 1) {
            ToolUpdate(0, tool);
        } else {
            GestureUpdate();
        }
#endif
    }
	
    void ToolUpdate(int mouseButton, Tools tool) {
        var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
        Debug.DrawRay(mouse3D, Camera.main.transform.forward * 100, Color.white, 0.1f);
        var mask = groundLayerMask;
        if (tool == Tools.PlacingArt) {
            mask = wallLayerMask;
        }
        if (Input.GetMouseButtonDown(mouseButton) && !IsPointerBusy()) {
            cameraAnchor = Camera.main.transform.position;
            dragging[mouseButton] = true;
            anchorPointScreen = mouse2D;
            var anchorWorld = raycast(mouse3D, Camera.main.transform.forward, Mathf.Infinity, mask);
            anchorPointWorld = anchorWorld.point;
            anchorNormalWorld = anchorWorld.normal;
            lastDragPointScreen = anchorPointScreen;
            centerPointWorld = raycast(cameraAnchor, Camera.main.transform.forward, Mathf.Infinity, groundLayerMask).point;
        }
        if (Input.GetMouseButtonUp(mouseButton)) {
            dragging[mouseButton] = false;
        }
        if (dragging[mouseButton]) {
            var dragPointScreen = Vector3.zero;
            var dragPointWorld = Vector3.zero;
            dragPointScreen = mouse2D;
            dragPointWorld = raycast(mouse3D, Camera.main.transform.forward, Mathf.Infinity, groundLayerMask).point;
            var dragOffsetScreen = anchorPointScreen - dragPointScreen;
            var dragOffsetWorld = anchorPointWorld - dragPointWorld;
            var frameOffsetScreen = dragPointScreen - lastDragPointScreen;
            switch (tool) {
                case Tools.Drawing:
                    Draw(dragPointWorld);
                    break;
                case Tools.Moving:
                    Move(dragOffsetWorld);
                    break;
                case Tools.Rotating:
                    Rotate(centerPointWorld, new Vector3(-frameOffsetScreen.y / Screen.height * 180, frameOffsetScreen.x / Screen.width * 180, 0));
                    break;
                case Tools.Erasing:
                    Erase(dragPointWorld);
                    break;
                case Tools.Scaling:
                    Scale(Mathf.Pow(2, -frameOffsetScreen.y / Screen.height));
                    break;
                case Tools.PlacingObject:
                    PlaceObject(dragPointWorld, anchorPointWorld);
                    break;
                case Tools.PlacingArt:
                    PlaceArt(dragPointWorld, anchorPointWorld, anchorNormalWorld, dragPointScreen, anchorPointScreen);
                    break;
            }
            lastDragPointScreen = dragPointScreen;
        }
    }

    void MouseUpdate() {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll < 0) {
            Scale(1.1f);
        } else if (scroll > 0) {
            Scale(0.9f);
        }
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
            Scale(Mathf.Pow(2, deltaMagnitudeDiff*2 / Screen.height));
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

    void PlaceArt(Vector3 dragPointWorld, Vector3 anchorPointWorld, Vector3 anchorNormalWorld, Vector3 dragPointScreen, Vector3 anchorPointScreen) {
        if (Vector3.Magnitude(anchorNormalWorld) < 0.5) {
            return;
        }
        var diff = Vector3.Distance(anchorPointScreen, dragPointScreen);
        var scale = 0.5f + 4*diff / Screen.width;
        currentMuseum.AddArt(currentArt, anchorPointWorld, Quaternion.LookRotation(anchorNormalWorld).eulerAngles,scale,currentFrame);
    }
}
