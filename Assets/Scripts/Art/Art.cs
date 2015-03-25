using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Art : Storable<Art, ArtData>
{

    public int ID { get; set; }
    public string name;
    public string description;
    public User owner;
    public List<string> tags = new List<string>();
    public List<string> genres = new List<string>();
    public byte[] image { get; private set; }

    public Art() {
        owner = new User();
    }

    /// <summary>
    /// Create an ArtData for serialization.
    /// </summary>
    /// <returns>The ArtData</returns>
    public ArtData Save()
    {
        var userData = owner.Save();
        return new ArtData(ID, name, description, userData, tags, genres, image);
    }

    /// <summary>
    /// Load an ArtData in this art.
    /// </summary>
    /// <param name="data">The ArtData</param>
    public void Load(ArtData data)
    {
        ID = data.ID;
        name = data.Name;
        description = data.Description;
        owner.Load(data.Owner);
        tags = data.Tags;
        genres = data.Genres;
        image = data.Image;
    }
}
