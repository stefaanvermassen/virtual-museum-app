using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using System.Collections;
using UnityEngine.UI;

[TestFixture]
public class AddInfoTest
{
    private const int TEST_CASES = 10;

    private void DestroyEverything()
    {
        var objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var o in objects) GameObject.DestroyImmediate(o);
    }
    
    [Test]
    public void Initialize_InitializeInputFields_InitializedFields()
    {
        for (int count = 0; count < TEST_CASES; count++)
        {
            string name = "testnaam";
            string description = "Dit is een lange nutteloze uitleg.";
            AddMuseumInfo script = Initialize(name, description);

            Assert.AreEqual(script.museumName.text, name, "TitleField should be " + name + " but it's " + script.museumName.text);
            Assert.AreEqual(script.museumDescription.text, description, "DescriptionField should be " + description + " but it's " + script.museumDescription.text);
            DestroyEverything();
        }
    }

    [Test]
    public void Save_SaveInfo_InfoUnchanged()
    {
        for (int count = 0; count < TEST_CASES; count++)
        {
            string name = "testnaam";
            string description = "Dit is een lange nutteloze uitleg.";
            AddMuseumInfo script = Initialize(name, description);

            script.Save(0);
            Assert.AreEqual(script.museum.museumName, name, "MuseumName should be " + name + " but it's " + script.museum.museumName);
            Assert.AreEqual(script.museum.description, description, "MuseumDescription should be " + description + " but it's " + script.museum.description);
            
            // Destroy
            script.panel.SetActive(true);
            DestroyEverything();
        }
    }

    [Test]
    public void Save_SaveInfo_InfoSaved()
    {
        for (int count = 0; count < TEST_CASES; count++)
        {
            AddMuseumInfo script = Initialize("n", "b");

            string name = "testnaampje";
            string description = "Dit is een korte nutteloze uitleg.";
            script.museumName.text = name;
            script.museumDescription.text = description;
            script.Save(42);

            Assert.AreEqual(script.museum.museumName, name, "MuseumName should be " + name + " but it's " + script.museum.museumName);
            Assert.AreEqual(script.museum.description, description, "MuseumDescription should be " + description + " but it's " + script.museum.description);
            
            // Destroy
            script.panel.SetActive(true);
            DestroyEverything();
        }
    }

    private AddMuseumInfo Initialize(string name, string description)
    {
        Museum museum = CreateMuseum(name, description);
        var ob = new GameObject();
        AddMuseumInfo script = ob.AddComponent<AddMuseumInfo>();
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
            else 
            {
                throw new AssertionException("Unknown field encountered");
            }
        }
        script.panel = ui.GetComponent<Canvas>().gameObject;

        script.Start();

        return script;
    }

    private Museum CreateMuseum(string name, string description)
    {
        MuseumData data = new MuseumData(new List<MuseumTileData>(), new List<MuseumArtData>(), new List<MuseumObjectData>(), "thomas", name, description, 1, API.Level.PUBLIC);
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.Load(data);
        return museum;
    }
}
