using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArtData : Data<Art> {

    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public User Owner { get; private set; }
    public List<string> Tags { get; private set; }
    public List<string> Genres { get; private set; }

    public ArtData(int id, string name, string description, User owner, List<string> tags, List<string> genres)
    {
        ID = id;
        Name = name;
        Description = description;
        Owner = owner;
        Tags = tags;
        Genres = genres;
    }
}
