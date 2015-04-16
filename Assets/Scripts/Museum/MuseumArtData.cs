using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MuseumArtData: Data<MuseumArt>{

    public ArtData Art { get; private set; }
    public float X { get; private set; }
    public float Y { get; private set; }
    public float Z { get; private set; }
    public float RX { get; private set; }
    public float RY { get; private set; }
    public float RZ { get; private set; }
    public float Scale { get; private set; }
    public int FrameStyle { get; private set; }

    public MuseumArtData(ArtData art, float x, float y, float z, float rx, float ry, float rz, float scale, int frameStyle) {
        Art = art;
        X = x;
        Y = y;
        Z = z;
        RX = rx;
        RY = ry;
        RZ = rz;
        Scale = scale;
        FrameStyle = frameStyle;
    }

}
