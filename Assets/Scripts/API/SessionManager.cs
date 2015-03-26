using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API {
	/// <summary>
	/// /Controls the User Session, stands above the UserController, and does some OAuth thingies.
	/// </summary>
	public class SessionManager
	{
		private User loggedInUser;

		protected SessionManager() {
			//TODO: remove this when we have a login view
			var request = UserController.Instance.login ("VirtualMuseum", "@wesomePeople_20", ((user)=>{
				loginUser(user);
			}), ((error)=>{
				Debug.Log("An error occured when logging in");
			}));
		}
		
		private static readonly SessionManager _instance = new SessionManager();
		
		public static SessionManager Instance {
			get {
				return _instance;
			}
		}

		/// <summary>
		/// Get the current access token to authenticate requests
		/// </summary>
		/// <returns>The access token.</returns>
		public string getAccessToken() {
			//Debug.Log (loggedInUser.AccessToken());
			if (loggedInUser != null && loggedInUser.AccessToken () != null) {
				return loggedInUser.AccessToken ().AccessToken ();
			} else {
				Debug.LogWarning("No user token available, actions which require authorization will not work.");
				return "";
			}
		}

		public void loginUser(User user) {
			loggedInUser = user;
		}
	}
	
	public class User
	{
		private string Name;

		private Token accessToken;
		private int CurrentArtist = 1;

		public User(string name, Token accessToken) {
			Name = name;
			this.accessToken = accessToken;
		}
		
		public void clearToken() {
			accessToken = null;
		}

		public Token AccessToken() {
			return accessToken;
		}
	}

	public class Token
	{
		private string accessToken;
		private DateTime expires;

		private Token(string token, DateTime expires)
		{
			this.accessToken = token;
			this.expires = expires;
		}
		public static Token createFromDictionary(Hashtable hash) 
		{
			return new Token ((string)hash["access_token"], DateTime.Parse((string)hash[".expires"]));
		}

		public bool needsRefreshing() {
			//TODO: implement
			var newExpireDate = expires;
			newExpireDate.AddDays (-5.0);
			return  newExpireDate < DateTime.Now;
		}

		public string AccessToken() {
			return accessToken;
		}
	}
}

