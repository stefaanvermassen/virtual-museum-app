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

		public HTTP.Request uploadImage(string name, string mime, string imageLocation, byte[] image, Action<Hashtable> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm();
			form.AddBinaryData(imageLocation, image, name, MIME + mime);
			return postForm(BASE_URL + ARTWORK, form, true, success, error);
		}

	}
}

