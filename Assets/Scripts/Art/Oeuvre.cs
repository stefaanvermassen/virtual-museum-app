using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Art
{
    class Oeuvre
    {
        public List<ArtPiece> Artpieces { get; set; }

        public Oeuvre()
        {
            Artpieces = new List<ArtPiece>();
        }

        public void AddArtPiece(ArtPiece art)
        {
            Artpieces.Add(art);
        }

        public void RemoveArtPiece(ArtPiece art)
        {
            Artpieces.Remove(art);
        }

    }
}
