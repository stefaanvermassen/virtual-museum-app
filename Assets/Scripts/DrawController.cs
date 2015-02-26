using UnityEngine;
using System.Collections;

public class DrawController : MonoBehaviour {

    public GameObject toDraw;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update() {
        var mouse2D = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1);
        var mouse3D = Camera.main.ScreenToWorldPoint(mouse2D);
        var dir = (mouse3D - Camera.main.transform.position).normalized;
        var origin = Camera.main.transform.position;
        Debug.DrawRay(origin, dir*100,Color.white,0.1f);
        if(Input.GetMouseButtonDown(0)){
            Debug.Log("Press "+origin+" "+dir);
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info)) {
                var hitPosition = new Vector3((int)info.point.x, ((int)info.point.y) + 0.5f, (int)info.point.z);
                Debug.Log(hitPosition);
                GameObject.Instantiate(toDraw, hitPosition, toDraw.transform.rotation);
            }
        }
	}
}
