/* Can be added to any camera to make it isometric with the desired settings.
 */
using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour {

	void Start () {
        var camera = GetComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 5;
        camera.transform.rotation = Quaternion.Euler(new Vector3(45, 45, 0));
        camera.transform.transform.position = new Vector3(-10, 14, -10);
	}

    public void Move(float x) {
        gameObject.transform.position += new Vector3(x, 0, 0);
    }
}
