using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace API {

	/// <summary>
	/// API Error Codes
	/// </summary>
	public enum API_Error {
		REQUEST_FAILED = 1,
		REQUEST_NOT_ALLOWED = 2,
		USER_NOT_AUTHENTICATED = 3,
		OBJECT_NOT_FOUND = 4,
		SERVER_ERROR = 5,
	}

	/// <summary>
	/// API connection. Base class for the ObjectControllers, which fetch the requests from the API.
	/// </summary>
	public abstract class APIConnection
	{
		/// <summary>
		/// URL which points to the API End Point
		/// </summary>
		protected const string BASE_URL = "http://api.awesomepeople.tv/api/";

		/// <summary>
		/// Do a GET request to the server for a given URL
		/// </summary>
		/// <param name="url">URL. the location of the request</param>
		/// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
		/// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
		/// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
		protected HTTP.Request get(string url,  Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true) {
			return sendRequest (new HTTP.Request ("get", url), success, error, authToken);
		}

		/// <summary>
		/// Simple post, without closures
		/// </summary>
		/// <param name="url">URL. the location of the request</param>
		/// <param name="name">Name. Array with keys</param>
		/// <param name="value">Value. Array with values</param>
		protected HTTP.Request post(string url, string[] name, string[] value)
		{
			return post (url, name, value, null, null, true);
		}

		/// <summary>
		/// Post with key and values.
		/// </summary>
		/// <param name="url">URL. the location of the request</param>
		/// <param name="name">Name. Array with keys</param>
		/// <param name="value">Value. Array with values</param>
		/// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
		/// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
		/// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
		protected HTTP.Request post(string url, string[] name, string[] value, Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true)
		{
			WWWForm form = new WWWForm();
			for (int i = 0; i < name.Length; i++)
			{
				form.AddField(name[i], value[i]);
			}
			
			return postForm(url, form, success, error, authToken);
		}

		/// <summary>
		/// Post with dictionary values.
		/// </summary>
		/// <param name="url">URL. the location of the request</param>
		/// <param name="formData">Dictionary which creates the form data.</param>
		/// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
		/// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
		/// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
		protected HTTP.Request post(string url, Dictionary<string, string> formData, Action<HTTP.Response> success = null, Action<API.API_Error> error = null, bool authToken = true)
		{
			WWWForm form = new WWWForm ();
			foreach (var pair in formData) {
				form.AddField(pair.Key, pair.Value);
			}
			return postForm(url, form, success, error, authToken);
		}

		/// <summary>
		/// Posts the form with a WWWForm object.
		/// </summary>
		/// <returns>The request.</returns>
		/// <param name="url">URL.</param>
		/// <param name="form">The WWWForm which contains the data.</param>
		/// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
		/// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
		/// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
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

		/// <summary>
		/// Private method to send the requests, and call the right closures.
		/// </summary>
		/// <returns>The request.</returns>
		/// <param name="sendRequest">The request that needs to be sended</param>
		/// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
		/// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
		/// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
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
