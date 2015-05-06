using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using API;

namespace Scanning
{
    public class ArtFilter : Scannable
    {

        public ArtFilter()
        {
            Tags = new List<string>();
            Genres = new List<string>();
        }

		public string ArtistID { get; set; }
		public string ArtId { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Genres { get; set; }


        public void Configure(string s)
        {
            string[] labels = s.Split('?');
            string[] tags = labels[5].Split(',');
            string[] genres = labels[7].Split(',');

            ArtistID = labels[1];
			ArtId = labels [3];
            Tags = new List<string>(tags);
            Genres = new List<string>(genres);
        }

        string Scannable.GetUniqueString()
        {

			String s = "museum.awesomepeople.tv/filter/artist?" + ArtistID;
			s += "?art?";
			if (ArtId != null)
				s += ArtId;
			else
				s += "-1";
			s+="?tags?";
            foreach (string tag in Tags)
            {
                s += tag + ",";
            }
			if(s.Last() == ','){
				s = s.Substring(0, s.Length - 1);
			}
            s += "?genres?";
            foreach (string genre in Genres)
            {
                s += genre + ",";
            }
			if(s.Last() == ','){
            	s = s.Substring(0, s.Length - 1);
			}
            Debug.Log("Scannable string generated succesfully");
            return s;
		}
		
		public void Collect(){
			API.ArtworkFilterController c = API.ArtworkFilterController.Instance;
			API.ArtWorkFilter f = new API.ArtWorkFilter ();
			f.ArtistID = int.Parse(ArtistID);
			f.ArtWorkID = int.Parse(ArtId);
			f.Values = new ArrayList ();
			foreach (string tag in Tags) {
				f.Values.Add (ArtWorkFilter.CreateMetaData("tag", tag));
			}
			foreach (string genre in Genres) {
				f.Values.Add (ArtWorkFilter.CreateMetaData("genre", genre));
			}

			Debug.Log ("TEST TEST "+f.ArtistID+" "+f.ArtWorkID);
			
			c.CreateArtWorkFilter (f, 
			                       (art)=> {
				Debug.Log ("Filter added");
			}, 
			(error) => {
				Debug.Log ("Failed to add filter.");
			}
			);
		}
    }
}
