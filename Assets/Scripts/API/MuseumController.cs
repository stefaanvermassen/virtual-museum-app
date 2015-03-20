using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace API
{
	public class MuseumController: APIConnection
	{
		private static string MUSEUM = "museum";

		public MuseumController ()
		{
		}

		private static readonly MuseumController _instance = new MuseumController();
		
		public static MuseumController Instance {
			get {
				return _instance;
			}
		}

		public HTTP.Request uploadMuseum(string name, string imageLocation, byte[] image, Action<HTTP.Response> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm();
			form.AddBinaryData(imageLocation, image, name, "museum/binary");
			return postForm(BASE_URL + MUSEUM, form, success, error, true);
		}
	}
}

