using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace API
{
    /// <summary>
    ///     /Controls the User Session, stands above the UserController, and does some OAuth thingies.
    /// </summary>
    public class SessionManager
    {
        private User _loggedInUser;

        private const string CurrentVersion = "1.0";

        protected SessionManager()
        {
            var user = ReadUserFromFile();
            if (user != null)
            {
                LoginUser(user); 
                Debug.Log("Logged in from file!");
            }
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
            if (_loggedInUser != null && _loggedInUser.AccessToken() != null)
            {
                return _loggedInUser.AccessToken().AccessToken();
            }
            Debug.LogWarning("No user token available, actions which require authorization will not work.");
            return "";
        }

        /// <summary>
        /// Check if the user is logged in at the moment
        /// </summary>
        /// <returns>
        /// True when the user is logged in, false otherwise
        /// </returns>
        public bool LoggedIn()
        {
            return !(_loggedInUser == null || _loggedInUser.AccessToken() == null ||
                   _loggedInUser.AccessToken().NeedsRefreshing());
        }

        /// <summary>
        /// Log the user in, should be called after logging in the UserController.
        /// </summary>
        /// <param name="user">The user whom needs to login.</param>
        public void LoginUser(User user)
        {
            _loggedInUser = user;
            StoreCurrentUser();
        }

        private void StoreCurrentUser()
        {
            var hash = new Hashtable();
            hash["user"] = _loggedInUser.CreateHashtable();
            hash["version"] = CurrentVersion;

            Stream fileStream = File.Create(getPath());
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(fileStream, hash);
            fileStream.Close();
            Debug.Log("Data Saved Locally to " + getPath());
        }

        private User ReadUserFromFile()
        {
            string path = getPath();
            if (File.Exists(path))
            {
                Stream fileStream = File.OpenRead(path);
                BinaryFormatter deserializer = new BinaryFormatter();
                var data = (Hashtable)deserializer.Deserialize(fileStream);
                fileStream.Close();

                if (((string) data["version"]).Equals(CurrentVersion))
                {
                    var user = User.CreateUser((Hashtable)data["user"]);
                    if (user.AccessToken().NeedsRefreshing())
                    {
                        //TODO: implement refresh
                    }
                    return user;
                }
            }
            return null;
        }

        private string getPath()
        {
            //Make sure the folder exists
            string path = Application.persistentDataPath + "/3DVirtualMuseum/userinfo";
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
            {
                Directory.CreateDirectory(path);
            }
            return path + "/userinfo.json";
        }
    }

    public class User
    {
        private string _name;

        private Token _accessToken;
        private int _currentArtist = 1;

        public User(string name, Token accessToken)
        {
            _name = name;
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

        public Hashtable CreateHashtable()
        {
            Hashtable h = new Hashtable();
            h["name"] = _name;
            h["token"] = _accessToken.CreateHashtable();
            return h;
        }

        public static User CreateUser(Hashtable h)
        {
            return new User((string)h["name"], Token.CreateToken((Hashtable)h["token"]));
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

        public static Token Create(Hashtable hash)
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

        public Hashtable CreateHashtable()
        {
            Hashtable h = new Hashtable();
            h["access_token"] = _accessToken;
            h["expire_date"] = _expires;

            return h;
        }

        public static Token CreateToken(Hashtable h)
        {
            return new Token((string)h["access_token"], (DateTime)h["expire_date"]);
        }
    }
}

