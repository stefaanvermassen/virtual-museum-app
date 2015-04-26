using UnityEngine;
using System.Collections;

public class User : Storable<User, UserData>
{

    public int ID { get; set; }
    public string name;

    /// <summary>
    /// Create a UserData for serialization.
    /// </summary>
    /// <returns>The UserData</returns>
    public UserData Save()
    {
        return new UserData(ID, name);
    }

    /// <summary>
    /// Load a UserData in this art.
    /// </summary>
    /// <param name="data">The UserData</param>
    public void Load(UserData data)
    {
        ID = data.ID;
        name = data.Name;
    }
}
