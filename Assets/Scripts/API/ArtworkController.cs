using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API
{
	/// <summary>
	/// Artwork controller.
	/// To create: uploadImage, and after this fill in the necessary fields, and call updateMuseum
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
		/// Gets the artwork.
		/// </summary>
		/// <returns>The artwork.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getArtwork(string id, Action<ArtWork> success = null, Action<API.API_Error> error = null) {
			return get (BASE_URL + ARTWORK + "/" + id, ((response) => {
				if(success != null) {
					success(ArtWork.FromDictionary(response.Object));
				}}), error);
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
		/// <param name="success">Success. Returns an arrayList with Artwork objects</param>
		/// <param name="error">Error.</param>
		private HTTP.Request getArtworksByFilter(string filter, Action<ArrayList> success = null, Action<API.API_Error> error = null) {
			//TODO: create filter class
			return get (BASE_URL + ARTWORK + filter, ((response) => {
				if(success != null) {
					var apiList = (ArrayList)response.Object["ArtWorks"];
					var list = new ArrayList();
					foreach(Hashtable val in apiList) {
						list.Add(ArtWork.FromDictionary(val));
					}
					success(list);
				}}), error);
		}

		/// <summary>
		/// Gets the artwork data.
		/// </summary>
		/// <returns>The artwork data.</returns>
		/// <param name="id">ArtWork Identifier. The ArtWorkID for which we request the data</param>
		/// <param name="success">Success. Returns a byte[]</param>
		/// <param name="error">Error.</param>
		public HTTP.Request getArtworkData(string id, Action<byte[]> success = null, Action<API.API_Error> error = null) {
			return get (BASE_URL + ARTWORK + "/" + id + "/data", ((response) => {
				if(success != null) {
					success(response.bytes);
				}}), error);
		}

		/// <summary>
		/// Uploads the image.
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="name">Name.</param>
		/// <param name="mime">MIME. Image type, to easily decode on the backend</param>
		/// <param name="imageLocation">Image location. Param to find it by on the backend</param>
		/// <param name="image">Image. the image as a byte[]</param>
		/// <param name="success">Success. Returns an artwork</param>
		/// <param name="error">Error.</param>
		public HTTP.Request uploadImage(string name, string mime, string imageLocation, byte[] image, Action<ArtWork> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm();
			form.AddBinaryData(imageLocation, image, name, MIME + mime);
			return postForm(BASE_URL + ARTWORK, form, ((response) => {
				if (success != null) {
					ArrayList respArray = (ArrayList)response.Object["ArtWorks"];
					ArtWork artWork = ArtWork.FromDictionary((Hashtable)respArray[0]);
					success(artWork);
				}
			}), error, true);
		}

		/// <summary>
		/// Updates the art work.
		/// </summary>
		/// <returns>HTTP.Request</returns>
		/// <param name="artwork">Artwork. the artwork to update, filled we the new values.</param>
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
		//make sure string is not null
		public string Name="";

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

        public static ArtWork FromArt(Art art)
        {
            return new ArtWork()
            {
                ArtWorkID = art.ID,
                ArtistID = art.owner.ID,
                Name = art.name
            };
        }
	}
}

