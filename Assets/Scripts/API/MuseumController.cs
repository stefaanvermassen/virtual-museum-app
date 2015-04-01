using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API
{
	/// <summary>
	/// Museum controller.
	/// </summary>
	public class MuseumController: APIConnection
	{
	    private const string MUSEUM = "museum";

	    public MuseumController ()
		{
		}

		private static readonly MuseumController _Instance = new MuseumController();
		
		public static MuseumController Instance {
			get {
				return _Instance;
			}
		}

		/// <summary>
		/// Gets the museum.
		/// </summary>
		/// <returns>The museum.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request GetMuseum(string id, Action<Museum> success = null, Action<API.API_Error> error = null) {
			return Get (BASE_URL + MUSEUM + "/" + id, ((response) => {
				if(success != null) {
					success(Museum.FromDictionary(response.Object));
				}}), error);
		}

		/// <summary>
		/// Creates the museum.
		/// </summary>
		/// <returns>The museum.</returns>
		/// <param name="museum">Museum.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request CreateMuseum(Museum museum, Action<Museum> success = null, Action<API.API_Error> error = null)
		{
			var form = museum.ToDictionary ();

			return Post (BASE_URL + MUSEUM, form, ((response) => {
				if(success != null) {
					var m = Museum.FromDictionary(response.Object);
					success(m);
				}
			}), error, true);
		}


		/// <summary>
		/// Updates the museum.
		/// </summary>
		/// <returns>The museum.</returns>
		/// <param name="museum">Museum.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request UpdateMuseum(Museum museum, Action<Museum> success = null, Action<API.API_Error> error = null)
		{
			var form = museum.ToDictionary ();
			
			return Put (BASE_URL + MUSEUM + "/" + museum.MuseumID.ToString(), form, ((response) => {
				if(success != null) {
					var m = Museum.FromDictionary(response.Object);
					success(m);
				}
			}), error, true);
		}

		/// <summary>
		/// Uploads the museum.
		/// </summary>
		/// <returns>The museum.</returns>
		/// <param name="name">Name.</param>
		/// <param name="museumLocation">Image location.</param>
		/// <param name="image">Image.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request UploadMuseumData(string id, string name, string museumLocation, byte[] museum, Action<HTTP.Response> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm();
			form.AddBinaryData(museumLocation, museum, name, "museum/binary");
			return PostForm(BASE_URL + MUSEUM + "/" + id, form, success, error, true);
		}

		/// <summary>
		/// Lists the museums.
		/// </summary>
		/// <returns>The museums.</returns>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request ListMuseums(Action<HTTP.Response> success = null, Action<API.API_Error> error = null)
		{
			throw new NotImplementedException ();
			return null;
		}

		/// <summary>
		/// Gets the museum data.
		/// </summary>
		/// <returns>The museum data.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request GetMuseumData(string id, Action<byte[]> success = null, Action<API.API_Error> error = null) {
			return Get (BASE_URL + MUSEUM + "/" + id + "/data", ((response) => {
				if(success != null) {
					success(response.bytes);
				}}), error);
		}

		/// <summary>
		/// Gets the random museum.
		/// </summary>
		/// <returns>The random museum.</returns>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request GetRandomMuseum(Action<Museum> success = null, Action<API.API_Error> error = null)
		{
			return Get(BASE_URL + MUSEUM + "/random", ((response) => {
				if(success != null) {
					var m = Museum.FromDictionary(response.Object);
					success(m);
				}}), error);
		}
	}

	/// <summary>
	/// Museum wrapper class
	/// </summary>
	public class Museum
	{
		public int MuseumID;
		public string Description;
		public DateTime LastModified;
		public Level Privacy;

		public Dictionary<string, string> ToDictionary(){
		    Dictionary<string, string> dict = new Dictionary<string, string>
		    {
		        {"MuseumID", MuseumID.ToString()},
		        {"Description", Description},
		        {"LastModified", LastModified.ToString()},
		        {"Privacy", ((int) Privacy).ToString()}
		    };

		    return dict;
		}

		public static Museum FromDictionary(Hashtable dict) {
			Museum m = new Museum () {
				MuseumID = (int)dict["MuseumID"],
				Description = (string)dict["Description"],
				LastModified = DateTime.Parse((string)dict["LastModified"]),
				Privacy = (Level)dict["Privacy"]
			};

			return m;
		}
	}

	public enum Level {
		PRIVATE = 0,
		PUBLIC = 1
	}
}

