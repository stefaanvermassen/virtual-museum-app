using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MuseumObjectData: Data<MuseumObject>{

    public int ObjectID { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public int Z { get; private set; }
    public float Angle { get; private set; }

    public MuseumObjectData(int objectID, int x, int y, int z, float angle){
        ObjectID = objectID;
        X = x;
        Y = y;
        Z = z;
        Angle = angle;
    }

}
