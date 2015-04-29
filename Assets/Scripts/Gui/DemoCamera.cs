using UnityEngine;
using System.Collections;
using System;

public class DemoCamera : MonoBehaviour {

	public Camera cam;
	Vector3 defaultPos;
	Quaternion defaultRot;
	public double speed;
	public GameObject target;
	double speedScale;
	public double rad;
	int counter = 0;

	// Use this for initialization
	void Start () {
		defaultPos = cam.transform.localPosition;
		defaultRot = cam.transform.localRotation;
		speedScale = (0.001*2*Math.PI)/speed;
	}
	
	// Update is called once per frame
	void Update () {
		var angle = (++counter)*speedScale;
		var horRelative = Math.Sin (angle) * rad;
		var verRelative = Math.Cos (angle) * rad;

		Vector3 newPos = new Vector3 ((float)horRelative, (float)verRelative, 0f);
		newPos = defaultRot * newPos;
		newPos += defaultPos;

		cam.transform.localPosition = newPos;
		cam.transform.LookAt (target.transform.position);
	}
}
