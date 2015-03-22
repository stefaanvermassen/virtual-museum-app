﻿using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UI;

[TestFixture]
public class AddInfoTest
{
    private const int TEST_CASES = 10;
    private const int SEED = 987;

	public AddInfoTest()
	{
        Random.seed = SEED;
	}

    [Test]
    public void testStart()
    {
        for (int count = 0; count < TEST_CASES; count++)
        {
            string ownerid = RandomString(0, 5);
            string name = RandomString(0, 20);
            string description = RandomString(0, 500);
            Museum museum = createMuseum(ownerid, name, description);
            var ob = new GameObject();
            var script = ob.AddComponent<AddMuseumInfo>();
            GameObject ui = (GameObject)GameObject.Instantiate(Resources.Load("AddInfoUI"));

            script.museum = museum;
            InputField[] fields = ui.GetComponentsInChildren<InputField>();
            foreach (InputField i in fields)
            {
                if (i.CompareTag("TitleField"))
                {
                    script.museumName = i;
                }
                else if (i.CompareTag("DescriptionField"))
                {
                    script.museumDescription = i;
                }
            }
            script.panel = ui.GetComponent<Canvas>().gameObject;

            script.Start();

            Assert.AreEqual(script.museumName.text, name, "TitleField should be " + name + " but it's " + script.museumName.text);
            Assert.AreEqual(script.museumDescription.text, description, "DescriptionField should be " + description + " but it's " + script.museumDescription.text);
            testSave(script, name, description);
        }
    }

    private void testSave(AddMuseumInfo script, string name, string description)
    {
        script.save(RandomInt(0, int.MaxValue));
        Assert.AreEqual(script.museum.museumName, name, "MuseumName should be " + name + " but it's " + script.museum.museumName);
        Assert.AreEqual(script.museum.description, description, "MuseumDescription should be " + description + " but it's " + script.museum.description);
    }

    private Museum createMuseum(string ownerid, string name, string description)
    {
        MuseumData data = new MuseumData(new List<MuseumTileData>(), new List<MuseumArtData>(), new List<MuseumObjectData>(), ownerid, name, description);
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.Load(data);
        return museum;
    }

    private int RandomInt(int from, int until)
    {
        return (int)(Random.value * (from + until) - from);
    }

    private string RandomString(int minLength, int maxLength)
    {
        string s = "";
        int length = RandomInt(minLength, maxLength);
        for (int i = 0; i < length; i++)
        {
            char c = (char)RandomInt(0, 255);
            s += c;
        }
        return s;
    }
}
