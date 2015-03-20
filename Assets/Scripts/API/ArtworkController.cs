using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API
{
	public class ArtworkController: APIConnection
	{

		private const string MIME = "image/";
		private const string ARTWORK = "artwork";

		protected ArtworkController ()
		{

		}

		private static readonly ArtworkController _instance = new ArtworkController();

		public static ArtworkController Instance {
			get {
				return _instance;
			}
		}

		public HTTP.Request getAllArtworksByArtist(string artistID, Action<ArrayList> success = null, Action<API.API_Error> error = null){
			return getArtworksByFilter ("?ArtistID=" + artistID, success, error);
		}

		public HTTP.Request getAllArtworksByName(string name, Action<ArrayList> success = null, Action<API.API_Error> error = null){
			return getArtworksByFilter ("?Name=" + name, success, error);
		}

		public HTTP.Request getAllArtworks(Action<ArrayList> success = null, Action<API.API_Error> error = null) {
			return getArtworksByFilter ("", success, error);
		}

		private HTTP.Request getArtworksByFilter(string filter, Action<ArrayList> success = null, Action<API.API_Error> error = null) {
			//TODO: create filter class
			return get (BASE_URL + ARTWORK + filter, ((response) => {
				if(success != null) {
					success((ArrayList)response.Object["ArtWorks"]);
				}}), error);
		}

		public HTTP.Request getArtwork(string id, Action<Texture2D> success = null, Action<API.API_Error> error = null) {
			return get (BASE_URL + ARTWORK + "/" + id, ((response) => {
				if(success != null) {
					Texture2D tex = new Texture2D(2,2);
					tex.LoadImage(response.bytes);
					success(tex);
				}}), error);
		}

		public HTTP.Request uploadImage(string name, string mime, string imageLocation, byte[] image, Action<HTTP.Response> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm();
			form.AddBinaryData(imageLocation, image, name, MIME + mime);
			return postForm(BASE_URL + ARTWORK, form, success, error, true);
		}

		public HTTP.Request updateArtWork(ArtWork artwork, Action<HTTP.Response> success = null, Action<API.API_Error> error = null){
			Dictionary<string, string> form = new Dictionary<string, string>();
			form.Add ("ArtWorkID", artwork.ArtWorkID);
			form.Add ("ArtistID", artwork.ArtistID);
			form.Add ("Name", artwork.Name);

			return put (BASE_URL + ARTWORK + "/" + artwork.ArtWorkID, form, success, error);
		}
	}

	public class ArtWork {
		public string ArtWorkID;
		public string ArtistID;
		public string Name;
	}
}

