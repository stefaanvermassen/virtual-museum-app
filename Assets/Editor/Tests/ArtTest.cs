using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class ArtTest {

    [Test]
    public void LoadArtInformation_LoadingArtData_ArtDataLoaded()
    {
        var user = new User();
        UserData userData = new UserData(6, "Rudy");
        user.Load(userData);
        ArtData data = new ArtData(5, "Tekening", "Mooie tekening van Rudy.", userData, new List<string>(), new List<string>(), null);
        var art = new Art();
        art.Load(data);

        Assert.AreEqual(art.ID, data.ID, "ID should be " + data.ID + " but it's " + art.ID);
        Assert.AreEqual(art.name, data.Name, "Name should be " + data.Name + " but it's " + art.name);
        Assert.AreEqual(art.name, data.Name, "Description should be " + data.Description + " but it's " + art.description);
        Assert.AreEqual(art.owner.ID, user.ID, "UserID should be " + user.ID + " but it's " + art.owner.ID);
        Assert.AreEqual(art.owner.name, user.name, "OwnerName should be " + user.name + " but it's " + art.owner.name);
        Assert.IsEmpty(art.tags, "Tags should be empty");
        Assert.IsEmpty(art.genres, "Genres should be empty");
        Assert.IsNotNull(art.image, "Image should not be null");
    }

    [Test]
    public void SaveArtInformation_SaveArtData_ArtDataSaved()
    {
        var user = new User();
        UserData userData = new UserData(6, "Rudy");
        user.Load(userData);
        ArtData data = new ArtData(5, "Tekening", "Mooie tekening van Rudy.", userData, new List<string>(), new List<string>(), null);
        var art = new Art();
        art.Load(data);

        string tag = "Kabouter Wesley";
        string genre1 = "Komedie";
        string genre2 = "Thriller-actie-drama-romantisch";
        string newName = "Redelijke Mooie Tekening";
        
        art.tags.Add(tag);
        art.genres.Add(genre1);
        art.genres.Add(genre2);
        art.name = newName;

        ArtData newData = art.Save();

        Assert.AreEqual(art.ID, newData.ID, "ID should be " + art.ID + " but it's " + newData.ID);
        Assert.AreEqual(newName, newData.Name, "Name should be " + newName + " but it's " + newData.Name);
        Assert.AreEqual(art.name, newData.Name, "Description should be " + art.description + " but it's " + newData.Description);
        Assert.AreEqual(newData.Owner.ID, user.ID, "UserID should be " + user.ID + " but it's " + newData.Owner.ID);
        Assert.AreEqual(newData.Owner.Name, user.name, "OwnerName should be " + user.name + " but it's " + newData.Owner.Name);
        Assert.NotNull(art.tags, "Tags should not be null");
        Assert.NotNull(art.genres, "Genres should not be null");
        Assert.AreEqual(newData.Tags[0], tag, "Tag should be " + tag + " but it's " + newData.Tags[0]);
        Assert.AreEqual(newData.Genres[0], genre1, "Genre1 should be " + genre1 + " but it's " + newData.Genres[0]);
        Assert.AreEqual(newData.Genres[1], genre2, "Genre2 should be " + genre2 + " but it's " + newData.Genres[1]);
        Assert.IsNotNull(art.image, "Image should not be null");
    }
}
