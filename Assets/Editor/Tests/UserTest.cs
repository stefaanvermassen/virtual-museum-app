using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class UserTest {

    [Test]
    public void LoadUserInformation_LoadingUserData_UserDataLoaded()
    {
        UserData data = new UserData(5, "Bert");
        var user = new User();
        user.Load(data);
        Assert.AreEqual(user.ID, data.ID, "ID should be " + data.ID + " but it's " + user.ID);
        Assert.AreEqual(user.name, data.Name, "Name should be " + data.Name + " but it's " + user.name);
    }

    [Test]
    public void SaveUserInformation_SaveNewName_NewNameSaved()
    {
        UserData data = new UserData(5, "Bert");
        var user = new User();
        user.Load(data);
        string newName = "Rudyniet";
        user.name = newName;
        UserData newData = user.Save();
        Assert.AreEqual(newName, newData.Name, "Name should be " + newName + " but it's " + newData.Name);
    }
}
