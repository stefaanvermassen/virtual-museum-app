using UnityEngine;
using System.Threading;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[TestFixture]
public class Remote_Savable
{

    [Test]
    public void Storage_LoadSavableRemote_APIRequestSet()
    {
        Museum museum = new Museum();
        Storage st = Storage.Instance;
        st.LoadRemote(museum, "1");
        Assert.AreEqual("http://api.awesomepeople.tv/api/museum/1/data", museum.req.uri.ToString());
        //if it is Okay or wheter the data was loaded correctly is not the Storage responsibility, this test should not break because of bugs in that code
    }

    [Test]
    public void Storage_SaveSavableRemote_APIRequestSet()
    {
        Museum museum = new Museum();
        museum.museumID = 10;
        museum.museumName = "TestUpdatedName";
        museum.description = "test description";
        museum.privacy = API.Level.PUBLIC;
        Storage st = Storage.Instance;
        st.SaveRemote(museum);
        Debug.Log(museum.req.uri);
        Assert.AreEqual("http://api.awesomepeople.tv/api/museum/10", museum.req.uri.ToString());
        //if it is Okay or wheter the data was saved correctly is not the Storage responsibility, this test should not break because of bugs in that code
    }




}