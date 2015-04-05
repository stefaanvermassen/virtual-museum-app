using UnityEngine;
using System.Collections;

public class LoginGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//a login stub for the moment
		API.SessionManager sm = API.SessionManager.Instance;
		//wait for login to happen
		//todo use asyncloader and check the key of the login
		AsyncLoader loader = AsyncLoader.CreateAsyncLoader(
			() => {
			Debug.Log("Started");
		},() => {
			//ewait for accestoken to be received
			return !sm.GetAccessToken().Equals("");
		},
		() => {
			Debug.Log("Still loading");
		},
		() => {
			Debug.Log("Loaded");
			Catalog.Refresh ();
			
		});
	}
	IEnumerator Wait()
	{
		// suspend execution for 5 seconds
		yield return new WaitForSeconds(5);


	}
	

}
