using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//probably inherits from user or something like that

    class Artist
    {

        private string ArtistName { get; set; }
        public Oeuvre Oeuvre { get; set; }

        public Artist(string name)
        {
            ArtistName = name;
            Oeuvre = new Oeuvre();
        }
    }

