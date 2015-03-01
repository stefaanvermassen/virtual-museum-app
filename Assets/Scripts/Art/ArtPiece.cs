using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Art
{
    class ArtPiece
    {

        //TODO: image File and database stuff

        public ArtPiece()
        {
            Tags = new List<string>();
            Genres = new List<string>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Genres { get; set; }

    }
}