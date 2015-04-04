using UnityEngine;
using System.Collections;

public class LoginGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//a login stub for the moment
		API.SessionManager sm = API.SessionManager.Instance;
		//wait for login to happen
		StartCoroutine(Wait());
		Debug.Log ("duh");
	}
	IEnumerator Wait()
	{
		// suspend execution for 5 seconds
		yield return new WaitForSeconds(5);
		Catalog.Refresh ();

	}
	

}
