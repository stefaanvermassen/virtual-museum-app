using System;
using System.Collections;
using System.Collections.Generic;
using HTTP;
using UnityEngine;

namespace API
{
    /// <summary>
    ///     Museum controller.
    ///     To create a new museum, first create, than add the data.
    /// </summary>
    public class MuseumController : APIConnection
    {
        private const string MUSEUM = "museum";

        private static readonly MuseumController _Instance = new MuseumController();

        public static MuseumController Instance
        {
            get { return _Instance; }
        }

        /// <summary>
        ///     Gets the museum.
        /// </summary>
        /// <returns>An HTTP.Request to follow up on the request.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="success">
        ///     Success. A closure which will be executed when creating the museum is a succes, gives you a
        ///     museum object with the needed id.
        /// </param>
        /// <param name="error">Error.</param>
        public Request GetMuseum(string id, Action<Museum> success = null, Action<API_Error> error = null)
        {
            return Get(BASE_URL + MUSEUM + "/" + id, (response =>
            {
                if (success != null)
                {
                    success(Museum.Create(response.Object));
                }
            }), error);
        }

		/// <summary>
		/// Get the list of museums owned by the user
		/// </summary>
		/// <returns>The Request</returns>
		/// <param name="success">Success. A closure which will be executed when creating the museum is a succes, gives you the
		///     list of museums which are owned by the user.</param>
		/// <param name="error">Error.</param>
		public Request GetConnectedMuseums(Action<ArrayList> success = null, Action<API_Error> error = null)
		{
			return Get(BASE_URL + MUSEUM + "/connected", (response =>
			                 {
				var apiList = (ArrayList) response.Object["Museums"];
				var list = new ArrayList();
				foreach (Hashtable val in apiList)
				{
					list.Add(Museum.Create(val));
				}
				if (success != null)
				{
					success(list);
				}
			}), error);
		}

		
		/// <summary>
		///     Lists the museums.
		/// </summary>
		/// <returns>The museums.</returns>
		/// <param name="success">Success.</param>
		/// <param name="error">Error.</param>
		public Request GetMuseums(MuseumSearchModel msm = null, Action<ArrayList> success = null, Action<API_Error> error = null)
		{
			msm = msm != null ? msm : new MuseumSearchModel ();
			return Get (BASE_URL + MUSEUM + msm.QueryString() ,
			          (response =>
		    {
				var apiList = (ArrayList) response.Object["Museums"];
				var list = new ArrayList();
				foreach (Hashtable val in apiList)
				{
					list.Add(Museum.Create(val));
				}
				if (success != null)
				{
					success(list);
				}
			}), error);
		}
        /// <summary>
        ///     Creates the museum, should be the first task when creating a museum
        /// </summary>
        /// <returns>HTTP.Request</returns>
        /// <param name="museum">
        ///     Museum is an API class which contains all necessary fields, MuseumID can be left empty when
        ///     creating a new museum
        /// </param>
        /// <param name="success">
        ///     Success. A closure which will be executed when creating the museum is a succes, gives you a
        ///     museum object with the needed id.
        /// </param>
        /// <param name="error">Error. Handle errors</param>
        public Request CreateMuseum(Museum museum, Action<Museum> success = null, Action<API_Error> error = null)
        {
            var form = museum.ToHash();

            return Post(BASE_URL + MUSEUM, form, (response =>
            {
                if (success != null)
                {
                    var m = Museum.Create(response.Object);
                    success(m);
                }
            }), error, true);
        }


        /// <summary>
        ///     Updates the museum info, should be called when renaming, or adding more info.
        /// </summary>
        /// <returns>HTTP.Request</returns>
        /// <param name="museum">Museum is an API class which contains all APIfields, MuseumID is required</param>
        /// <param name="success">
        ///     Success. A closure which will be executed when creating the museum is a succes, gives you a
        ///     museum object with the needed id.
        /// </param>
        /// <param name="error">Error.</param>
        public Request UpdateMuseum(Museum museum, Action<Museum> success = null, Action<API_Error> error = null)
        {
            var form = museum.ToHash();

            return Put(BASE_URL + MUSEUM + "/" + museum.MuseumID, form, (response =>
            {
                if (success != null)
                {
                    var m = Museum.Create(response.Object);
                    success(m);
                }
            }), error, true);
        }

