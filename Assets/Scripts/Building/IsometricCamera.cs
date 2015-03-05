using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var camera = GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5;
        camera.transform.rotation = Quaternion.Euler(new Vector3(45, 45, 0));
        camera.transform.transform.position = new Vector3(-10, 14, -10);
	}

    public void Move(float x) {
        gameObject.transform.position += new Vector3(x,0,0);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
