using UnityEngine;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[TestFixture]
public class Local_Savable{

    [Test]
    public void Storage_SaveSavableLocal_LocalFileMadeInApplicationDataPath()
    {
        Museum museum = new Museum();
        museum.ownerID = "testOwner";
        museum.museumID = 10;
        museum.museumName = "testMuseum";
        museum.description = "test description";
        museum.privacy = API.Level.PUBLIC;
        Storage st = Storage.Instance;

        SavableData m = (SavableData)museum; //stupid interfaces and their private methods

        string path = Application.persistentDataPath + "/3DVirtualMuseum/" + m.getFolder() + "/" + m.getFileName() + "." + m.getExtension();
        if (File.Exists(path)) File.Delete(path);
        Assert.IsFalse(File.Exists(path));
        st.SaveLocal<Museum,MuseumData>(museum);
        Assert.IsTrue(File.Exists(path));
       
    }

    [Test]
    public void Storage_LoadSavableLocal_SameDataAfterLoad()
    {
        Museum museum = new Museum();
        museum.ownerID = "testOwner";
        museum.museumID = 10;
        museum.museumName = "testMuseum";
        museum.description = "test description";
        museum.privacy = API.Level.PUBLIC;
        Storage st = Storage.Instance;

        SavableData m = (SavableData)museum; //stupid interfaces and their private methods

        string path = Application.persistentDataPath + "/3DVirtualMuseum/" + m.getFolder() + "/" + m.getFileName() + "." + m.getExtension();
        if (File.Exists(path)) File.Delete(path);
        st.SaveLocal<Museum, MuseumData>(museum);

        var ob = new GameObject();
        Museum museum2 = ob.AddComponent<Museum>();
        st.LoadLocal<Museum, MuseumData>(museum2,path);

        Assert.AreEqual(museum.ownerID, museum2.ownerID);
        Assert.AreEqual(museum.museumID, museum2.museumID);
        Assert.AreEqual(museum.museumName, museum2.museumName);
        Assert.AreEqual(museum.description, museum2.description);
        Assert.AreEqual(museum.privacy, museum2.privacy);

    }

    [Test]
    [ExpectedException(typeof(FileNotFoundException))]
    public void Storage_LoadSavableDataFromNonExistingFile_IOException()
    {
        Museum museum = new Museum();
        museum.ownerID = "testOwner";
        museum.museumID = 10;
        museum.museumName = "testMuseum";
        museum.description = "test description";
        museum.privacy = API.Level.PUBLIC;
        Storage st = Storage.Instance;

        SavableData m = (SavableData)museum; //stupid interfaces and their private methods

        string path = Application.persistentDataPath + "/3DVirtualMuseum/" + m.getFolder() + "/" + "NonExistingFile" + "." + m.getExtension();
        if (File.Exists(path)) File.Delete(path);
        st.SaveLocal<Museum, MuseumData>(museum);

        Museum museum2 = new Museum();
        st.LoadLocal<Museum, MuseumData>(museum2, path);

    }

    [Test]
    [ExpectedException(typeof(FileLoadException))]
    public void Storage_LoadSavableDataFromFileWithWrongExtension_FileLoadException()
    {
        Museum museum = new Museum();
        museum.ownerID = "testOwner";
        museum.museumID = 10;
        museum.museumName = "testMuseum";
        museum.description = "test description";
        museum.privacy = API.Level.PUBLIC;
        Storage st = Storage.Instance;

        SavableData m = (SavableData)museum; //stupid interfaces and their private methods

        string path = Application.persistentDataPath + "/3DVirtualMuseum/" + m.getFolder() + "/" + "NonExistingFile" + "." + "soe"; //soe = some other extension
        File.Create(path);

        Museum museum2 = new Museum();
        st.LoadLocal<Museum, MuseumData>(museum2, path);

    }

   
}
