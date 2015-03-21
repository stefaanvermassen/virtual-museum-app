using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API
{
	/// <summary>
	/// Artwork controller.
	/// </summary>
	public class ArtworkController: APIConnection
	{

		private const string MIME = "image/";
		private const string ARTWORK = "artwork";

		protected ArtworkController ()
		{

		}

		private static readonly ArtworkController _instance = new ArtworkController();

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <value>The instance.</value>
		public static ArtworkController Instance {
			get {
				return _instance;
			}
		}

		/// <summary>
		/// Gets all artworks by artist.
		/// </summary>
		/// <returns>The all artworks by artist.</returns>
		/// <param name="artistID">Artist I.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getAllArtworksByArtist(string artistID, Action<ArrayList> success = null, Action<API.API_Error> error = null){
			return getArtworksByFilter ("?ArtistID=" + artistID, success, error);
		}

		/// <summary>
		/// Gets the name of the all artworks by.
		/// </summary>
		/// <returns>The all artworks by name.</returns>
		/// <param name="name">Name.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getAllArtworksByName(string name, Action<ArrayList> success = null, Action<API.API_Error> error = null){
			return getArtworksByFilter ("?Name=" + name, success, error);
		}

		/// <summary>
		/// Gets all artworks.
		/// </summary>
		/// <returns>The all artworks.</returns>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getAllArtworks(Action<ArrayList> success = null, Action<API.API_Error> error = null) {
			return getArtworksByFilter ("", success, error);
		}

		/// <summary>
		/// Gets the artworks by filter.
		/// </summary>
		/// <returns>The artworks by filter.</returns>
		/// <param name="filter">Filter.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		private HTTP.Request getArtworksByFilter(string filter, Action<ArrayList> success = null, Action<API.API_Error> error = null) {
			//TODO: create filter class
			return get (BASE_URL + ARTWORK + filter, ((response) => {
				if(success != null) {
					success((ArrayList)response.Object["ArtWorks"]);
				}}), error);
		}

		/// <summary>
		/// Gets the artwork.
		/// </summary>
		/// <returns>The artwork.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getArtwork(string id, Action<Texture2D> success = null, Action<API.API_Error> error = null) {
			return get (BASE_URL + ARTWORK + "/" + id, ((response) => {
				if(success != null) {
					Texture2D tex = new Texture2D(2,2);
					tex.LoadImage(response.bytes);
					success(tex);
				}}), error);
		}

		/// <summary>
		/// Uploads the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="name">Name.</param>
		/// <param name="mime">MIME.</param>
		/// <param name="imageLocation">Image location.</param>
		/// <param name="image">Image.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request uploadImage(string name, string mime, string imageLocation, byte[] image, Action<HTTP.Response> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm();
			form.AddBinaryData(imageLocation, image, name, MIME + mime);
			return postForm(BASE_URL + ARTWORK, form, success, error, true);
		}

		/// <summary>
		/// Updates the art work.
		/// </summary>
		/// <returns>The art work.</returns>
		/// <param name="artwork">Artwork.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request updateArtWork(ArtWork artwork, Action<HTTP.Response> success = null, Action<API.API_Error> error = null){
			Dictionary<string, string> form = artwork.ToDictionary ();

			return put (BASE_URL + ARTWORK + "/" + artwork.ArtWorkID, form, success, error);
		}
	}

	/// <summary>
	/// Art work.
	/// </summary>
	public class ArtWork {
		public int ArtWorkID;
		public int ArtistID;
		public string Name;

		public Dictionary<string, string> ToDictionary() {
			Dictionary<string, string> dict = new Dictionary<string, string>();
			dict.Add ("ArtWorkID", ArtWorkID.ToString());
			dict.Add ("ArtistID", ArtistID.ToString());
			dict.Add ("Name", Name);

			return dict;
		}

		public static ArtWork FromDictionary(Hashtable dict) {
			ArtWork aw = new ArtWork () {
				ArtWorkID = ((int)dict["ArtWorkID"]),
				ArtistID = ((int)dict["ArtistID"]),
				Name = (string)dict["Name"]
			};

			return aw;
		}
	}
}

