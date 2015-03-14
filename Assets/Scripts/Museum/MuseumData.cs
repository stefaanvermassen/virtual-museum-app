using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MuseumData: Data<Museum>{

    public List<MuseumTileData> Tiles { get; private set; }
    public List<MuseumArtData> Art { get; private set; }
    public List<MuseumObjectData> Objects { get; private set; }
    public string Author { get; private set; }
    public string MuseumName { get; private set; }
    public string Description { get; private set; }

    public MuseumData(List<MuseumTileData> tiles, List<MuseumArtData> art, List<MuseumObjectData> objects, string author, string museumName, string description) {
        Tiles = tiles;
        Art = art;
        Objects = objects;
        Author = author;
        MuseumName = museumName;
        Description = description;
    }

}
