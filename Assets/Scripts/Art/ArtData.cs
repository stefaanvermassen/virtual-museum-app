using UnityEngine;
using System.Collections;

public class ArtData : Data<Art> {

    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public User Owner { get; private set; }

    public ArtData(int id, string name, string description, User owner)
    {
        ID = id;
        Name = name;
        Description = description;
        Owner = owner;
    }
}
