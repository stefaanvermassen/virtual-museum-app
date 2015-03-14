using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace API {

	public enum API_Error {
		REQUEST_FAILED = 1,
		REQUEST_NOT_ALLOWED = 2,
		USER_NOT_AUTHENTICATED = 3,
		OBJECT_NOT_FOUND = 4,
		SERVER_ERROR = 5,
	}

	public abstract class APIConnection
	{
		protected const string BASE_URL = "http://api.awesomepeople.tv/api/";

		protected HTTP.Request get(string url,  Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true) {
			return sendRequest (new HTTP.Request ("get", url), success, error, authToken);
		}

		protected HTTP.Request post(string url, string[] name, string[] value)
		{
			return post (url, name, value, null, null, true);
		}

		protected HTTP.Request post(string url, string[] name, string[] value, Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true)
		{
			WWWForm form = new WWWForm();
			for (int i = 0; i < name.Length; i++)
			{
				form.AddField(name[i], value[i]);
			}
			
			return postForm(url, form, success, error, authToken);
		}

		protected HTTP.Request post(string url, Dictionary<string, string> formData, Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true)
		{
			WWWForm form = new WWWForm ();
			foreach (var pair in formData) {
				form.AddField(pair.Key, pair.Value);
			}
			return postForm(url, form, success, error, authToken);
		}

		protected HTTP.Request postForm(string url, WWWForm form, Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true)
		{
			return postOrPutForm ("post", url, form, success, error, authToken);
		}

		protected HTTP.Request postOrPutForm(string method, string url, WWWForm form, Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true)
		{
			HTTP.Request postRequest = new HTTP.Request ("post", url, form);

			return sendRequest(postRequest, success, error, authToken);
		}

		protected HTTP.Request put(string url, Dictionary<string, string> formData, Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true)
		{
			WWWForm form = new WWWForm ();
			foreach (var pair in formData) {
				form.AddField(pair.Key, pair.Value);
			}

			return postOrPutForm ("put", url, form, success, error, authToken);
		}

		private HTTP.Request sendRequest(HTTP.Request sendRequest, Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true) 
		{
			if (authToken) {
				sendRequest.AddHeader("Authorization", "Bearer " + UserController.Instance.user.accessToken);
			}

			sendRequest.Send ((request) => {
				HTTP.Response response = request.response;
				if(response.status == 200) {
					if(success != null) {
						success(response);
					}
				} else {
					Debug.Log("Response status: " + response.status);
					Debug.Log("Response text: " + response.Text);
					var api_error = API_Error.REQUEST_FAILED;
					switch(response.status) {
					case 404:
						api_error = API_Error.OBJECT_NOT_FOUND;
						break;
					case 401:
						api_error = API_Error.USER_NOT_AUTHENTICATED;
						break;
					case 500:
						api_error = API_Error.SERVER_ERROR;
						break;
					}
					if(error != null) {
						error(api_error);
					}
				}
			});
			
			return sendRequest;
		}

	}

}
