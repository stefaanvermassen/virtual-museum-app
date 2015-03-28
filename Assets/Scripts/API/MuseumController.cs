using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API
{
	/// <summary>
	/// Museum controller.
	/// To create a new museum, first create, than add the data.
	/// </summary>
	public class MuseumController: APIConnection
	{
		private static string  MUSEUM = "museum";

		public MuseumController ()
		{
		}

		private static readonly MuseumController _instance = new MuseumController();
		
		public static MuseumController Instance {
			get {
				return _instance;
			}
		}

		/// <summary>
		/// Gets the museum.
		/// </summary>
		/// <returns>An HTTP.Request to follow up on the request.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="success">Success. A closure which will be executed when creating the museum is a succes, gives you a museum object with the needed id.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getMuseum(string id, Action<Museum> success = null, Action<API.API_Error> error = null) {
			return get (BASE_URL + MUSEUM + "/" + id, ((response) => {
				if(success != null) {
					success(Museum.FromDictionary(response.Object));
				}}), error);
		}

		/// <summary>
		/// Creates the museum, should be the first task when creating a museum
		/// </summary>
		/// <returns>HTTP.Request</returns>
		/// <param name="museum">Museum is an API class which contains all necessary fields, MuseumID can be left empty when creating a new museum</param>
		/// <param name="success">Success. A closure which will be executed when creating the museum is a succes, gives you a museum object with the needed id.</param>
		/// <param name="error">Error. Handle errors</param>
		public HTTP.Request createMuseum(Museum museum, Action<Museum> success = null, Action<API.API_Error> error = null)
		{
			var form = museum.ToDictionary ();

			return post (BASE_URL + MUSEUM, form, ((response) => {
				if(success != null) {
					var m = Museum.FromDictionary(response.Object);
					success(m);
				}
			}), error, true);
		}


		/// <summary>
		/// Updates the museum info, should be called when renaming, or adding more info.
		/// </summary>
		/// <returns>HTTP.Request</returns>
		/// <param name="museum">Museum is an API class which contains all APIfields, MuseumID is required</param>
		/// <param name="success">Success. A closure which will be executed when creating the museum is a succes, gives you a museum object with the needed id.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request updateMuseum(Museum museum, Action<Museum> success = null, Action<API.API_Error> error = null)
		{
			var form = museum.ToDictionary ();
			
			return put (BASE_URL + MUSEUM + "/" + museum.MuseumID.ToString(), form, ((response) => {
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
		/// <param name="name">Name of the museum</param>
		/// <param name="museum">Byte array which contains the museum data</param>
		/// <param name="success">Success. A closure which will be executed when creating the museum is a succes, returns a Response when succesfull</param>
		/// <param name="error">Error.</param>
		public HTTP.Request uploadMuseumData(string id, string name, byte[] museum, Action<HTTP.Response> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm();
			form.AddBinaryData("museum", museum, name, "museum/binary");
			return postForm(BASE_URL + MUSEUM + "/" + id, form, success, error, true);
		}

		/// <summary>
		/// Lists the museums.
		/// </summary>
		/// <returns>The museums.</returns>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request listMuseums(Action<HTTP.Response> success = null, Action<API.API_Error> error = null)
		{
			throw new NotImplementedException ();
			return null;
		}

		/// <summary>
		/// Gets the museum data.
		/// </summary>
		/// <returns>HTTP.Reuqest</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="success">Success, closure variable is a byte[], which contains the museum data</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getMuseumData(string id, Action<byte[]> success = null, Action<API.API_Error> error = null) {
			return get (BASE_URL + MUSEUM + "/" + id + "/data", ((response) => {
				if(success != null) {
					success(response.bytes);
				}}), error);
		}

		/// <summary>
		/// Gets the random museum.
		/// </summary>
		/// <returns>HTTP.Request</returns>
		/// <param name="success">Succes gives the random museum object, after this you should call the getMuseumData function</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getRandomMuseum(Action<Museum> success = null, Action<API.API_Error> error = null)
		{
			return get(BASE_URL + MUSEUM + "/random", ((response) => {
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
			Dictionary<string, string> dict = new Dictionary<string, string>();
			dict.Add ("MuseumID", MuseumID.ToString());
			dict.Add ("Description", Description);
			dict.Add ("LastModified", LastModified.ToString());
			dict.Add ("Privacy", ((int)Privacy).ToString());

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

