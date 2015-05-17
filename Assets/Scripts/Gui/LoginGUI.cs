using UnityEngine;
using System.Collections;

public class LoginGUI : MonoBehaviour {

    // TODO: Is this still needed?
	// Use this for initialization
	void Start () {
		//a login stub for the moment
		API.SessionManager sm = API.SessionManager.Instance;
		//wait for login to happen
		//todo use asyncloader and check the key of the login
//		AsyncLoader loader = AsyncLoader.CreateAsyncLoader(
//			() => {
//			Debug.Log("Started");
//		},() => {
//			//ewait for accestoken to be received
//			return !sm.GetAccessToken().Equals("");
//		},
//		() => {
//		},
//		() => {
//			Debug.Log("Loaded");
//			//this is debug, nomally the method is called implicitly by tiestamp and change on the server
//			Catalog.RefreshArtWork ();
//			
//		});
	}


}
