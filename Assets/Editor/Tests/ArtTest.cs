using UnityEngine;
using System.Collections;
using NUnit.Framework;

[TestFixture]
public class ArtTest : MonoBehaviour {

    [Test]
    public void LoadArtInformation_LoadingArtData_ArtDataLoaded()
    {
        var ob = new GameObject();
        var user = ob.AddComponent<User>();
        UserData userData = new UserData(6, "Rudy");
        user.Load(userData);
        ArtData data = new ArtData(5, "Tekening", "Mooie tekening van Rudy.", user);
        var art = ob.AddComponent<Art>();
        art.Load(data);

        Assert.AreEqual(art.ID, data.ID, "ID should be " + data.ID + " but it's " + art.ID);
        Assert.AreEqual(art.name, data.Name, "Name should be " + data.Name + " but it's " + art.name);
        Assert.AreEqual(art.name, data.Name, "Description should be " + data.Description + " but it's " + art.description);
        Assert.AreEqual(art.owner.ID, user.ID, "UserID should be " + user.ID + " but it's " + art.owner.ID);
        Assert.AreEqual(art.owner.name, user.name, "OwnerName should be " + user.name + " but it's " + art.owner.name);
    }
}
