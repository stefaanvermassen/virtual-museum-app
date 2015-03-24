using UnityEngine;
using System.Collections;

public class Art : MonoBehaviour, Storable<Art, ArtData>
{

    public int ID { get; private set; }
    public string name;
    public string description;
    public User owner;

    /// <summary>
    /// Create an ArtData for serialization.
    /// </summary>
    /// <returns>The ArtData</returns>
    public ArtData Save()
    {
        return new ArtData(ID, name, description, owner);
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
        owner = data.Owner;
    }
}
