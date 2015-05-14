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
			Debug.Log (BASE_URL + ARTWORK + "/" + artwork.ArtWorkID);
            return PutJsonRequest(BASE_URL + ARTWORK + "/" + artwork.ArtWorkID, form, success, error);
        }
    }

    /// <summary>
    ///     Art work.
    /// </summary>
    public class ArtWork
    {
        public int ArtWorkID;
        public Artist Artist;
        //make sure string is not null
        public string Name = "";
        public ArrayList Metadata;

        public Hashtable ToHash()
        {
            var dict = new Hashtable
            {
                {"ArtWorkID", ArtWorkID.ToString()},
                {"ArtistID", Artist.ID.ToString()},
                {"Name", Name},
                {"Metadata", Metadata}
            };

            return dict;
        }

		public static Hashtable CreateMetaData(string key, string value) {
			var dict = new Hashtable
			{
				{"Name", key},
				{"Value", value}
			};
			
			return dict;
		}

        public static ArtWork Create(Hashtable dict)
        {
            var aw = new ArtWork
            {
                ArtWorkID = ((int) dict["ArtWorkID"]),
                Artist = Artist.Create((Hashtable)dict["Artist"]),
                Name = (string) dict["Name"],
				Metadata = (ArrayList) dict["Metadata"]
            };

            return aw;
        }

		public static ArtWork FromArt (Art art)
		{
			var artist = new Artist (){
				Name = art.owner.name,
				ID = art.owner.ID
			};
			var list = new ArrayList ();
			if(art.description != null) {
				list.Add (ArtWork.CreateMetaData("Description", art.description));
			}
			return new ArtWork ()
			{
				ArtWorkID = art.ID,
				Artist = artist,
				Name = art.name,
				Metadata = list
			};
		}

		public static Art ToArt (ArtWork artwork)
		{
			Art art = new Art ();
			art.ID = artwork.ArtWorkID;
			art.owner.ID = artwork.Artist.ID;
			art.owner.name = artwork.Artist.Name;
			art.name = artwork.Name;
			foreach (Hashtable dict in artwork.Metadata) {
				if(dict.ContainsKey("Name") && dict.ContainsKey("Value")) {
					string key = (string) dict["Name"];
					string value = (string) dict["Value"];
					if(key.ToLower().Equals("description")) {
						art.description = value;
					} else if(key.ToLower().Equals("genre")) {
						art.genres.Add (value);
					}
				}
			}
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
