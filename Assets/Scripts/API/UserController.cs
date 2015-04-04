using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API {

	/// <summary>
	/// User controller. Does all request concerning the User, such as OAuth, and user management tasks.
	/// </summary>
	public class UserController : APIConnection
	{
		protected UserController() {
		}

		private static readonly UserController _Instance = new UserController();

		public static UserController Instance {
			get {
				return _Instance;
			}
		}
		
		public HTTP.Request CreateUser(string username, string email, string password, Action<User> succes = null, Action<API_Error> error = null){
			return Post(BASE_URL+"account/register", 
			            new string[] { "UserName", "Email", "Password", "ConfirmPassword" }, 
			new string[] { username, email, password, password }, 
			((response) => {
				Login(username, password, succes, error);
			}), error, false);
		}

		public bool UpdateCredit(User user, int credit) {
				//TODO: needs to be implemented in API
				return false;
		}

		public HTTP.Request Login(string username, string password, Action<User> succes = null, Action<API_Error> error = null )
		{
			return Post("http://api.awesomepeople.tv/Token", 
			     new string[] { "grant_type", "username", "password" }, 
			     new string[] { "password", username, password }, 
			((response) => {
				Token token = Token.CreateFromDictionary(response.Object);
				Debug.Log(token.AccessToken());
				User user = new User((string)response.Object["userName"], token);
				if (succes != null) {
					succes(user);
				}
			}), error, false);
		}
	}

}
