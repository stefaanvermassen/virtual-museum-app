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

		private static readonly UserController _instance = new UserController();

		public static UserController Instance {
			get {
				return _instance;
			}
		}
		
		public HTTP.Request createUser(string username, string email, string password, Action<User> succes = null, Action<API_Error> error = null){
			return post(BASE_URL+"account/register", 
			            new string[] { "UserName", "Email", "Password", "ConfirmPassword" }, 
			new string[] { username, email, password, password }, 
			((response) => {
				login(username, password, succes, error);
			}), error, false);
		}

		public bool updateCredit(User user, int credit) {
				//TODO: needs to be implemented in API
				return false;
		}

		public HTTP.Request login(string username, string password, Action<User> succes = null, Action<API_Error> error = null )
		{
			return post("http://api.awesomepeople.tv/Token", 
			     new string[] { "grant_type", "username", "password" }, 
			     new string[] { "password", username, password }, 
			((response) => {
				Token token = Token.createFromDictionary(response.Object);
				Debug.Log(token.AccessToken());
				User user = new User((string)response.Object["userName"], token);
				if (succes != null) {
					succes(user);
				}
			}), error, false);
		}
	}

}
