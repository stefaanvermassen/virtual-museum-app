using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scanning
{
    public class ArtFilter : Scannable
    {

        public ArtFilter()
        {

        }

        public string ArtistName { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Genres { get; set; }


        public void configure(string s)
        {
            string[] labels = s.Split(':');
            string[] tags = labels[3].Split(',');
            string[] genres = labels[5].Split(',');

            ArtistName = labels[1];
            Tags = new List<string>(tags);
            Genres = new List<string>(genres);
        }

        string Scannable.GetUniqueString()
        {
            String s = "artist:" + ArtistName + ":tags:";
            foreach (string tag in Tags)
            {
                s += tag + ",";
            }
            s = s.Substring(0, s.Length - 1);
            s += ":genres:";
            foreach (string genre in Genres)
            {
                s += genre + ",";
            }
            s = s.Substring(0, s.Length - 1);
            return s;
        }
    }
}
