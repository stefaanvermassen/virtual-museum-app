using UnityEngine;
using System.Collections;

namespace API {

	/// <summary>
	/// Artist controller: control the artist requests
	/// </summary>
	public class ArtistController : APIConnection
	{
		protected ArtistController() {

		}
		
		private static readonly ArtistController _instance = new ArtistController();
		
		public static ArtistController Instance {
			get {
				return _instance;
			}
		}

	}

	public class Artist
	{
		public string Name;
		public int ID;
	}
}

