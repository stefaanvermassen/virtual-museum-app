using UnityEngine;
using System.Collections;

public class User : MonoBehaviour, Storable<User, UserData> {

    public int ID;
    public string name;

    public UserData Save()
    {
        return new UserData(ID, name);
    }

    public void Load(UserData data)
    {
        ID = data.ID;
        name = data.Name;
    }
}
