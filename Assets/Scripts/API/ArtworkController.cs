using System;
using System.Collections;
using System.Collections.Generic;
using HTTP;
using UnityEngine;

namespace API
{
    /// <summary>
    ///     Artwork controller.
    ///     To create: uploadImage, and after this fill in the necessary fields, and call updateMuseum
    /// </summary>
    public class ArtworkController : APIConnection
    {
        private const string MIME = "image/";
        private const string ARTWORK = "artwork";

        protected ArtworkController()
        {
        }

        private static readonly ArtworkController _Instance = new ArtworkController();

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ArtworkController Instance
        {
            get { return _Instance; }
        }

        /// <summary>
        ///     Gets the artwork.
        /// </summary>
        /// <returns>The artwork.</returns>
        /// <param name="id">Identifier.</param>
        /// <param name="success">Success.</param>
        /// <param name="error">Error.</param>
        public Request GetArtwork(string id, Action<ArtWork> success = null, Action<API_Error> error = null)
        {
            return Get(BASE_URL + ARTWORK + "/" + id, (response =>
            {
                if (success != null)
                {
                    success(ArtWork.Create(response.Object));
                }
            }), error);
        }

        /// <summary>
        ///     Gets all artworks by artist.
        /// </summary>
        /// <returns>The all artworks by artist.</returns>
        /// <param name="artistID">Artist I.</param>
        /// <param name="success">Success.</param>
        /// <param name="error">Error.</param>
        public Request GetAllArtworksByArtist(string artistID, Action<ArrayList> success = null,
            Action<API_Error> error = null)
        {
            return GetArtworksByFilter("?ArtistID=" + artistID, success, error);
        }

        /// <summary>
        ///     Gets the name of the all artworks by.
        /// </summary>
        /// <returns>The all artworks by name.</returns>
        /// <param name="name">Name.</param>
        /// <param name="success">Success.</param>
        /// <param name="error">Error.</param>
        public Request GetAllArtworksByName(string name, Action<ArrayList> success = null,
            Action<API_Error> error = null)
        {
            return GetArtworksByFilter("?Name=" + name, success, error);
        }

        /// <summary>
        ///     Gets all artworks.
        /// </summary>
        /// <returns>The all artworks.</returns>
        /// <param name="success">Success.</param>
        /// <param name="error">Error.</param>
        public Request GetAllArtworks(Action<ArrayList> success = null, Action<API_Error> error = null)
        {
            return GetArtworksByFilter("", success, error);
        }

        /// <summary>
        ///     Gets the artworks by filter.
        /// </summary>
        /// <returns>The artworks by filter.</returns>
        /// <param name="filter">Filter.</param>
        /// <param name="success">Success. Returns an arrayList with Artwork objects</param>
        /// <param name="error">Error.</param>
        private Request GetArtworksByFilter(string filter, Action<ArrayList> success = null,
            Action<API_Error> error = null)
        {
            //TODO: create filter class
            return Get(BASE_URL + ARTWORK + filter, (response =>
            {
                if (success != null)
                {
                    var apiList = (ArrayList) response.Object["ArtWorks"];
                    var list = new ArrayList();
                    foreach (Hashtable val in apiList)
                    {
                        list.Add(ArtWork.Create(val));
                    }
                    success(list);
                }
            }), error);
        }

        /// <summary>
        ///     Gets the artwork data.
        /// </summary>
        /// <returns>The artwork data.</returns>
        /// <param name="id">ArtWork Identifier. The ArtWorkID for which we request the data</param>
        /// <param name="success">Success. Returns a byte[]</param>
        /// <param name="error">Error.</param>
        public Request GetArtworkData(string id, Action<byte[]> success = null, Action<API_Error> error = null, ArtworkSizes size = ArtworkSizes.MOBILE_SMALL)
        {
            return Get(BASE_URL + ARTWORK + "/" + id + "/data?size=" + ((int)size).ToString(), (response =>
            {
                if (success != null)
                {
                    success(response.bytes);
                }
            }), error);
        }

        /// <summary>
        ///     Uploads the image.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="name">Name.</param>
        /// <param name="mime">MIME. Image type, to easily decode on the backend</param>
        /// <param name="imageLocation">Image location. Param to find it by on the backend</param>
        /// <param name="image">Image. the image as a byte[]</param>
        /// <param name="success">Success. Returns an artwork</param>
        /// <param name="error">Error.</param>
        public Request UploadImage(string name, string mime, string imageLocation, byte[] image,
            Action<ArtWork> success = null, Action<API_Error> error = null)
        {
            var form = new WWWForm();
            form.AddBinaryData(imageLocation, image, name, MIME + mime);
            return PostForm(BASE_URL + ARTWORK, form, (response =>
            {
                if (success == null) return;
                var respArray = (ArrayList) response.Object["ArtWorks"];
                var artWork = ArtWork.Create((Hashtable) respArray[0]);
                success(artWork);
            }), error, true);
        }

        /// <summary>
        ///     Updates the art work.
        /// </summary>
        /// <returns>HTTP.Request</returns>
        /// <param name="artwork">Artwork. the artwork to update, filled we the new values.</param>
        /// <param name="success">Success.</param>
        /// <param name="error">Error.</param>
        public Request UpdateArtWork(ArtWork artwork, Action<Response> success = null, Action<API_Error> error = null)
        {
            var form = artwork.ToHash();

            return PutJsonRequest(BASE_URL + ARTWORK + "/" + artwork.ArtWorkID, form, success, error);
        }
    }

    /// <summary>
    ///     Art work.
    /// </summary>
    public class ArtWork
    {
        public int ArtWorkID;
        public int ArtistID;
        //make sure string is not null
        public string Name = "";
        public ArrayList Metadata;

        public Hashtable ToHash()
        {
            var temp = new ArrayList() {"description", "test description"};
            var dict = new Hashtable
            {
                {"ArtWorkID", ArtWorkID.ToString()},
                {"ArtistID", ArtistID.ToString()},
                {"Name", Name},
                {"Metadata", temp}
            };

            return dict;
        }

        public static ArtWork Create(Hashtable dict)
        {
            var aw = new ArtWork
            {
                ArtWorkID = ((int) dict["ArtWorkID"]),
                ArtistID = ((int) dict["ArtistID"]),
                Name = (string) dict["Name"]
            };

            return aw;
        }
		public static ArtWork FromArt (Art art)
		{
			return new ArtWork ()
			{
				ArtWorkID = art.ID,
				ArtistID = art.owner.ID,
				Name = art.name
			};
		}
		public static Art ToArt (ArtWork artwork)
		{
			Art art = new Art ();
			art.ID = artwork.ArtWorkID;
			art.owner.ID = artwork.ArtistID;
			art.name = artwork.Name;
			return art;
		}
    }

	public enum ArtworkSizes {
		MOBILE_SMALL = 1,
		DESKTOP_SMALL = 2,
		MOBILE_LARGE = 3,
		DESKTOP_LARGE = 4,
	    ORIGINAL = 5
	}
}
