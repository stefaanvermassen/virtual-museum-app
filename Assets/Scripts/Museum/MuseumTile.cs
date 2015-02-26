﻿using UnityEngine;
using System.Collections;

public class MuseumTile : MonoBehaviour {

    public int x, y, z;
    public bool left, right, front, back;
    public int ceilingStyle;
    public int wallStyle;
    public int floorStyle;

    private GameObject upObject, downObject, leftObject, rightObject, frontObject, backObject;

	void Start () {
        upObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        upObject.transform.parent = gameObject.transform;
        upObject.transform.Rotate(new Vector3(0, 90, 0));
        downObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        downObject.transform.parent = gameObject.transform;
        downObject.transform.Rotate(new Vector3(0, 90, 0));
        UpdateEdges();
	}
	
	void Update () {
	
	}

    public void Remove() {
        if (upObject != null)       Destroy(upObject);
        if (downObject != null)     Destroy(downObject);
        if (leftObject != null)     Destroy(leftObject);
        if (rightObject != null)    Destroy(rightObject);
        if (frontObject != null)    Destroy(frontObject);
        if (backObject != null)     Destroy(backObject);
    }

    public void UpdateEdges() {
        if (left && leftObject == null) {
            leftObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            leftObject.transform.parent = gameObject.transform;
            leftObject.transform.Rotate(new Vector3(0, 90, 0));
        }
        if (!left && leftObject != null) {
            Destroy(leftObject);
            leftObject = null;
        }
        if (right && rightObject == null) {
            rightObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            rightObject.transform.parent = gameObject.transform;
            rightObject.transform.Rotate(new Vector3(0, 90, 0));
        }
        if (!right && rightObject != null) {
            Destroy(rightObject);
            rightObject = null;
        }
        if (front && frontObject == null) {
            frontObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            frontObject.transform.parent = gameObject.transform;
            frontObject.transform.Rotate(new Vector3(0, 90, 0));
        }
        if (!front && frontObject != null) {
            Destroy(frontObject);
            frontObject = null;
        }
        if (back && backObject == null) {
            backObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            backObject.transform.parent = gameObject.transform;
            backObject.transform.Rotate(new Vector3(0, 90, 0));
        }
        if (!back && backObject != null) {
            Destroy(backObject);
            backObject = null;
        }
    }
}
