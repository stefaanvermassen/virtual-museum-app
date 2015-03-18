using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MuseumData: Data<Museum>{

    public List<MuseumTileData> Tiles { get; private set; }
    public List<MuseumArtData> Art { get; private set; }
    public List<MuseumObjectData> Objects { get; private set; }
    public string OwnerID { get; private set; }
    public string MuseumName { get; private set; }
    public string Description { get; private set; }

    public MuseumData(List<MuseumTileData> tiles, List<MuseumArtData> art, List<MuseumObjectData> objects, string ownerID, string museumName, string description) {
        Tiles = tiles;
        Art = art;
        Objects = objects;
        OwnerID = ownerID;
        MuseumName = museumName;
        Description = description;
    }

}
