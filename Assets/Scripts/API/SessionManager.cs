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
			UserController.Instance.stubLogin (); //TODO: remove this when we have a login view
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
			return loggedInUser.AccessToken.AccessToken;
		}

		public void loginUser(User user) {
			loggedInUser = user;
		}
	}
	
	public class User
	{
		private string Name;

		public Token AccessToken { get; }
		private int CurrentArtist = 1;

		public User(string name, Token accessToken) {
			Name = name;
			this.AccessToken = accessToken;
		}
		
		public void clearToken() {
			AccessToken = null;
		}
	}

	public class Token
	{
		public string AccessToken{get;};
		DateTime expires;

		private Token(string token, DateTime expires)
		{
			this.AccessToken = token;
			this.expires = expires;
		}
		public static Token createFromDictionary(Hashtable hash) 
		{
			return new Token ((string)hash["access_token"], DateTime.Parse((string)hash[".expires"]));
		}

		public bool needsRefreshing() {
			//TODO: implement
			var newExpireDate = expires;
			newExpireDate.AddDays (-5);
			return  newExpireDate < DateTime.Now;
		}
	}
}

