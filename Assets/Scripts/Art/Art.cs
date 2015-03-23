using UnityEngine;
using System.Collections;

public class Art : MonoBehaviour, Storable<Art, ArtData>
{

    public int ID;
    public string name;
    public string description;
    public User owner;

    public ArtData Save()
    {
        return new ArtData(ID, name, description, owner);
    }

    public void Load(ArtData data)
    {
        ID = data.ID;
        name = data.Name;
        description = data.Description;
        owner = data.Owner;
    }
}
