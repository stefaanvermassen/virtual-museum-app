using UnityEngine;
using System.Collections;

public class DrawController : MonoBehaviour {

    public GameObject toDraw;
    public Museum currentMuseum;
    public float cameraSpeed = 10;
    public float edgeRatio = 0.05f;

	void Start () {
	
	}
	
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
                currentMuseum.SetTile((int)Mathf.Floor(info.point.x+0.5f), 0, (int)Mathf.Floor(info.point.z+0.5f),0,0,0);
            }
        }
        var mouseRatio = new Vector2(mouse2D.x / Display.main.renderingWidth, mouse2D.y / Display.main.renderingHeight);
        if (mouseRatio.x < edgeRatio)       Camera.main.transform.position += new Vector3(-cameraSpeed, 0, 0) * Time.deltaTime;
        if (mouseRatio.x > 1 - edgeRatio)   Camera.main.transform.position += new Vector3(cameraSpeed, 0, 0) * Time.deltaTime;
        if (mouseRatio.y < edgeRatio)       Camera.main.transform.position += new Vector3(0, 0, -cameraSpeed) * Time.deltaTime;
        if (mouseRatio.y > 1 - edgeRatio)   Camera.main.transform.position += new Vector3(0, 0, cameraSpeed) * Time.deltaTime;
	}
}
