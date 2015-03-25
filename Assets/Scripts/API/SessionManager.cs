using UnityEngine;
using System.Collections;

namespace API {
	/// <summary>
	/// /Controls the User Session, stands above the UserController, and does some OAuth thingies.
	/// </summary>
	public class SessionManager : MonoBehaviour
	{
		/// <summary>
		/// Get the current access token to authenticate requests
		/// </summary>
		/// <returns>The access token.</returns>
		public string getAccessToken() {
			return API.UserController.Instance.user.accessToken;
		}
	}
	
}

