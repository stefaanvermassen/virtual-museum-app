using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class MuseumData: Data<Museum>{

    public List<MuseumTileData> Tiles { get; set; }
    public string Author { get; set; }
    public string MuseumName { get; set; }
    public string Description { get; set; }

    public MuseumData(List<MuseumTileData> tiles, string author, string museumName, string description) {
        Tiles = tiles;
        Author = author;
        MuseumName = museumName;
        Description = description;
    }

}
