using System;
using System.Collections;
using UnityEngine;

namespace API
{
    /// <summary>
    ///     /Controls the User Session, stands above the UserController, and does some OAuth thingies.
    /// </summary>
    public class SessionManager
    {
        private User _loggedInUser;

        protected SessionManager()
        {
            //TODO: remove this when we have a login view
            UserController.Instance.Login("VirtualMuseum", "@wesomePeople_20", (LoginUser),
                (error => { Debug.Log("An error occured when logging in"); }));
        }

        private static readonly SessionManager _Instance = new SessionManager();

        public static SessionManager Instance
        {
            get { return _Instance; }
        }

        /// <summary>
        ///     Get the current access token to authenticate requests
        /// </summary>
        /// <returns>The access token.</returns>
        public string GetAccessToken()
        {
            //Debug.Log (loggedInUser.AccessToken());
            if (_loggedInUser != null && _loggedInUser.AccessToken() != null)
            {
                return _loggedInUser.AccessToken().AccessToken();
            }
            Debug.LogWarning("No user token available, actions which require authorization will not work.");
            return "";
        }

        public void LoginUser(User user)
        {
            _loggedInUser = user;
        }
    }

    public class User
    {
        private Token _accessToken;

        public User(string name, Token accessToken)
        {
            _accessToken = accessToken;
        }

        public void ClearToken()
        {
            _accessToken = null;
        }

        public Token AccessToken()
        {
            return _accessToken;
        }
    }

    public class Token
    {
        private readonly string _accessToken;
        private readonly DateTime _expires;

        private Token(string token, DateTime expires)
        {
            _accessToken = token;
            _expires = expires;
        }

        public static Token CreateFromDictionary(Hashtable hash)
        {
            return new Token((string) hash["access_token"], DateTime.Parse((string) hash[".expires"]));
        }

        public bool NeedsRefreshing()
        {
            var newExpireDate = _expires;
            newExpireDate = newExpireDate.AddDays(-5.0);
            return newExpireDate < DateTime.Now;
        }

        public string AccessToken()
        {
            return _accessToken;
        }
    }
}

