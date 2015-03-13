using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace API {

	public enum API_Error {
		REQUEST_FAILED = 1,
		REQUEST_NOT_ALLOW = 2,
		USER_NOT_AUTHENTICATED = 3
	}

	public abstract class APIConnection
	{
		protected const string BASE_URL = "http://api.awesomepeople.tv/api/";

		protected HTTP.Request post(string url, string[] name, string[] value)
		{
			return post (url, name, value, true);
		}

		protected HTTP.Request post(string url, string[] name, string[] value, bool authToken = true, Action<Hashtable> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm();
			for (int i = 0; i < name.Length; i++)
			{
				form.AddField(name[i], value[i]);
			}
			
			return postForm(url, form, authToken, success, error);
		}

		protected HTTP.Request post(string url, Dictionary<string, string> formData, bool authToken = true, Action<Hashtable> success = null, Action<API.API_Error> error = null)
		{
			WWWForm form = new WWWForm ();
			foreach (var pair in formData) {
				form.AddField(pair.Key, pair.Value);
			}
			return postForm(url, form, authToken, success, error);
		}

		protected HTTP.Request postForm(string url, WWWForm form, bool authToken = true, Action<Hashtable> success = null, Action<API.API_Error> error = null)
		{
			HTTP.Request postRequest = new HTTP.Request ("post", url, form);
			if (authToken) {
				postRequest.AddHeader("Authorization", "Bearer " + UserController.Instance.user.accessToken);
			}
			postRequest.Send ((request) => {
				HTTP.Response response = request.response;
				if(response.status == 200) {
					if(success != null) {
						success(response.Object);
					}
				} else {
					if(error != null) {
						//TODO: add API_Errors
						error(API_Error.REQUEST_FAILED);
					}
				}
			});

			return postRequest;
		}
	}

}
