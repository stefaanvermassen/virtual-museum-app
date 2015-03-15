using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MuseumTileData: Data<MuseumTile> {

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool Front { get; set; }
    public bool Back { get; set; }
    public int CeilingStyle { get; set; }
    public int WallStyle { get; set; }
    public int FloorStyle { get; set; }

    public MuseumTileData(int x, int y, int z, bool left, bool right, bool front, bool back, int ceilingStyle, int wallStyle, int floorStyle) {
        X = x;
        Y = y;
        Z = z;
        Left = left;
        Right = right;
        Front = front;
        Back = back;
        CeilingStyle = ceilingStyle;
        WallStyle = wallStyle;
        FloorStyle = floorStyle;
    }

}
