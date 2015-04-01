using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class ArtData : Data<Art>
{

    public int ID { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public UserData Owner { get; private set; }
    public List<string> Tags { get; private set; }
    public List<string> Genres { get; private set; }
    public byte[] Image { get; private set; }

    public ArtData(int id, string name, string description, UserData owner, List<string> tags, List<string> genres, byte[] image)
    {
        ID = id;
        Name = name;
        Description = description;
        Owner = owner;
        Tags = tags;
        Genres = genres;
        Image = image;
    }
}
