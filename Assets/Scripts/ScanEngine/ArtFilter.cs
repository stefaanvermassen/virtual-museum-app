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
