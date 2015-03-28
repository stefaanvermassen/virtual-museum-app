using UnityEngine;
using System.Collections;
using NUnit.Framework;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[TestFixture]
public class Remote_Savable
{

    [Test]
    public void Storage_LoadSavableRemote_APIRequestOK()
    {
        Museum museum = new Museum();
        Storage st = Storage.Instance;
        st.LoadRemote(museum, "1");
    }


}