using System;
using System.Collections;
using System.Collections.Generic;
using HTTP;
using UnityEngine;

namespace API
{
    /// <summary>
    ///     API Error Codes
    /// </summary>
    public enum API_Error
    {
        REQUEST_FAILED = 1,
        REQUEST_NOT_ALLOWED = 2,
        USER_NOT_AUTHENTICATED = 3,
        OBJECT_NOT_FOUND = 4,
        SERVER_ERROR = 5
    }

    /// <summary>
    ///     API connection. Base class for the ObjectControllers, which fetch the requests from the API.
    /// </summary>
    public abstract class APIConnection
    {
        /// <summary>
        ///     URL which points to the API End Point
        /// </summary>
        protected const string BASE_URL = "http://api.awesomepeople.tv/api/";

        /// <summary>
        ///     Do a GET request to the server for a given URL
        /// </summary>
        /// <param name="url">URL. the location of the request</param>
        /// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
        /// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
        /// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
        protected Request Get(string url, Action<Response> success = null, Action<API_Error> error = null,
            bool authToken = true)
        {
            return SendRequest(new Request("get", url), success, error, authToken);
        }
        
        /// <summary>
        ///     Do a DELETE request to the server for a given URL
        /// </summary>
        /// <param name="url">URL. the location of the request</param>
        /// <param name="success">Success. Closure that gets run if deleting the file is succesfull</param>
        /// <param name="error">Error. Closure that gets run if deleting the file is unsuccesfull</param>
        /// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>

        protected Request Delete(string url, Action<Response> success = null, Action<API_Error> error = null,
            bool authToken = true)
        {
            return SendRequest(new Request("delete", url), success, error, authToken);
        }

        /// <summary>
        ///     Simple post, without closures
        /// </summary>
        /// <param name="url">URL. the location of the request</param>
        /// <param name="name">Name. Array with keys</param>
        /// <param name="value">Value. Array with values</param>
        protected Request Post(string url, string[] name, string[] value)
        {
            return Post(url, name, value, null, null, true);
        }

        /// <summary>
        ///     Post with key and values.
        /// </summary>
        /// <param name="url">URL. the location of the request</param>
        /// <param name="name">Name. Array with keys</param>
        /// <param name="value">Value. Array with values</param>
        /// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
        /// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
        /// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
        protected Request Post(string url, string[] name, string[] value, Action<Response> success = null,
            Action<API_Error> error = null, bool authToken = true)
        {
            var form = new WWWForm();
            for (var i = 0; i < name.Length; i++)
            {
                form.AddField(name[i], value[i]);
            }

            return PostForm(url, form, success, error, authToken);
        }

        /// <summary>
        ///     Post with dictionary values.
        /// </summary>
        /// <param name="url">URL. the location of the request</param>
        /// <param name="formData">Dictionary which creates the form data.</param>
        /// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
        /// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
        /// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
        protected Request Post(string url, Hashtable formData, Action<Response> success = null,
            Action<API_Error> error = null, bool authToken = true)
        {
            var form = new WWWForm();
            foreach (DictionaryEntry pair in formData)
            {
                form.AddField((string)pair.Key, (string)pair.Value);
            }
            return PostForm(url, form, success, error, authToken);
        }

        /// <summary>
        ///     Posts the form with a WWWForm object.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="url">URL.</param>
        /// <param name="form">The WWWForm which contains the data.</param>
        /// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
        /// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
        /// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
        protected Request PostForm(string url, WWWForm form, Action<Response> success = null,
            Action<API_Error> error = null, bool authToken = true)
        {
            return PostOrPutForm("post", url, form, success, error, authToken);
        }

        protected Request PostOrPutForm(string method, string url, WWWForm form, Action<Response> success = null,
            Action<API_Error> error = null, bool authToken = true)
        {
            var postRequest = new Request(method, url, form);

            return SendRequest(postRequest, success, error, authToken);
        }

        protected Request Put(string url, Hashtable formData, Action<Response> success = null,
            Action<API_Error> error = null, bool authToken = true)
        {
            var form = new WWWForm();
            foreach (DictionaryEntry pair in formData)
            {
                form.AddField((string)pair.Key, (string)pair.Value);
            }

            return PostOrPutForm("put", url, form, success, error, authToken);
        }

        protected Request PostJsonRequest(string url, Hashtable json, Action<Response> success = null,
            Action<API_Error> error = null, bool authToken = true)
        {
            return PostOrPutJsonRequest("post", url, json, success, error, authToken);
        }

        protected Request PutJsonRequest(string url, Hashtable json, Action<Response> success = null,
            Action<API_Error> error = null, bool authToken = true)
        {
            return PostOrPutJsonRequest("put", url, json, success, error, authToken);
        }

        protected Request PostOrPutJsonRequest(string method, string url, Hashtable json, Action<Response> success = null,
            Action<API_Error> error = null, bool authToken = true)
        {
            var request = new Request(method, url, json);

            return SendRequest(request, success, error, authToken);
        }

        /// <summary>
        ///     Private method to send the requests, and call the right closures.
        /// </summary>
        /// <returns>The request.</returns>
        /// <param name="sendRequest">The request that needs to be sended</param>
        /// <param name="success">Success. Closure that gets run if downloading the file is succesfull</param>
        /// <param name="error">Error. Closure that gets run if downloading the file is unsuccesfull</param>
        /// <param name="authToken">If set to <c>true</c> auth token is added to the request.</param>
        private Request SendRequest(Request sendRequest, Action<Response> success = null, Action<API_Error> error = null,
            bool authToken = true)
        {
            if (authToken)
            {
                sendRequest.AddHeader("Authorization", "Bearer " + SessionManager.Instance.GetAccessToken());
            }

            sendRequest.Send(request =>
            {
                var response = request.response;
                if (response.status == 200)
                {
                    if (success != null)
                    {
                        success(response);
                    }
                }
                else
                {
                    Debug.Log("Response status: " + response.status);
                    Debug.Log("Response text: " + response.Text);

                    var apiError = API_Error.REQUEST_FAILED;
                    switch (response.status)
                    {
                        case 404:
                            apiError = API_Error.OBJECT_NOT_FOUND;
                            break;
                        case 401:
                            apiError = API_Error.USER_NOT_AUTHENTICATED;
                            break;
                        case 500:
                            apiError = API_Error.SERVER_ERROR;
                            break;
                        default:
                            break;
                    }
                    if (error != null)
                    {
                        error(apiError);
                    }
                }
            });

            return sendRequest;
        }
    }
}
