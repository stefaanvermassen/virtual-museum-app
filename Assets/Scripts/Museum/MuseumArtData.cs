using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MuseumArtData: Data<MuseumArt>{

    public ArtData Art { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }
    public int Orientation { get; private set; }

    public MuseumArtData(ArtData art, int x, int y, int z, int orientation) {
        Art = art;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
    }

}
