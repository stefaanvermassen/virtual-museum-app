// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEngine;

public class ArtGUIInterface: MonoBehaviour
	{
		private String id;
		private String artistID;
		private String title;
		private Texture2D thumbnail;

		public ArtGUIInterface (string id, string artistID, string title, Texture2D thumbnail)
		{
			this.id = id;
			this.artistID = artistID;
			this.title = title;
			this.thumbnail = thumbnail;
		}

		public string Id {
			get {
				return this.id;
			}
			set {
				id = value;
			}
		}

		public string ArtistID {
			get {
				return this.artistID;
			}
			set {
				artistID = value;
			}
		}

		public string Name {
			get {
				return this.title;
			}
			set {
				title = value;
			}
		}

		public Texture2D Thumbnail {
			get {
				return this.thumbnail;
			}
			set {
				thumbnail = value;
			}
		}
}
