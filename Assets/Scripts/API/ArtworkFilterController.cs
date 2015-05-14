using System;
using System.Collections;
using System.Collections.Generic;
using HTTP;
using UnityEngine;

namespace API
{
    /// <summary>
    ///     Artworkfilter controller.
    ///     updateMuseum
    /// </summary>
    public class ArtworkFilterController : APIConnection
    {
        private const string ARTWORKFILTER = "artworkfilter";

        protected ArtworkFilterController()
        {
        }

        private static readonly ArtworkFilterController _Instance = new ArtworkFilterController();

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ArtworkFilterController Instance
        {
            get { return _Instance; }
        }

    

        /// <summary>
        ///     Gets all artworkfilters that are connected to the user.
        /// </summary>
        /// <returns>The all artworks.</returns>
        /// <param name="success">Success.</param>
        /// <param name="error">Error.</param>
        public Request GetConnectedArtworkFilters(Action<ArrayList> success = null, Action<API_Error> error = null)
        {
			return Get(BASE_URL + ARTWORKFILTER + "/connected", (response =>
			                                         {
				if (success != null)
				{
					var apiList = (ArrayList) response.Object["ArtWorkFilters"];
					var list = new ArrayList();
					foreach (Hashtable val in apiList)
					{
						list.Add(ArtWorkFilter.Create(val));
					}
					success(list);
				}
			}), error);
        }

        /// <summary>
        /// Creates the art work filter.
        /// </summary>
        /// <returns>The art work filter.</returns>
		/// <param name="filter">ArtWorkFilter is an API class which contains all necessary fields, ArtWorkFilterID can be left empty when
		///     creating a new museum</param>
		/// <param name="success">Success. A closure which will be executed when creating the artworkfilter is a succes, gives you a
		///     artworkfilter object with the needed id.</param>
		/// <param name="error">Error. Handle errors</param>
		public Request CreateArtWorkFilter(ArtWorkFilter filter, Action<Response> success = null, Action<API_Error> error = null)
		{
			var form = filter.ToHash();
			return PostJsonRequest(BASE_URL + ARTWORKFILTER, form, success, error);
		}
    }

    /// <summary>
    ///     Artworkfilter.
    /// </summary>
    public class ArtWorkFilter
    {
        public int ArtWorkFilterID;

		public int ArtWorkID;

		public ArrayList Values;
 
		public int ArtistID;
    

        public Hashtable ToHash()
        {
            var dict = new Hashtable
            {
				{"Id", ArtWorkFilterID.ToString()},
                {"Pairs", Values},
                {"ArtistID", ArtistID},
				{"ArtWorkID", ArtWorkID}
            };

            return dict;
        }

        public static ArtWorkFilter Create(Hashtable dict)
        {
            var awf = new ArtWorkFilter
            {
				ArtWorkFilterID = ((int) dict["Id"]),
                Values = (ArrayList) dict["Pairs"],
				ArtistID = ((int) dict["ArtistID"]),
				ArtWorkID = ((int) dict["ArtWorkID"])
            };

            return awf;
        }

		public static Hashtable CreateMetaData(string key, string value) {
			var dict = new Hashtable
			{
				{"Name", key},
				{"Value", value}
			};
			
			return dict;
		}
		

    }
}
