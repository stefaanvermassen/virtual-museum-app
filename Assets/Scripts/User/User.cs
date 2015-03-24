using UnityEngine;
using System.Collections;

public class User : MonoBehaviour, Storable<User, UserData> {

    private int ID;
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

    /// <summary>
    /// Returns the id, no setter as we don't want the id of a user to change
    /// </summary>
    /// <returns>The id of the user</returns>
    public int getID()
    {
        return ID;
    }
}
