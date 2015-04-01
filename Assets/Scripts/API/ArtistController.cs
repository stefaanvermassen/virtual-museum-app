using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace API {

	/// <summary>
	/// Artist controller: control the artist requests
	/// </summary>
	public class ArtistController : APIConnection
	{
		private static string ARTIST = "artist";

		protected ArtistController() {

		}
		
		private static readonly ArtistController _Instance = new ArtistController();
		
		public static ArtistController Instance {
			get {
				return _Instance;
			}
		}

		public HTTP.Request GetArtists(Action<ArrayList> success = null, Action<API.API_Error> error = null)
		{
			return GetBaseArtist("", success, error);
		}

		public HTTP.Request GetConnectedArtists(Action<ArrayList> success = null, Action<API.API_Error> error = null)
		{
			return GetBaseArtist ("/connected", success, error);
		}
			
		private HTTP.Request GetBaseArtist(string url, Action<ArrayList> success = null, Action<API.API_Error> error = null)
		{
			return Get(url, ((response) => {
				var apiList = (ArrayList)response.Object["Artists"];
				var list = new ArrayList();
				foreach(Hashtable val in apiList) {
					list.Add(Artist.FromDictionary(val));
				}
				if(success != null) {
					success(list);
				}
			}), error);
		}

		public HTTP.Request GetArtist(int id, Action<Artist> success = null, Action<API.API_Error> error = null)
		{
			return Get(BASE_URL + ARTIST + "/" + id.ToString(), ((response) => {
				var artist = Artist.FromDictionary(response.Object);
				if(success != null){
					success(artist);
				}
			}), error);
		}

		public HTTP.Request CreateArtist(Artist artist, Action<Artist> success = null, Action<API.API_Error> error = null)
		{
			return Post(BASE_URL+ARTIST, new string[] {"id", "Name"}, new string[]{"0", artist.Name}, ((response) => {
				var a = Artist.FromDictionary(response.Object);
				if(success != null){
					success(a);
				}
			}), error);
		}

		public HTTP.Request UpdateArtist(Artist artist, Action<Artist> success = null, Action<API.API_Error> error = null)
		{
			return Put (BASE_URL + ARTIST + "/" + artist.ID.ToString (), artist.ToDictionary (), ((response) => {
				var a = Artist.FromDictionary (response.Object);
				if (success != null) {
					success (a);
				}
			}), error);
		}
	}

	public class Artist
	{
		public string Name;
		public int ID;

		public Dictionary<string, string> ToDictionary() {
		    Dictionary<string, string> dict = new Dictionary<string, string>
		    {
		        {"Name", Name}, 
                {"ArtistID", ID.ToString()}
		    };

		    return dict;
		}

		public static Artist FromDictionary(Hashtable hash)
		{
			return new Artist() {Name = (string)hash["Name"],
				ID = (int)hash["ArtistID"]
				};
		}
	}
}

