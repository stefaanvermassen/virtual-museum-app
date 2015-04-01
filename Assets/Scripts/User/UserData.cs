using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class UserData : Data<User>
{

    public int ID { get; private set; }
    public string Name { get; private set; }

    public UserData(int id, string name)
    {
        ID = id;
        Name = name;
    }
}
