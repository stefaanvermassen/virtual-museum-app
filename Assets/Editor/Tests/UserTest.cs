using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class UserTest : MonoBehaviour {

    [Test]
    public void LoadUserInformation_LoadingUserData_UserDataLoaded()
    {
        UserData data = new UserData(5, "Bert");
        var ob = new GameObject();
        var user = ob.AddComponent<User>();
        user.Load(data);
        Assert.AreEqual(user.name, data.Name, "Name should be " + data.Name + " but it's " + user.name);
    }
}
