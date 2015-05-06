using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Scanning
{
    public class ArtFilter : Scannable
    {

        public ArtFilter()
        {
            Tags = new List<string>();
            Genres = new List<string>();
        }

		public string ArtistName { get; set; }
		public int ArtId { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Genres { get; set; }


        public void Configure(string s)
        {
            string[] labels = s.Split('?');
            string[] tags = labels[5].Split(',');
            string[] genres = labels[7].Split(',');

            ArtistName = labels[1];
			ArtId = labels [3];
            Tags = new List<string>(tags);
            Genres = new List<string>(genres);
        }

        string Scannable.GetUniqueString()
        {

			String s = "museum.awesomepeople.tv/filter/artist?" + ArtistName;
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
            s = s.Substring(0, s.Length - 1);
            s += "?genres?";
            foreach (string genre in Genres)
            {
                s += genre + ",";
            }
            s = s.Substring(0, s.Length - 1);
            Debug.Log("Scannable string generated succesfully");
            return s;
        }
    }
}