        /// <summary>
        ///     Uploads the museum.
        /// </summary>
        /// <returns>The museum.</returns>
        /// <param name="name">Name of the museum</param>
        /// <param name="museum">Byte array which contains the museum data</param>
        /// <param name="success">
        ///     Success. A closure which will be executed when creating the museum is a succes, returns a
        ///     Response when succesfull
        /// </param>
        /// <param name="error">Error.</param>
        public Request UploadMuseumData(string id, string name, byte[] museum, Action<Response> success = null,
            Action<API_Error> error = null)
        {
            var form = new WWWForm();
            form.AddBinaryData("museum", museum, name, "museum/binary");
            return PostForm(BASE_URL + MUSEUM + "/" + id, form, success, error, true);
        }

        /// <summary>
        ///     Gets the museum data.
        /// </summary>
        /// <returns>HTTP.Reuqest</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="success">Success, closure variable is a byte[], which contains the museum data</param>
        /// <param name="error">Error.</param>
        public Request GetMuseumData(string id, Action<byte[]> success = null, Action<API_Error> error = null)
        {
            return Get(BASE_URL + MUSEUM + "/" + id + "/data", (response =>
            {
                if (success != null)
                {
                    success(response.bytes);
                }
            }), error);
        }

        /// <summary>
        ///     Gets the random museum.
        /// </summary>
        /// <returns>HTTP.Request</returns>
        /// <param name="success">Succes gives the random museum object, after this you should call the getMuseumData function</param>
        /// <param name="error">Error.</param>
        public Request GetRandomMuseum(Action<Museum> success = null, Action<API_Error> error = null)
        {
            return Get(BASE_URL + MUSEUM + "/random", (response =>
            {
                if (success != null)
                {
                    var m = Museum.Create(response.Object);
                    success(m);
                }
            }), error);
        }
        
        public Request DeleteMuseum(int museumID, Action<Response> success = null, Action<API_Error> error = null)
        {
            return Delete(BASE_URL + MUSEUM + "/" + museumID, success, error);
        }
    }

    /// <summary>
    ///     Museum wrapper class
    /// </summary>
    public class Museum
    {
        public int MuseumID { get; set; }
        public string Description { get; set; }
        public DateTime LastModified { get; set; }
        public Level Privacy { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }

        public Hashtable ToHash()
        {
            var dict = new Hashtable()
            {
                {"MuseumID", MuseumID.ToString()},
                {"Description", Description},
                {"LastModified", LastModified.ToString()},
                {"Privacy", ((int) Privacy).ToString()},
                {"Name", Name},
                {"OwnerName", OwnerName}
            };

            return dict;
        }

        public static Museum Create(Hashtable dict)
        {
            var m = new Museum
            {
                MuseumID = (int) dict["MuseumID"],
                Description = (string) dict["Description"],
                LastModified = DateTime.Parse((string) dict["LastModified"]),
                Privacy = (Level) dict["Privacy"],
                Name = (string) dict["Name"],
                OwnerName = (string) dict["OwnerName"]
            };

            return m;
        }
    }

    public enum Level
    {
        PRIVATE = 0,
        PUBLIC = 1
    }

	public class MuseumSearchModel
	{
		/// <summary>
		/// Substring of the description
		/// </summary>
		public string Description { get; set; }
		
		/// <summary>
		/// Substring of the ownername that needs to be found
		/// </summary>
		public string OwnerName { get; set; }
		
		/// <summary>
		/// Substring of the museum name
		/// </summary>
		public string Name { get; set; }
		
		/// <summary>
		/// The rating of the museums
		/// </summary>
		public int Rating { get; set; }

		public MuseumSearchModel() 
		{
			Description = "";
			OwnerName = "";
			Name = "";
			Rating = 0;
		}

		public string QueryString()
		{
			return "?Description=" + Description + "&OwnerName=" + OwnerName + "&Name=" + Name + "&Rating=" + Rating.ToString ();
		}
	}
}
