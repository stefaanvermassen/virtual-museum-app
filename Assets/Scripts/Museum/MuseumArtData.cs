using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MuseumArtData: Data<MuseumArt>{

    public int ArtID { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }
    public int Orientation { get; private set; }

    public MuseumArtData(int artID, int x, int y, int z, int orientation) {
        ArtID = artID;
        X = x;
        Y = y;
        Z = z;
        Orientation = orientation;
    }

}
