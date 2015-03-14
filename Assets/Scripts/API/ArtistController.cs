using UnityEngine;
using System.Collections;

namespace API {
	
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
	
}

