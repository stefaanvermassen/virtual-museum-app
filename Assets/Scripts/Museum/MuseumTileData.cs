using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using UnityEngine;

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

	[OptionalField]
	public SerializableColor CeilingColor;

	[OptionalField]
	public SerializableColor WallColor;

	[OptionalField]
	public SerializableColor FloorColor;

    public MuseumTileData(int x, int y, int z, bool left, bool right, bool front, bool back, int ceilingStyle, int wallStyle, int floorStyle,
	                      Color wallColor, Color floorColor, Color ceilingColor) {
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
		CeilingColor = new SerializableColor (ceilingColor);
		WallColor = new SerializableColor (wallColor);
		FloorColor = new SerializableColor (floorColor);
    }

	[System.Serializable]
	public class SerializableColor {
		public float[] Color { get; set; }
		public SerializableColor(Color color)
		{
			Color = new float[4];
			for(int i = 0; i < 4; i++) {
				Color[i] = color[i];
			}
		}

		public Color ToColor() {
			return new Color(Color[0], Color[1], Color[2], Color[3]);
		}
	}

}
