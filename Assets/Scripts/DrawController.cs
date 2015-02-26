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
        var dir = Camera.main.transform.forward;
        var origin = mouse3D;
        Debug.DrawRay(origin, dir*100,Color.white,0.1f);
        if(Input.GetMouseButtonDown(0)){
            Debug.Log("Press "+origin+" "+dir);
            RaycastHit info;
            if (Physics.Raycast(origin, dir, out info,1 << 8)) {
                var hitPosition = new Vector3(Mathf.Floor(info.point.x+0.5f), 0.5f, Mathf.Floor(info.point.z+0.5f));
                Debug.Log(hitPosition +" "+ info.point);
                GameObject.Instantiate(toDraw, hitPosition, toDraw.transform.rotation);
            }
        }
	}
}
