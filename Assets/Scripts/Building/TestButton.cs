using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TestButton : MonoBehaviour {

    public Museum museum;

	// Use this for initialization
	void Start () {
        var button = GetComponent<Button>();
        button.onClick.AddListener(() => {
            var data = museum.Save();
            Stream TestFileStream = File.Create(Application.persistentDataPath + "/test.bin");
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(TestFileStream, data);
            TestFileStream.Close();
            Application.LoadLevel("WalkingController");
        });
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
