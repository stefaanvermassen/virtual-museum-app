using UnityEngine;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[TestFixture]
public class Local_SavableData {

    [Test]
    public void Storage_SaveSavableDataLocal_LocalFileMadeInApplicationDataPath()
    {
        
        int testInt = 99;
        string testString = "test_blabla spaties \n newline, leestekenshit !";

        TestSavableData d = new TestSavableData();
        d.TestInt = testInt;
        d.TestString = testString;

        Storage s = Storage.Instance;

        string path = Application.persistentDataPath + "/3DVirtualMuseum/" + d.getFolder() + "/" + d.getFileName() + "." + d.getExtension();
        if(File.Exists(path))File.Delete(path);
        Assert.IsFalse(File.Exists(path));
        s.SaveLocal(d);
        Assert.IsTrue(File.Exists(path));
    }

    [Test]
    public void Storage_LoadSavableDataLocal_SameDataAfterLoad()
    {

        int testInt = 99;
        string testString = "test_blabla spaties \n newline, leestekenshit !";

        TestSavableData d = new TestSavableData();
        d.TestInt = testInt;
        d.TestString = testString;

        Storage s = Storage.Instance;
        s.SaveLocal(d);

        string path = Application.persistentDataPath + "/3DVirtualMuseum/" + d.getFolder() + "/" + d.getFileName() + "." + d.getExtension();
        TestSavableData d2 = new TestSavableData();
        d2 = (TestSavableData) s.LoadLocal(d2, path);

        Assert.AreEqual(d.TestInt, d2.TestInt);
        Assert.AreEqual(d.TestString, d2.TestString);
    }

    [Test]
    [ExpectedException(typeof(FileNotFoundException))]
    public void Storage_LoadSavableDataFromNonExistingFile_IOException()
    {
        int testInt = 99;
        string testString = "test_blabla spaties \n newline, leestekenshit !";

        TestSavableData d = new TestSavableData();
        d.TestInt = testInt;
        d.TestString = testString;

        Storage s = Storage.Instance;
        s.SaveLocal(d);

        string path = Application.persistentDataPath + "/3DVirtualMuseum/" + "NonExistingFile" + "." + "testdata"; //soe = some other extension
        TestSavableData d2 = new TestSavableData();
        d2 = (TestSavableData)s.LoadLocal(d2, path);

    }

    [Test]
    [ExpectedException(typeof(FileLoadException))]
    public void Storage_LoadSavableDataFromFileWithWrongExtension_FileLoadException()
    {
        int testInt = 99;
        string testString = "test_blabla spaties \n newline, leestekenshit !";

        TestSavableData d = new TestSavableData();
        d.TestInt = testInt;
        d.TestString = testString;

        Storage s = Storage.Instance;
        s.SaveLocal(d);

        string path = Application.persistentDataPath + "/3DVirtualMuseum/" + d.getFolder() + "/" + d.getFileName() + "." + "soe"; //soe = some other extension
        File.Create(path);
        TestSavableData d2 = new TestSavableData();
        d2 = (TestSavableData)s.LoadLocal(d2, path);

    }

}

[Serializable]
public class TestSavableData : SavableData
{

    public int TestInt { get; set; }
    public string TestString { get; set; }

    public string getFolder()
    {
        return "testFolder";
    }
    public string getFileName()
    {
        return "savableData";
    }
    public string getExtension()
    {
        return "testdata";
    }

    //These methods should contain server API calls
    public void SaveRemote()
    {
        //non existent, will not be tested here
    }
    public void LoadRemote(string identifier)
    {
        // non existent, will not be tested here
    }
    public DateTime LastModified(string identifier)
    {
        // non existent, will not be tested here
        return new DateTime();
    }
}
