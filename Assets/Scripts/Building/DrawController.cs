﻿using UnityEngine;
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

    private bool dragging = false;
    private Vector3 centerPointWorld = Vector3.zero;
    private Vector3 anchorPointScreen = Vector3.zero;
    private Vector3 anchorPointWorld = Vector3.zero;
    private Vector3 lastDragPointScreen = Vector3.zero;
    private Vector3 cameraAnchor = Vector3.zero;

    private LayerMask groundLayerMask;

	void Start () {
        groundLayerMask = (1 << LayerMask.NameToLayer("Ground"));
	}

    public void SetTool(int tool) {
        this.tool = (Tools)tool;
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
        var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
        Debug.DrawRay(mouse3D, Camera.main.transform.forward * 100, Color.white, 0.1f);
        if (Input.GetMouseButtonDown(0) && !IsPointerBusy()) {
            cameraAnchor = Camera.main.transform.position;
            dragging = true;
            anchorPointScreen = mouse2D;
            anchorPointWorld = raycast(mouse3D, Camera.main.transform.forward, Mathf.Infinity, groundLayerMask);
            lastDragPointScreen = anchorPointScreen;
            centerPointWorld = raycast(cameraAnchor, Camera.main.transform.forward, Mathf.Infinity, groundLayerMask);
        }
        if (Input.GetMouseButtonUp(0)) {
            dragging = false;
        }
        if (dragging) {
            var dragPointScreen = Vector3.zero;
            var dragPointWorld = Vector3.zero;
            dragPointScreen = mouse2D;
            dragPointWorld = raycast(mouse3D, Camera.main.transform.forward, Mathf.Infinity, groundLayerMask);
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
                    Rotate(centerPointWorld, frameOffsetScreen.x/Display.main.renderingWidth*180);
                    break;
                case Tools.Erasing:
                    Erase(dragPointWorld);
                    break;
                case Tools.Scaling:
                    Scale(Mathf.Pow(2,-frameOffsetScreen.y / Display.main.renderingHeight));
                    break;
                case Tools.PlacingObject:
                    PlaceObject(dragPointWorld, anchorPointWorld);
                    break;
                case Tools.PlacingArt:
                    PlaceArt(dragPointWorld, anchorPointWorld);
                    break;
            }
            lastDragPointScreen = dragPointScreen;
        }
    }

    void Draw(Vector3 dragPointWorld) {
        currentMuseum.SetTile((int)Mathf.Floor(dragPointWorld.x + 0.5f), 0, (int)Mathf.Floor(dragPointWorld.z + 0.5f), 0, 0, 0);
    }

    void Erase(Vector3 dragPointWorld) {
        currentMuseum.RemoveTile((int)Mathf.Floor(dragPointWorld.x + 0.5f), 0, (int)Mathf.Floor(dragPointWorld.z + 0.5f));
    }

    void Move(Vector3 movement) {
        Camera.main.transform.Translate(movement, Space.World);
    }

    void Rotate(Vector3 center, float amount) {
        Camera.main.transform.RotateAround(center, new Vector3(0, 1, 0), amount);
    }

    void Scale(float factor) {
        Camera.main.orthographicSize *= factor;
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