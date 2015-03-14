using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API {

	/// <summary>
	/// User controller. Does all request concerning the User, such as OAuth, and user management tasks.
	/// </summary>
	public class UserController : APIConnection
	{
		public User user;

		protected UserController() {
			//TODO: load user from cache or settings
			if (user == null || string.IsNullOrEmpty (user.accessToken)) {
				stubLogin();
			}
		}

		private static readonly UserController _instance = new UserController();

		public static UserController Instance {
			get {
				return _instance;
			}
		}
		
		public bool createUser(User user){
				return false;
		}

		public bool loginUser(User user, string password) {
				return false;
		}

		public bool updateCredit(User user, int credit) {
				//TODO: needs to be implemented in API
				return false;
		}

		public void stubLogin()
		{	
			string name = "Virtual Museum";
			user = new User (name, null);
			post("http://api.awesomepeople.tv/Token", 
				new string[] { "grant_type", "username", "password" }, 
				new string[] { "password", "VirtualMuseum", "@wesomePeople_20" }, 
			((response) => {
				string accessToken = (string)response.Object["access_token"];
				Debug.Log (accessToken);
				user.accessToken = accessToken;
			}), ((API_Error)=>{
				Debug.Log("An error  occured while requesting the token.");
			}), false);
		}
	}

	public class User {
		private string Name;
		//TODO: fix this and use long lived OAuth tokens, instead of requesting new ones every time
		public string accessToken { get; set;}
		private int CurrentArtist = 1;

		public User(string name, string accessToken) {
			Name = name;
			this.accessToken = accessToken;
		}

		public void clearToken() {
			accessToken = null;
		}
	}

}
